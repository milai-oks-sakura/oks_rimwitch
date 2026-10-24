# RimWitch 开发计划 · 炼金系统最小闭环（0.1.0）

> - **状态**：设计稿，尚未实现
> - **版本**：0.1.0（未打 tag 前不变）
> - **目标**：用一个可玩通的小样，验证「采集/掉落 → 炼金台配方 → 使用效果 → 战斗增益」四条链路
> - **读者**：开发者（Def 编写、C# 实现、验收）
> - **相关文档**：[开发文档](develop.md) · [任务清单](../TODOLIST.md) · [项目主页（英文）](../README.md)

---

## 1. 背景与目标

当前的 0.1.0 已实现：**银器弱点**（Harmony 补丁）与 **三种药材**（乌鸦眼、金银花、长叶车前，含植物/产物 Def 与中文文本）。技术路线已经验证，但三种植物目前数值完全相同，且还没有验证「材料 → 成品 → 效果」的经济闭环。

0.1.0 不追求还原《巫师 3》的整棵炼金树，而是选一组最小内容，把下面四条链路一次跑通：

1. **采集/掉落**：材料能稳定获得；
2. **配方**：炼金台读得到配方、正确扣料、正确产出；
3. **使用效果**：成品能产生可见的 Hediff 或即时治疗；
4. **战斗增益**：剑油能按目标类型过滤后加成伤害。

这四条链路跑通之后，后续加海克娜煎药、黑血、食尸生物油，都只是「再填几条 Def」的体力活。

## 2. Demo 范围

| 类别 | 物品 | 选它的理由 |
| :--- | :--- | :--- |
| 药材 | **乌鸦眼**（已有） | 同时进煎药和剑油，验证一料多用 |
| 药材 | **金银花** | 巨魔煎药材料，验证野生植物采集 |
| 药材 | **长叶车前** | 白色拉法达材料，验证药剂链 |
| 怪物产物 | **巨魔突变诱发物** | 煎药核心材料，验证稀有掉落 |
| 怪物产物 | **孽鬼之心** | 白色拉法达材料，验证常见怪物掉落 |
| 怪物产物 | **怪物之肝** | 通用材料，Demo 中给野兽油，验证通用性 |
| 煎药 | **巨魔煎药** | 持续回血，效果直观、容易观测 |
| 药剂 | **白色拉法达** | 即时回血，验证一次性 UseEffect |
| 剑油 | **野兽油** | 对野兽增伤，验证按目标种族过滤 |

**本期不做**：新种族 / 新派系、突变诱发物分级、任务与事件、自定义 UI、完整配方树、数值精调、成就与兼容层。

> Demo 配方只保证系统闭环，材料数量按《巫师 3》简化，等后续版本再校准。

## 3. 数据设计

### 3.1 分类树

避免和原版食物、药物、材料混在一起，新建一个分类根：

```text
Oks_RimWitch_ThingCategory_Alchemy        (parent: Root)
├─ Oks_RimWitch_ThingCategory_Ingredient  # 药材
├─ Oks_RimWitch_ThingCategory_MonsterPart # 怪物产物
├─ Oks_RimWitch_ThingCategory_Decoction   # 煎药
├─ Oks_RimWitch_ThingCategory_Potion      # 药剂
└─ Oks_RimWitch_ThingCategory_Oil         # 剑油
```

- 乌鸦眼根从原版 `PlantMatter` 迁到 `Ingredient`，同时保留 `ingestible`（仅绝望时可吃）。
- 分类同时影响仓库/储存区筛选，迁移后需要玩家重新勾选储存区——在 changelog 里注明。
- 怪物产物统一 `tradeability = Normal`，煎药/药剂/剑油 `tradeability = None`（Demo 阶段不让商人刷出来）。

### 3.2 Def 清单

