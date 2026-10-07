using System;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace HealthPulse
{
    // Low health shows at the edges of the screen: a red vignette that beats like a heart,
    // stronger and faster the lower the health, with a flash when a hit lands, and optionally a
    // heartbeat sound. The vignette is a nested canvas in the game's GUI canvas, sorted just
    // below it, outside hudroot: the whole HUD draws over it, and HUD mods that pick up what
    // other mods put into hudroot (HudLayout) leave it alone. The texture and the sound are made
    // in code, there are no asset files. Client only: nothing is sent and nothing is patched.
    [BepInPlugin(Guid, Name, Version)]
    public class HealthPulsePlugin : BaseUnityPlugin
    {
        public const string Guid = "j1ga.healthpulse";
        public const string Name = "HealthPulse";
        public const string Version = "0.1.0";

        private const string SecGeneral = "01 General";
        private const string SecLook = "02 Look";
        private const string SecHit = "03 Hit flash";
        private const string SecSound = "04 Heartbeat sound";

        private ConfigEntry<bool> _enabled;
        private ConfigEntry<float> _threshold;
        private ConfigEntry<bool> _hideWithHud;
        private ConfigEntry<float> _maxAlpha;
        private ConfigEntry<Color> _color;
        private ConfigEntry<float> _width;
        private ConfigEntry<float> _bpmMin, _bpmMax;
        private ConfigEntry<bool> _hitFlash;
        private ConfigEntry<float> _hitStrength;
        private ConfigEntry<bool> _sound;
        private ConfigEntry<float> _soundThreshold;
        private ConfigEntry<float> _soundVolume;
        private ConfigEntry<int> _configVersion;
        private const int ConfigVersion = 2;

        private Image _image;
        private GameObject _root;
        private Texture2D _tex;
        private Sprite _sprite;
        private float _texWidth = -1f;
        private Hud _hud;

        private AudioSource _audio;
        private AudioClip _beat;
        private bool _soundLogged;

        private float _phase;          // 0..1 within the current heartbeat
        private float _shown;          // the vignette's current strength, eased
        private float _flash;          // the hit flash, decays
        private float _lastHealth = -1f;

        private void Awake()
        {
            _enabled = Config.Bind(SecGeneral, "Enabled", true, "Show the low-health vignette.");
            _threshold = Config.Bind(SecGeneral, "Threshold", 35f, new ConfigDescription(
                "Health, in percent of the maximum, below which the edges start to glow. The lower the health, the stronger and faster.", new AcceptableValueRange<float>(5f, 90f)));
            _hideWithHud = Config.Bind(SecGeneral, "HideWithHud", false,
                "Hide the vignette too when the HUD is hidden (Ctrl+F3). Off: it stays, as a warning.");

            _maxAlpha = Config.Bind(SecLook, "MaxStrength", 0.75f, new ConfigDescription(
                "How opaque the edges get at the lowest health (0..1).", new AcceptableValueRange<float>(0.1f, 1f)));
            _color = Config.Bind(SecLook, "Color", new Color(0.75f, 0f, 0f, 1f), "Colour of the vignette.");
            _width = Config.Bind(SecLook, "Width", 0.45f, new ConfigDescription(
                "How far the glow reaches in from the edges (0.2 a thin frame .. 0.8 most of the screen).", new AcceptableValueRange<float>(0.2f, 0.8f)));
            _bpmMin = Config.Bind(SecLook, "BeatsPerMinuteAtThreshold", 45f, new ConfigDescription(
                "Heart rate of the pulse just below the threshold.", new AcceptableValueRange<float>(30f, 200f)));
            _bpmMax = Config.Bind(SecLook, "BeatsPerMinuteAtZero", 80f, new ConfigDescription(
                "Heart rate of the pulse near zero health.", new AcceptableValueRange<float>(30f, 220f)));

            _hitFlash = Config.Bind(SecHit, "Enabled", true, "Flash the edges when you take a hit; the lower the health, the stronger.");
            _hitStrength = Config.Bind(SecHit, "Strength", 1f, new ConfigDescription(
                "How strong the flash is.", new AcceptableValueRange<float>(0.1f, 2f)));

            _sound = Config.Bind(SecSound, "Enabled", false, "Play a heartbeat with the pulse when health is very low.");
            _soundThreshold = Config.Bind(SecSound, "Threshold", 15f, new ConfigDescription(
                "Health, in percent, below which the heartbeat is heard.", new AcceptableValueRange<float>(5f, 90f)));
            _soundVolume = Config.Bind(SecSound, "Volume", 0.8f, new ConfigDescription(
                "Volume of the heartbeat (the game's sound volume applies on top).", new AcceptableValueRange<float>(0f, 1f)));
            _configVersion = Config.Bind("99 Internal", "ConfigVersion", 0, "Used by the mod to update old defaults once. Do not change.");
            MigrateConfig();

            Logger.LogInfo(Name + " " + Version + " loaded. No Harmony patches are applied.");
        }

        // 0.1.0 test builds had a faster, sharper pulse and a quieter sound: values still at those
        // defaults move to the new ones, anything changed by hand stays
        private void MigrateConfig()
        {
            if (_configVersion.Value >= ConfigVersion) return;
            if (Mathf.Approximately(_bpmMin.Value, 60f)) _bpmMin.Value = 45f;
            if (Mathf.Approximately(_bpmMax.Value, 120f)) _bpmMax.Value = 80f;
            if (Mathf.Approximately(_soundVolume.Value, 0.6f)) _soundVolume.Value = 0.8f;
            _configVersion.Value = ConfigVersion;
        }

        private void OnDestroy()
        {
            if (_root != null) Destroy(_root);
            if (_sprite != null) Destroy(_sprite);
            if (_tex != null) Destroy(_tex);
            if (_beat != null) Destroy(_beat);
        }

        private void Update()
        {
            try { Tick(); }
            catch (Exception e)
            {
                Logger.LogError("Update failed, the vignette is off for the session: " + e);
                enabled = false;
                if (_image != null) _image.enabled = false;
            }
        }

        private void Tick()
        {
            Hud hud = Hud.instance;
            if (hud == null) { _hud = null; _image = null; return; }
            if (hud != _hud || _image == null) Attach(hud);
            if (_image == null) return;
            if (!Mathf.Approximately(_texWidth, _width.Value)) MakeTexture();

            float dt = Time.unscaledDeltaTime;
            Player p = Player.m_localPlayer;
            bool show = _enabled.Value && p != null && !p.IsDead() && !p.IsTeleporting() && !p.IsSleeping()
                        && (!_hideWithHud.Value || !hud.m_userHidden);
            float max = p != null ? p.GetMaxHealth() : 0f;
            float hp = p != null && max > 0f ? Mathf.Clamp01(p.GetHealth() / max) : 1f;

            // how bad it is: 0 at the threshold, 1 at zero health
            float thr = _threshold.Value / 100f;
            float sev = show ? Mathf.Clamp01((thr - hp) / thr) : 0f;
            bool low = show && hp < thr;

            // the heartbeat: two beats, "lub-dub", faster as it gets worse
            float bpm = Mathf.Lerp(_bpmMin.Value, _bpmMax.Value, sev);
            float before = _phase;
            _phase += dt * bpm / 60f;
            if (_phase >= 1f) _phase -= Mathf.Floor(_phase);
            // the sound lands with the first swell, a little before its peak
            bool newBeat = _phase >= 0.02f && (before < 0.02f || before > _phase);
            float beat = Mathf.Max(Bump(_phase, 0.08f), 0.55f * Bump(_phase, 0.38f));

            // the hit flash: a drop in health, scaled by how low it is now
            if (show && _lastHealth >= 0f && p != null)
            {
                float drop = (_lastHealth - p.GetHealth()) / Mathf.Max(1f, max);
                if (_hitFlash.Value && drop > 0.005f)
                {
                    float near = Mathf.Clamp01((thr * 1.5f - hp) / (thr * 1.5f));
                    _flash = Mathf.Min(1f, _flash + (0.35f + drop * 3f) * near * _hitStrength.Value);
                }
            }
            _lastHealth = show && p != null ? p.GetHealth() : -1f;
            _flash = Mathf.MoveTowards(_flash, 0f, dt / 0.45f);

            float target = low ? (0.3f + 0.7f * sev) * (0.72f + 0.28f * beat) : 0f;
            // eased towards the target: soft swells instead of hard steps
            _shown = Mathf.Lerp(_shown, target, 1f - Mathf.Exp(-dt / 0.12f));
            float alpha = Mathf.Clamp01(Mathf.Max(_shown, _flash) * _maxAlpha.Value);

            Color c = _color.Value;
            c.a = alpha;
            _image.color = c;
            _image.enabled = alpha > 0.003f;

            // the sound, on the first beat of each heartbeat
            if (newBeat && _sound.Value && show && hp < _soundThreshold.Value / 100f) PlayBeat(hp);
        }

        // a soft swell around c, a sixth of a heartbeat wide, wrapping round the cycle
        private static float Bump(float x, float c)
        {
            float d = x - c;
            if (d > 0.5f) d -= 1f; else if (d < -0.5f) d += 1f;
            d /= 0.15f;
            return Mathf.Exp(-d * d);
        }

        // ------------------------------------------------------------------
        // the vignette: a full-screen image in its own nested canvas, sorted just below the
        // game's GUI canvas, so the whole HUD is drawn over it; outside hudroot, so HUD mods
        // that pick up other mods' objects there do not take it for a HUD element
        // ------------------------------------------------------------------
        private void Attach(Hud hud)
        {
            _hud = hud;
            _image = null;
            if (_root != null) Destroy(_root);
            Canvas hudCanvas = hud.m_rootObject != null ? hud.m_rootObject.GetComponentInParent<Canvas>() : null;
            Canvas top = hudCanvas != null ? hudCanvas.rootCanvas : null;
            if (top == null) { Logger.LogWarning("The HUD's canvas was not found; no vignette."); return; }

            _root = new GameObject("HealthPulse", typeof(RectTransform));
            _root.layer = top.gameObject.layer;
            RectTransform rt = (RectTransform)_root.transform;
            rt.SetParent(top.transform, false);
            rt.SetAsFirstSibling();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            Canvas own = _root.AddComponent<Canvas>();
            own.overrideSorting = true;
            own.sortingLayerID = top.sortingLayerID;
            own.sortingOrder = top.sortingOrder - 1;

            _image = _root.AddComponent<Image>();
            _image.raycastTarget = false;
            _image.enabled = false;
            _texWidth = -1f;
            MakeTexture();
            _shown = 0f; _flash = 0f; _lastHealth = -1f;
        }

        // White with alpha rising towards the edges; the colour comes from the image's tint. An
        // ellipse stretched over the screen, so the glow follows its shape.
        private void MakeTexture()
        {
            _texWidth = _width.Value;
            const int size = 256;
            if (_tex == null)
            {
                _tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                _tex.wrapMode = TextureWrapMode.Clamp;
                _tex.filterMode = FilterMode.Bilinear;
            }
            float inner = Mathf.Clamp(1f - _texWidth, 0.15f, 0.85f);
            Color32[] px = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float u = (x + 0.5f) / size * 2f - 1f, v = (y + 0.5f) / size * 2f - 1f;
                    // a squarer ellipse than a circle, so the corners and the sides glow alike
                    float r = Mathf.Pow(Mathf.Pow(Mathf.Abs(u), 3f) + Mathf.Pow(Mathf.Abs(v), 3f), 1f / 3f);
                    float a = Mathf.Clamp01((r - inner) / (1.05f - inner));
                    a = a * a * (3f - 2f * a);
                    px[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(a * 255f));
                }
            _tex.SetPixels32(px);
            _tex.Apply(false);
            if (_sprite == null) _sprite = Sprite.Create(_tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
            if (_image != null) _image.sprite = _sprite;
        }

        // ------------------------------------------------------------------
        // the heartbeat sound: two low thumps, made in code
        // ------------------------------------------------------------------
        private void PlayBeat(float hp)
        {
            if (_audio == null)
            {
                _audio = gameObject.AddComponent<AudioSource>();
                _audio.playOnAwake = false;
                _audio.spatialBlend = 0f;
                AudioMixerGroup group = SfxGroup();
                if (group != null) _audio.outputAudioMixerGroup = group;
            }
            if (_beat == null) _beat = MakeBeat();
            _audio.pitch = Mathf.Lerp(1.08f, 0.96f, Mathf.Clamp01(hp / Mathf.Max(0.01f, _soundThreshold.Value / 100f)));
            _audio.PlayOneShot(_beat, _soundVolume.Value);
            if (!_soundLogged)
            {
                _soundLogged = true;
                Logger.LogInfo("Heartbeat sound playing (mixer group: " + (_audio.outputAudioMixerGroup != null ? _audio.outputAudioMixerGroup.name : "none") + ").");
            }
        }

        private static AudioMixerGroup SfxGroup()
        {
            AudioMan am = AudioMan.instance;
            if (am == null) return null;
            if (am.m_masterMixer != null)
            {
                AudioMixerGroup[] g = am.m_masterMixer.FindMatchingGroups("SFX");
                if (g != null && g.Length > 0) return g[0];
            }
            return am.m_guiMixer;
        }

        private static AudioClip MakeBeat()
        {
            const int rate = 44100;
            int n = (int)(rate * 0.55f);
            float[] s = new float[n];
            Thump(s, rate, 0f, 1f);
            Thump(s, rate, 0.2f, 0.65f);
            // keep it clear of clipping
            float peak = 0f;
            for (int i = 0; i < n; i++) peak = Mathf.Max(peak, Mathf.Abs(s[i]));
            if (peak > 0.95f) for (int i = 0; i < n; i++) s[i] *= 0.95f / peak;
            AudioClip clip = AudioClip.Create("HealthPulseBeat", n, 1, rate, false);
            clip.SetData(s, 0);
            return clip;
        }

        // A thump falling in pitch from 110 to 60 Hz with its overtones (so it is heard on
        // headphones and small speakers too, which hardly play below 60 Hz) and a short soft
        // knock at the start; a fast attack and a quick decay.
        private static void Thump(float[] s, int rate, float at, float gain)
        {
            int start = (int)(at * rate);
            int len = (int)(0.25f * rate);
            double ph = 0;
            System.Random rnd = new System.Random(7);
            float lp = 0f;
            for (int i = 0; i < len && start + i < s.Length; i++)
            {
                float t = (float)i / rate;
                float f = Mathf.Lerp(110f, 60f, Mathf.Clamp01(t / 0.09f));
                ph += 2 * Math.PI * f / rate;
                float env = Mathf.Clamp01(t / 0.004f) * Mathf.Exp(-t / 0.07f);
                float tone = (float)(Math.Sin(ph) + 0.5 * Math.Sin(2 * ph) + 0.22 * Math.Sin(3 * ph));
                // the knock: low-passed noise over the first 15 ms
                lp += ((float)rnd.NextDouble() * 2f - 1f - lp) * 0.08f;
                float knock = lp * 2.5f * Mathf.Exp(-t / 0.012f);
                s[start + i] += (tone * env + knock) * gain * 0.55f;
            }
        }
    }
}
