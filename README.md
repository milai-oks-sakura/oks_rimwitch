<div align="center">

**English** · [简体中文](README.zh-CN.md)

# 🧙 RimWitch

**Monsters, silver swords and witch's herbs for RimWorld 1.7**

[![RimWorld](https://img.shields.io/badge/RimWorld-1.7-2b6cb0?logo=steam&logoColor=white)](https://store.steampowered.com/app/294100/RimWorld/)
[![Harmony](https://img.shields.io/badge/Requires-Harmony-e07b39)](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
[![License](https://img.shields.io/badge/License-GPL--3.0-3da639)](LICENSE)

<img src="About/Preview.png" alt="RimWitch preview" width="720">

</div>

> 🇨🇳 中文玩家 / Chinese readers: [点此阅读简体中文版 · Read this in Simplified Chinese](README.zh-CN.md).

RimWitch is a small, early-prototype content mod with a witcher flavour: some creatures shrug off ordinary steel and only fear silver, and a humble shrub hides something useful under the dirt.

> [!NOTE]
> RimWitch is at **0.1**. It currently ships **one crop** and the **silver-weakness framework**. It does **not** add monsters or alchemy yet — those are the next milestone, see [What's next](#whats-next).

## Monsters fear only silver

Any creature tagged as a monster takes only **10%** damage from attacks that are not silver:

- **Silver** — a weapon made of silver, or one flagged as silver by a mod — deals full damage.
- **Everything else** deals a tenth: steel, clubs, guns, teeth and claws.
- **Traps, explosions and environmental damage** are not reduced, because nobody is swinging them.

The rule is data-driven: any mod can flag its own creatures and weapons, and they join the system automatically.

> RimWitch itself doesn't include monsters yet. The framework is here so that upcoming creatures — and other mods — can use it.

## The crow's eye

A hardy shrub with dark, cypress-like foliage and a black-chestnut rootstock buried underground. Unremarkable above ground; valuable below.

| Attribute                 | Value                      |
| :------------------------ | :------------------------- |
| **Grows in**              | 5 days                     |
| **Harvest**               | 8 crow eye roots per plant |
| **Sowing skill**          | 3                          |
| **Fertility sensitivity** | 0.7                        |
| **Plantable in**          | Soil, hydroponics          |
| **Market value**          | 0.3 per root               |
| **Shelf life**            | 30 days before rotting     |
| **Edible?**               | Only in desperation        |

The root keeps a colonist lucid through pain and steady under fear — at the price of a lingering paranoia. It is the first ingredient of something bigger: a planned alchemy line of decoctions, potions and blade oils.

## Installation

1. Subscribe to **[Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)** — required.
2. Drop the `RimWitch` folder into:

   ```text
   …\steamapps\common\RimWorld\Mods\
   ```

3. Enable `[oks]rimwitch` in **Options → Mods**, with Harmony loaded before it.
4. **Restart the game.** Harmony patches are applied at startup; reloading defs is not enough.

> [!IMPORTANT]
> The GitHub source does not contain a compiled DLL. Cloned straight from the repository, only the crow's eye content loads and the silver rule stays inactive. If you are building from source, see [docs/develop.md](docs/develop.md).

## Compatibility

- **RimWorld 1.7** only.
- **Harmony** is required.
- Content-only mods are safe to combine.
- Combat overhauls that also rewrite damage handling may behave differently; please report what you see.

## What's next

The next milestone is a small, complete alchemy loop:

- **Herbs** — crow's eye, honeysuckle, plantain
- **Monster parts** — troll mutagen, nekker heart, monster liver
- **Troll decoction** — slow, steady healing
- **White Raffard's decoction** — instant healing
- **Beast oil** — extra damage against animals

The design, recipes and acceptance checks live in [docs/Plan.md](docs/Plan.md) (Chinese). This is a work in progress and not part of the current build.

## Credits

- [Harmony](https://github.com/pardeike/Harmony) by Andreas Pardeike — the patching backbone.
- [RimWorld](https://rimworldgame.com/) by Ludeon Studios.
- Preview artwork belongs to its original author; it will be replaced if requested.

## License

GPL-3.0 — see [LICENSE](LICENSE).

## For modders

Developer docs, build steps and the extension API: [docs/develop.md](docs/develop.md). Planned work: [docs/Plan.md](docs/Plan.md).