| defName | 类型 | 说明 |
| :--- | :--- | :--- |
| `Oks_RimWitch_Plant_Honeysuckle` | ThingDef (Plant) | 金银花，成熟收获根茎 |
| `Oks_RimWitch_Item_Honeysuckle` | ThingDef | 金银花材料 |
| `Oks_RimWitch_Plant_Ribleaf` | ThingDef (Plant) | 长叶车前（Ribleaf） |
| `Oks_RimWitch_Item_Ribleaf` | ThingDef | 长叶车前材料 |
| `Oks_RimWitch_Plant_CrowEye` | ThingDef (Plant) | 已有 |
| `Oks_RimWitch_Item_CrowEye` | ThingDef | 已有，改挂新分类 |
| `Oks_RimWitch_Item_TrollMutagen` | ThingDef | 巨魔突变诱发物 |
| `Oks_RimWitch_Item_NekkerHeart` | ThingDef | 孽鬼之心 |
| `Oks_RimWitch_Item_MonsterLiver` | ThingDef | 怪物之肝 |
| `Oks_RimWitch_Item_DwarvenSpirit` | ThingDef | 矮人烈酒（配方基底） |
| `Oks_RimWitch_Item_TrollDecoction` | ThingDef | 巨魔煎药 |
| `Oks_RimWitch_Item_WhiteRaffard` | ThingDef | 白色拉法达 |
| `Oks_RimWitch_Item_BeastOil` | ThingDef | 野兽油 |
| `Oks_RimWitch_Building_AlchemyTable` | ThingDef (Building) | 炼金台 |
| `Oks_RimWitch_Recipe_MakeTrollDecoction` | RecipeDef | 煎药配方 |
| `Oks_RimWitch_Recipe_MakeWhiteRaffard` | RecipeDef | 药剂配方 |
| `Oks_RimWitch_Recipe_MakeBeastOil` | RecipeDef | 剑油配方 |
| `Oks_RimWitch_Hediff_TrollDecoction` | HediffDef | 煎药持续回血 |
| `Oks_RimWitch_Hediff_Toxicity` | HediffDef | 毒性累积 |
| `Oks_RimWitch_Hediff_BeastOil` | HediffDef | 剑油 Buff 标记 |

**素材缺口**：3 种植物、3 种怪物产物、3 种成品、1 个建筑，共约 10 张图。先用占位图，美术最后替换。

### 3.3 配方

| 产物 | 材料 | 工作量（占位） | 产出 |
| :--- | :--- | ---: | ---: |
| 巨魔煎药 | 矮人烈酒 ×1 + 巨魔突变诱发物 ×1 + 乌鸦眼 ×1 + 金银花 ×1 | 1200 ticks | 1 |
| 白色拉法达 | 矮人烈酒 ×1 + 长叶车前 ×2 + 孽鬼之心 ×4 | 900 ticks | 1 |
| 野兽油 | 怪物之肝 ×1 + 乌鸦眼 ×1 | 600 ticks | 1 |

- 配方只挂在炼金台上（`recipeUsers`），不污染原版工作台。
- Demo 不设研究前置；是否用「炼金学」研究门禁留到后续版本。
- 白色拉法达的 4 个心偏贵，若测试时节奏过慢，先降到 ×2 再回调。

### 3.4 效果设计

| 成品 | 目标行为 | 实现方式 |
| :--- | :--- | :--- |
| 巨魔煎药 | 持续 1800 秒，每 60 ticks 回 1 HP；毒性 +50 | `HediffDef` + 自定义 `HediffComp_HealOverTime` + `HediffComp_Disappears` |
| 白色拉法达 | 使用后立即恢复 10 HP；毒性 +25 | 自定义 `IngestibleOutcomeDoer_Heal`（备选：瞬时 Hediff） |
| 野兽油 | 涂抹后 8 小时（20000 ticks）内，对 `race.Animal` 目标伤害 +10% | `HediffDef` 只作标记与计时；实际加成在伤害补丁里判定 |
| 毒性 | 每次使用累加，按天缓慢衰减 | 独立 `HediffDef`，`severityPerDay = -10`，阶段阈值 50 / 100 |

**为什么野兽油不放 `statOffsets`**：`MeleeDamageFactor` 是全局的，无法只对野兽生效，玩家在界面里会看到「对所有人 +10%」的假数值。所以 Hediff 只负责「有没有涂油、还剩多久」，伤害过滤交给 Harmony，在 `Pawn.PreApplyDamage` 里看 `dinfo.Instigator` 的 Hediff 和受害者 `race.Animal`。

