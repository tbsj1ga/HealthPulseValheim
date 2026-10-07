# HealthPulse

[English](README.md) · **Русский**

Мод для Valheim: когда здоровья мало, края экрана светятся красным и **бьются, как сердце**, —
чем меньше здоровья, тем сильнее и чаще; при попадании — вспышка, а по желанию — слышимое
сердцебиение. Только на клиенте: по сети ничего не передаётся, ничего не патчится; другим
игрокам и серверу ничего не нужно.

История версий — в `CHANGELOG-RU.md`.

## Как это работает

- Ниже `Threshold` (35 % от максимума здоровья) по краям появляется красная виньетка. У
  порога — лёгкий оттенок, у нуля — плотная красная рамка.
- Она мягко нарастает как сердцебиение, двумя ударами («тук-тук»), от 45 до 80 ударов в
  минуту по мере падения здоровья.
- Попадание коротко вспыхивает по краям; чем меньше здоровья, тем ярче. Далеко выше порога
  вспышки нет — хватает игровой.
- Всё плавно появляется и гаснет: подлечились — рамка тает, а не пропадает рывком.
- Не показывается, когда вы мертвы, спите, телепортируетесь или идёт загрузка.
- Виньетка — свой вложенный холст в холсте интерфейса игры, отсортированный сразу под ним и
  вне `hudroot`: весь HUD рисуется поверх неё, а моды HUD, подхватывающие объекты других модов
  в `hudroot` (HudLayout), её не трогают. По умолчанию она остаётся, когда HUD скрыт (Ctrl+F3),
  — это предупреждение; `HideWithHud` меняет это.
- Звук сердца (по умолчанию выключен) — два низких удара, созданных в коде, с обертонами,
  чтобы их воспроизводили и наушники, и небольшие колонки, — на первом ударе каждого
  сердцебиения ниже 15 % здоровья, с громкостью звуков игры.
- Текстура и звук создаются при запуске игры; файлов ресурсов у мода нет.

## Совместимость

Проверено на **Valheim 1.0.16** (сетевая версия 40), **BepInEx 5.4.23.5** (BepInExPack_Valheim 5.4.2351).

## Кому ставить

| Кто | Что |
|---|---|
| Игрок, которому нужен эффект | ставит мод |
| Другие игроки, хост, выделенный сервер | ничего |

## Известные конфликты

- Неизвестны. Виньетка лежит под холстом интерфейса игры; мод, рисующий свой полноэкранный
  слой под HUD, может делить с ней это место.

## Ошибки и отзывы

https://github.com/tbsj1ga/HealthPulseValheim/issues

## Установка

Через r2modman / Thunderstore, или положите `build/HealthPulse.dll` в

```
%AppData%\r2modmanPlus-local\Valheim\profiles\Valheim\BepInEx\plugins\HealthPulse\
```

(или `build.ps1 -Install`).

## Настройки

`BepInEx/config/j1ga.healthpulse.cfg` или в игре через ConfigurationManager (F1). Изменения
применяются сразу.

| Раздел | Ключ | По умолчанию | Что |
|---|---|---|---|
| 01 General | `Enabled` | `true` | виньетка вкл/выкл |
| 01 General | `Threshold` | `35` | здоровье в %, ниже которого края начинают светиться |
| 01 General | `HideWithHud` | `false` | прятать и её, когда HUD скрыт (Ctrl+F3); выкл.: остаётся как предупреждение |
| 02 Look | `MaxStrength` | `0.75` | насколько плотными становятся края при минимуме здоровья (0.1..1) |
| 02 Look | `Color` | тёмно-красный | цвет, например другой для дальтоников |
| 02 Look | `Width` | `0.45` | как далеко от краёв доходит свечение (0.2..0.8) |
| 02 Look | `BeatsPerMinuteAtThreshold` | `45` | пульс чуть ниже порога |
| 02 Look | `BeatsPerMinuteAtZero` | `80` | пульс у нуля здоровья |
| 03 Hit flash | `Enabled` | `true` | вспышка краёв при попадании |
| 03 Hit flash | `Strength` | `1` | сила вспышки (0.1..2) |
| 04 Heartbeat sound | `Enabled` | `false` | сердцебиение в такт пульсу при очень низком здоровье |
| 04 Heartbeat sound | `Threshold` | `15` | здоровье в %, ниже которого оно слышно |
| 04 Heartbeat sound | `Volume` | `0.8` | громкость; поверх действует громкость звуков игры |

