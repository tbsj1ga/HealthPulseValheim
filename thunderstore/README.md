# HealthPulse

When your health runs low, **the edges of the screen glow red and beat like a heart** -
stronger and faster the lower it gets, like in many games.

- Starts below **35 %** health: a faint red tint at first, a dense red frame near zero.
- **Swells softly like a heartbeat** - "lub-dub", from 45 to 80 beats a minute as you weaken.
- **Flashes when you take a hit** at low health.
- **Heartbeat sound** below 15 % - experimental, off by default.
- Fades in and out smoothly; hidden while dead, sleeping, teleporting or loading.
- Drawn under the whole HUD, so bars, hotbar and windows stay clear; HudLayout doesn't
  pick it up. Stays when you hide the HUD (Ctrl+F3) -
  it's a warning - unless you turn on `HideWithHud`.

Everything is adjustable in `BepInEx/config/j1ga.healthpulse.cfg` or in-game with
ConfigurationManager (F1): threshold, strength, colour (e.g. for colour blindness), width,
heart rate, hit flash, sound and its volume.

**Client-side only.** Install it if you want the effect; other players, the host and the
server need nothing. No Harmony patches, nothing sent over the network.

## Compatibility

Tested with Valheim 1.0.16, BepInEx 5.4.23.5 (BepInExPack_Valheim 5.4.2351).

## Bugs and feedback

[GitHub issues](https://github.com/tbsj1ga/HealthPulseValheim/issues) - please include
`BepInEx/LogOutput.log`.

## More mods by j1gA

| | Mod |
|---|---|
| [![LivingMap](https://raw.githubusercontent.com/tbsj1ga/LivingMapValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/LivingMap/) | **[LivingMap](https://thunderstore.io/c/valheim/p/j1gA/LivingMap/)** — Your buildings, roads and cleared forest on the map and the minimap — and a detailed map when you zoom in. |
| [![StationSpeed](https://raw.githubusercontent.com/tbsj1ga/StationSpeedValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/StationSpeed/) | **[StationSpeed](https://thunderstore.io/c/valheim/p/j1gA/StationSpeed/)** — Faster smelters, kilns, fermenters and crops — consistent even for players without the mod. |
| [![WeaponArts](https://raw.githubusercontent.com/tbsj1ga/WeaponArtsValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/WeaponArts/) | **[WeaponArts](https://thunderstore.io/c/valheim/p/j1gA/WeaponArts/)** — One key, one active ability per weapon: stagger, taunt, heals, berserk, crits. |
| [![ExtendedBosses](https://raw.githubusercontent.com/tbsj1ga/ExtendedBossesValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/ExtendedBosses/) | **[ExtendedBosses](https://thunderstore.io/c/valheim/p/j1gA/ExtendedBosses/)** — Raid-style boss fights: phases, adds, nests, shields, marks — built from vanilla parts. |
| [![HostOwner](https://raw.githubusercontent.com/tbsj1ga/HostOwnerValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/HostOwner/) | **[HostOwner](https://thunderstore.io/c/valheim/p/j1gA/HostOwner/)** — The host takes ownership of stations and bosses near it, so its mods work for everyone. |
| [![HudLayout](https://raw.githubusercontent.com/tbsj1ga/HudLayoutValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/HudLayout/) | **[HudLayout](https://thunderstore.io/c/valheim/p/j1gA/HudLayout/)** — Move, resize and restyle your HUD with the mouse: bars, food, hotbar, minimap, even other mods' HUD. |