**为什么煎药参数写进 XML**：`healAmount`、`intervalTicks` 作为 `HediffCompProperties` 字段暴露，调平衡时只改 Def、不用重新编译。

> [!WARNING]
> 按 1 HP / 60 ticks 实现，1800 秒总共能回 500 HP，等于战斗中瞬间满血。这属于 Demo 观感优先的设定，验收通过后在 M5 把总量压到 30–50 HP（如 1 HP / 600 ticks），并在 Plan 里记录数值变更。

### 3.5 采集与掉落

**药材（野外生成）**：用 XML Patch 把三种植物加进 `TemperateForest`、`BorealForest`、`TropicalRainforest` 的 `wildPlants`，并调低 `commonality`，避免铺满地图。先只做「野生可采」，不开放种植（种植配方留到后续版本）。

**怪物产物（屠宰掉落）**：Demo 阶段不建新种族，用 XML Patch 给现有动物挂 `butcherProducts`：

| 动物 | 临时产物 | 说明 |
| :--- | :--- | :--- |
| `Muffalo` | 巨魔突变诱发物 ×1 | 常见的「稀有」来源，仅作占位 |
| `Boomrat` | 孽鬼之心 ×1 | 小体型、易刷 |
| `WildBoar` | 怪物之肝 ×1 | 通用产物来源 |

- 用 `PatchOperationAdd` 追加到 `butcherProducts`，不覆盖原版产出，零代码。
- `butcherProducts` 没有概率字段，所以 Demo 是必掉；概率掉落等后续版本换成自定义 `PawnKindDef`（巨魔/孽鬼）时再处理。
- 屠宰需要尸体与屠宰台，比「击杀即掉」多一步，但更符合 RimWorld 的经济习惯，也方便测试。

**炼金台**：继承 `WorkbenchBase`，工作类型 `Crafting`，给一个交互格；`recipeUsers` 指向三条配方；`researchPrerequisites` 留空。

## 4. 技术改动清单

| 文件 | 改动 |
| :--- | :--- |
| `1.7/Defs/ThingCategoryDefs/RimWitch_Categories.xml` | 新增分类树 |
| `1.7/Defs/ThingDefs_Plants/` | 金银花、长叶车前 |
| `1.7/Defs/ThingDefs_Items/` | 3 药材 + 3 怪物产物 + 矮人烈酒 + 3 成品 |
| `1.7/Defs/ThingDefs_Buildings/RimWitch_AlchemyTable.xml` | 炼金台 |
| `1.7/Defs/RecipeDefs/RimWitch_AlchemyRecipes.xml` | 三条配方 |
| `1.7/Defs/HediffDefs/RimWitch_AlchemyHediffs.xml` | 3 个 Hediff |
| `1.7/Patches/RimWitch_WildPlants.xml` | 三种植物进生态 |
| `1.7/Patches/RimWitch_ButcherProducts.xml` | 三种动物加屠宰产物 |
| `Textures/...` | 占位图 |
| `1.7/Sources/RimWitchCore/HediffComp/HediffComp_HealOverTime.cs` | 持续回血组件（含 Properties） |
| `1.7/Sources/RimWitchCore/Ingestible/IngestibleOutcomeDoer_Heal.cs` | 即时治疗 |
| `1.7/Sources/RimWitchCore/Patch_PreApplyDamage_BeastOil.cs` | 剑油增伤（按目标过滤） |
| `1.7/Sources/RimWitchCore/RimWitchCore.csproj` | 加新文件；建议同期迁移到 `net10.0` |

补丁执行顺序：两个 `PreApplyDamage` Prefix 都用 `[HarmonyPriority]` 显式排序，确保「银器减伤」先算、「剑油增伤」后算；乘法可交换，但排序能让日志和调试结果稳定。

## 5. 里程碑与验收

### M0 · 分类与材料（S）

- [x] 三种药材的植物与产物 Def、中文文本（数值待差异化，见 TODOLIST）
- [ ] 分类树；3 种怪物产物、矮人烈酒、3 种成品
- [ ] 乌鸦眼等三种药材迁入新分类，配方可引用
- [ ] 占位贴图（怪物产物、成品、建筑）
- **验收**：开发者模式下能刷出全部材料，分类在储存区筛选里可见

