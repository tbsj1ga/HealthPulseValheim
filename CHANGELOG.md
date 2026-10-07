# Changelog

**English** · [Русский](CHANGELOG-RU.md)

The version is set in one place — `HealthPulsePlugin.Version` in `src/HealthPulsePlugin.cs`.

## 0.1.0

- First release. A red vignette at the edges of the screen below `Threshold` (35 %) health,
  stronger as health falls, pulsing like a heartbeat (two soft swells, 45-80 a minute); a flash
  when a hit lands at low health; an optional heartbeat sound below 15 % (off by default).
- The vignette is a nested canvas just below the game's GUI canvas, outside `hudroot`: the
  HUD draws over it and HudLayout does not pick it up. It stays when the HUD is hidden unless
  `HideWithHud`. Texture and sound are made in code.
