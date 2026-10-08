# HealthPulse

**English** · [Русский](README-RU.md)

A Valheim mod: when your health runs low, the edges of the screen glow red and **beat like a
heart** - stronger and faster the lower it gets - with a flash when a hit lands and, if you
like, a heartbeat you can hear. Client-side only: nothing is sent over the network and
nothing is patched; other players and the server need nothing.

Version history is in `CHANGELOG.md`.

## How it works

- Below `Threshold` (35 % of your maximum health) a red vignette appears at the edges. At
  the threshold it is a faint tint; near zero it is a dense red frame.
- It swells softly like a heartbeat, two beats ("lub-dub"), from 45 to 80 beats a minute as
  your health falls.
- A hit flashes the edges briefly; the lower your health, the stronger the flash. Well above
  the threshold there is no flash, the game's own one is enough.
- Everything eases in and out: heal up and it fades away instead of vanishing.
- Not shown when you are dead, sleeping, teleporting or loading.
- The vignette is its own nested canvas in the game's GUI canvas, sorted just below it and
  outside `hudroot`: the whole HUD is drawn over it, and HUD mods that pick up other mods'
  objects in `hudroot` (HudLayout) leave it alone. By default it stays when you hide the HUD
  (Ctrl+F3) - it is a warning; `HideWithHud` changes that.
- The heartbeat sound (experimental, off by default) is two low thumps made in code - with overtones, so
  headphones and small speakers play it too - with the first swell of each heartbeat below
  15 % health, through the game's sound volume.
- The texture and the sound are generated when the game starts; the mod ships no asset files.

## Compatibility

Tested with **Valheim 1.0.16** (network version 40), **BepInEx 5.4.23.5** (BepInExPack_Valheim 5.4.2351).

## Who needs it

| Who | What |
|---|---|
| A player who wants the effect | installs it |
| Other players, the host, a dedicated server | nothing |

## Known conflicts

- None known. The vignette sits under the GUI canvas of the game; a mod that draws its own
  full-screen layer under the HUD may share that spot.

## Bugs and feedback

https://github.com/tbsj1ga/HealthPulseValheim/issues

## Installation

Through r2modman / Thunderstore, or put `build/HealthPulse.dll` into

```
%AppData%\r2modmanPlus-local\Valheim\profiles\Valheim\BepInEx\plugins\HealthPulse\
```

(or `build.ps1 -Install`).

## Settings

`BepInEx/config/j1ga.healthpulse.cfg`, or in-game with ConfigurationManager (F1). Changes
apply at once.

| Section | Key | Default | What |
|---|---|---|---|
| 01 General | `Enabled` | `true` | the vignette on or off |
| 01 General | `Threshold` | `35` | health in % below which the edges start to glow |
| 01 General | `HideWithHud` | `false` | hide it too when the HUD is hidden (Ctrl+F3); off: it stays, as a warning |
| 02 Look | `MaxStrength` | `0.75` | how opaque the edges get at the lowest health (0.1..1) |
| 02 Look | `Color` | dark red | the colour, e.g. another one for colour blindness |
| 02 Look | `Width` | `0.45` | how far the glow reaches in from the edges (0.2..0.8) |
| 02 Look | `BeatsPerMinuteAtThreshold` | `45` | heart rate just below the threshold |
| 02 Look | `BeatsPerMinuteAtZero` | `80` | heart rate near zero health |
| 03 Hit flash | `Enabled` | `true` | flash the edges when a hit lands |
| 03 Hit flash | `Strength` | `1` | how strong the flash is (0.1..2) |
| 04 Heartbeat sound | `Enabled` | `false` | a heartbeat with the pulse at very low health |
| 04 Heartbeat sound | `Threshold` | `15` | health in % below which it is heard |
| 04 Heartbeat sound | `Volume` | `0.8` | its volume; the game's sound volume applies on top |

## Building

```
powershell -ExecutionPolicy Bypass -File .\build.ps1            # build and check references
powershell -ExecutionPolicy Bypass -File .\build.ps1 -Install   # ... and copy into plugins
powershell -ExecutionPolicy Bypass -File .\build.ps1 -Package   # ... and make the Thunderstore zip
```

The compiler is `csc.exe` from the .NET Framework (C# 5: no `out var`, `?.`, `$""`,
`nameof`); references come straight from the game folder and the r2modman profile's
`BepInEx\core`; the paths are at the top of `build.ps1` and `check-refs.ps1`. After the
build, `check-refs.ps1` checks every type and member reference against the game.

## Repository

Branch `main` on GitHub: https://github.com/tbsj1ga/HealthPulseValheim. Versioned: sources,
scripts, documentation, the Thunderstore template and `build\HealthPulse.dll`. Not
versioned: the BepInEx config, zip packages - see `.gitignore`.

## More mods by j1gA

| | Mod |
|---|---|
| [![LivingMap](https://raw.githubusercontent.com/tbsj1ga/LivingMapValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/LivingMap/) | **[LivingMap](https://thunderstore.io/c/valheim/p/j1gA/LivingMap/)** — Your buildings, roads and cleared forest on the map and the minimap — and a detailed map when you zoom in. |
| [![StationSpeed](https://raw.githubusercontent.com/tbsj1ga/StationSpeedValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/StationSpeed/) | **[StationSpeed](https://thunderstore.io/c/valheim/p/j1gA/StationSpeed/)** — Faster smelters, kilns, fermenters and crops — consistent even for players without the mod. |
| [![WeaponArts](https://raw.githubusercontent.com/tbsj1ga/WeaponArtsValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/WeaponArts/) | **[WeaponArts](https://thunderstore.io/c/valheim/p/j1gA/WeaponArts/)** — One key, one active ability per weapon: stagger, taunt, heals, berserk, crits. |
| [![ExtendedBosses](https://raw.githubusercontent.com/tbsj1ga/ExtendedBossesValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/ExtendedBosses/) | **[ExtendedBosses](https://thunderstore.io/c/valheim/p/j1gA/ExtendedBosses/)** — Raid-style boss fights: phases, adds, nests, shields, marks — built from vanilla parts. |
| [![HostOwner](https://raw.githubusercontent.com/tbsj1ga/HostOwnerValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/HostOwner/) | **[HostOwner](https://thunderstore.io/c/valheim/p/j1gA/HostOwner/)** — The host takes ownership of stations and bosses near it, so its mods work for everyone. |
| [![HudLayout](https://raw.githubusercontent.com/tbsj1ga/HudLayoutValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/HudLayout/) | **[HudLayout](https://thunderstore.io/c/valheim/p/j1gA/HudLayout/)** — Move, resize and restyle your HUD with the mouse: bars, food, hotbar, minimap, even other mods' HUD. |

## AI assistance

This mod was developed with the help of an AI assistant (Claude by Anthropic).
The code and the documentation were written together with it and checked
against the game's IL; the design decisions, in-game testing and releases are
the author's.