### M1 · 采集与掉落（M）

- [ ] 三种植物进生态，能野外采集
- [ ] 三种动物屠宰后产出对应材料
- **验收**：清单第 1、2 条

### M2 · 炼金台与配方（M）

- [ ] 炼金台可建造、可交互
- [ ] 三条配方可见，材料不足时灰显
- [ ] 扣料、耗时、产出正确
- **验收**：清单第 3 条

### M3 · 煎药与药剂（M）

- [ ] `HediffComp_HealOverTime` + 煎药 Hediff（1800s，回血可调）
- [ ] 毒性 Hediff 累积与衰减
- [ ] 白色拉法达即时治疗
- **验收**：清单第 4、5 条

### M4 · 野兽油（M）

- [ ] 涂抹产生 8 小时 Buff
- [ ] 对 `race.Animal` 目标 +10%，对其他目标不加成
- **验收**：清单第 6 条

### M5 · 收尾（S）

- [ ] 存档/读档不丢物品、配方、Hediff
- [ ] 数值校准（煎药回血总量、毒性衰减、配方成本）
- [ ] 中英文文案、贴图替换
- [ ] 更新 `README` / `docs/develop.md`
- **验收**：清单第 7 条

## 6. 验收清单

1. 地图上能采集乌鸦眼、金银花、长叶车前。
2. 击杀并屠宰测试动物能获得巨魔突变诱发物、孽鬼之心、怪物之肝。
3. 炼金台能看到三条配方，材料不足时不可制作。
4. 制作巨魔煎药并使用后出现回血 Hediff，受伤后能观测到回血。
5. 白色拉法达使用后立即回血。
6. 野兽油使用后出现 Buff，攻击动物时伤害提高；攻击非动物时不提高。
7. 保存读档后，物品、配方、Hediff 不丢失。

> 第 6 条如果先做成全局加成，必须在本文件标注为「待迭代」，不能当作已通过。

## 7. 目录与命名规范

- defName 统一前缀 `Oks_RimWitch_`，按类型分段：`_Plant_`、`_Item_`、`_Building_`、`_Recipe_`、`_Hediff_`、`_ThingCategory_`。
- 贴图路径与 Def 一一对应：`RimWitch_Plant/<Name>/...`、`RimWitch_Items/<Name>/...`。
- XML 一个文件只放同类内容；配方与 Hediff 分开，方便后续按版本替换。
- 新代码文件放进 `HediffComp/`、`Ingestible/`、`Patches/` 子目录，并同步修改 `csproj` 的 `<Compile Include>`（迁移到 SDK 风格后可省去）。

## 8. 风险与开放问题

| 问题 | 现状 / 建议 |
| :--- | :--- |
| 矮人烈酒用新物品还是原版 `Beer` 占位 | 建议新物品：配方更清晰；若求快可先引用 `Beer`，M5 再替换 |
| 剑油对昆虫、机械、人形是否生效 | Demo 只认 `race.Animal`；昆虫/机械按「不生效」处理，后续版本再按 `MonsterModExtension.monsterType` 扩展 |
| 毒性是否自然衰减 | 建议 `-10 severity/day`，避免永久惩罚；具体数值 M5 校准 |
| 炼金台是否需要研究 | Demo 不锁；后续版本挂「炼金学」研究 |
| 1.7 运行时为 .NET 10，当前 csproj 仍是 net472 | 工程已改为 SDK 风格；目标框架迁移见 [TODOLIST](../TODOLIST.md) §1，不迁移可能加载失败 |
| 与 Combat Extended 等战斗大修冲突 | 未测试；两个补丁都改 `PreApplyDamage`，需实测并在文档标注 |
| 贴图与本地化缺口 | 占位图 + 英文文案先行，中文语言包在 M5 补齐 |

## 9. 后续版本

- 海克娜煎药（行动消耗体力时回复）、黑血（吸血鬼与食尸生物攻击你时反伤）、食尸生物油等复杂效果。
- 自定义 `PawnKindDef`：巨魔、孽鬼，替换 Demo 的临时屠宰掉落。
- 突变诱发物分级（普通/稀有/大师），影响煎药时长与毒性。
- 炼金配方树与研究门禁、商人库存、任务奖励。