## Сборка

```
powershell -ExecutionPolicy Bypass -File .\build.ps1            # сборка и проверка ссылок
powershell -ExecutionPolicy Bypass -File .\build.ps1 -Install   # ... и копирование в plugins
powershell -ExecutionPolicy Bypass -File .\build.ps1 -Package   # ... и zip для Thunderstore
```

Компилятор — `csc.exe` из .NET Framework (C# 5: без `out var`, `?.`, `$""`, `nameof`);
ссылки берутся прямо из папки игры и `BepInEx\core` профиля r2modman; пути — в начале
`build.ps1` и `check-refs.ps1`. После сборки `check-refs.ps1` сверяет каждую ссылку на тип и
член с игрой.

## Репозиторий

Ветка `main` на GitHub: https://github.com/tbsj1ga/HealthPulseValheim. В репозитории:
исходники, скрипты, документация, шаблон Thunderstore и `build\HealthPulse.dll`. Не
версионируются: конфиг BepInEx, zip-пакеты — см. `.gitignore`.

## Другие моды j1gA

| | Мод |
|---|---|
| [![LivingMap](https://raw.githubusercontent.com/tbsj1ga/LivingMapValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/LivingMap/) | **[LivingMap](https://thunderstore.io/c/valheim/p/j1gA/LivingMap/)** — Постройки, дороги и вырубки на карте и мини-карте, а при приближении — детальная карта. |
| [![StationSpeed](https://raw.githubusercontent.com/tbsj1ga/StationSpeedValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/StationSpeed/) | **[StationSpeed](https://thunderstore.io/c/valheim/p/j1gA/StationSpeed/)** — Ускорение плавилен, печей, бочек и грядок — согласованно даже для игроков без мода. |
| [![WeaponArts](https://raw.githubusercontent.com/tbsj1ga/WeaponArtsValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/WeaponArts/) | **[WeaponArts](https://thunderstore.io/c/valheim/p/j1gA/WeaponArts/)** — Одна клавиша — своя активная способность у каждого оружия: стаггер, таунт, хилы, берсерк, криты. |
| [![ExtendedBosses](https://raw.githubusercontent.com/tbsj1ga/ExtendedBossesValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/ExtendedBosses/) | **[ExtendedBosses](https://thunderstore.io/c/valheim/p/j1gA/ExtendedBosses/)** — Боссы как рейды: фазы, адды, гнёзда, щиты, метки — из ванильных частей. |
| [![HostOwner](https://raw.githubusercontent.com/tbsj1ga/HostOwnerValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/HostOwner/) | **[HostOwner](https://thunderstore.io/c/valheim/p/j1gA/HostOwner/)** — Хост забирает владение станциями и боссами рядом, чтобы его моды работали для всех. |
| [![HudLayout](https://raw.githubusercontent.com/tbsj1ga/HudLayoutValheim/main/docs/media/icon-128.png)](https://thunderstore.io/c/valheim/p/j1gA/HudLayout/) | **[HudLayout](https://thunderstore.io/c/valheim/p/j1gA/HudLayout/)** — Перемещайте, масштабируйте и меняйте вид HUD мышью: полосы, еда, панель предметов, миникарта, даже HUD других модов. |

## Помощь ИИ

Мод разработан с помощью ИИ-ассистента (Claude от Anthropic). Код и
документация написаны вместе с ним и сверены с IL игры; решения по дизайну,
проверка в игре и релизы — за автором.
