# TODOLIST · RimWitch 炼金闭环（0.1.0）

> 依据 [docs/Plan.md](docs/Plan.md) 拆解，勾选状态按仓库当前内容维护。
> 图例：`[x]` 已完成 · `[ ]` 待开发 · 括号内标注「部分」表示已有基础但未达标。

## 0. 现状快照

| 模块                       | 状态              | 说明                                                       |
| :------------------------- | :---------------- | :--------------------------------------------------------- |
| 版本                       | ✅ 0.1.0           | `About.xml` 的 `modVersion`；打 git tag 前不变             |
| 银器弱点补丁               | ✅                 | `Pawn.PreApplyDamage`，非银伤害 ×0.1                       |
| 三种药材（植物 + 产物）    | ✅ 内容 / ⚠️ 数值   | 乌鸦眼、金银花、长叶车前；数值完全相同，见 §8              |
| 中文 DefInjected           | ✅ 部分            | 6 个 Def 有译文；新内容待补                                |
| 编译产物                   | ✅（net472 Release） | SDK 风格构建，Release 输出已统一到 `1.7/Assemblies/`       |
| 分类树                     | ❌                 | 尚无 `ThingCategoryDef`                                    |
| 怪物产物 / 矮人烈酒 / 成品 | ❌                 | 3 + 1 + 3 个 Def 待建                                      |
| 炼金台 / 配方              | ❌                 | 目录已建，XML 未写                                         |
| 煎药 / 药剂 / 剑油效果     | ❌                 | Hediff、Comp、补丁均未实现                                 |
| 采集与掉落                 | ❌                 | 植物未进生态，动物无屠宰产物                               |

## 1. 工程与仓库（阻塞项，先做）

- [ ] 将 `RimWitchCore.csproj` 从 `net472` 迁移到 `net10.0`
  - [x] 移除旧式 `System.*` 引用，`0Harmony` 改指 1.7
  - [x] 保留 `Assembly-CSharp` 引用（游戏根目录）
  - [x] 改为 SDK 风格工程（SDK 风格，net472）
  - [x] Release 输出也指向 `1.7/Assemblies/`（Debug/Release 同路径，`dotnet build -c Release` 已验证）
  - [ ] 进游戏确认补丁加载（`Player.log` 无异常）
- [x] 修复 `About/About.xml`
  - [x] `<autho>` → `<author>`
  - [x] `loadAfter` 去掉嵌套 `<li>`，改为两项并列
  - [x] 补 `<modVersion>0.1.0</modVersion>`
  - [x] 重写 `description`（概述 + 现状 + 开发中内容）
- [x] 构建与产物策略
  - [x] 决定是否继续提交 `.pdb`（建议忽略）
  - [x] 修正 `.gitignore` 行内注释（`.vscode/ # 注释` 导致规则失效）
- [x] 提交 Ribleaf 贴图重命名（`Ribleaf.png` → `Ribleaf_Fruit.png`）
- [x] 核实 `Textures/updream/*.md`：文档不应随 Mod 发布，移到 `docs/` 或删除
- [x] 版本统一为 `0.1.0`（About.xml、README、Plan、TODOLIST、develop）
- [x] 同步 `AssemblyInfo.cs` 到 `0.1.0.0` 并重建 DLL
- [ ] 引入 Doxygen：`Doxyfile` + 生成 `docs/api/`（生成物不入库）

## 2. M0 · 分类与材料

### 分类树

- [ ] 新建 `1.7/Defs/ThingCategoryDefs/RimWitch_Categories.xml`
  - [ ] `Oks_RimWitch_ThingCategory_Alchemy`（parent: Root）
  - [ ] `…_Ingredient`（药材）
  - [ ] `…_MonsterPart`（怪物产物）
  - [ ] `…_Decoction`（煎药）
  - [ ] `…_Potion`（药剂）
  - [ ] `…_Oil`（剑油）
- [ ] 三种药材产物从 `PlantMatter` 迁到 `Ingredient`
- [ ] 存档兼容：迁移会改变储存区/仓库筛选，写进 CHANGELOG

### 怪物产物（3 个）

- [ ] `Oks_RimWitch_Item_TrollMutagen`（巨魔突变诱发物）
- [ ] `Oks_RimWitch_Item_NekkerHeart`（孽鬼之心）
- [ ] `Oks_RimWitch_Item_MonsterLiver`（怪物之肝）
- [ ] 统一属性：重量、堆叠、是否腐烂、可交易（数值见 §8）

### 基底与成品（4 个）

- [ ] `Oks_RimWitch_Item_DwarvenSpirit`（矮人烈酒）——或复用原版 `Beer`，待决策
- [ ] `Oks_RimWitch_Item_TrollDecoction`（巨魔煎药）
- [ ] `Oks_RimWitch_Item_WhiteRaffard`（白色拉法达）
- [ ] `Oks_RimWitch_Item_BeastOil`（野兽油）
- [ ] 成品的 `ingestible` / `comps` 接线（见 §5、§6）

### 贴图占位

- [ ] 3 张怪物产物图
- [ ] 3 张成品图
- [ ] 1 张炼金台图

## 3. M1 · 采集与掉落

### 野生植物

- [ ] 新建 `1.7/Patches/RimWitch_WildPlants.xml`
  - [ ] `TemperateForest` 加入三种植物
  - [ ] `BorealForest` 加入耐寒组合（金银花优先）
  - [ ] `TropicalRainforest` 加入车前
  - [ ] 每种植物 `commonality` 按 §8 目标设置
- [ ] 验证野外可选中、可收获、产物正确、不铺满地图

### 屠宰掉落

- [ ] 新建 `1.7/Patches/RimWitch_ButcherProducts.xml`
  - [ ] `Muffalo` → 巨魔突变诱发物 ×1
  - [ ] `Boomrat` → 孽鬼之心 ×1
  - [ ] `WildBoar` → 怪物之肝 ×1
- [ ] 验证不与原版皮革/肉产出冲突

### 后续再替换

- [ ] 自定义 `PawnKindDef`：巨魔、孽鬼
- [ ] 概率掉落（`CompButcherProducts` 或补丁）

## 4. M2 · 炼金台与配方

- [ ] `Oks_RimWitch_Building_AlchemyTable`（`WorkbenchBase`、`Crafting`、交互格、可建造）
- [ ] 三条 `RecipeDef`
  - [ ] `MakeTrollDecoction`：烈酒 1 + 突变诱发物 1 + 乌鸦眼 1 + 金银花 1，1200 ticks
  - [ ] `MakeWhiteRaffard`：烈酒 1 + 长叶车前 2 + 孽鬼之心 4，900 ticks
  - [ ] `MakeBeastOil`：怪物之肝 1 + 乌鸦眼 1，600 ticks
- [ ] `recipeUsers` 绑定炼金台
- [ ] 材料不足灰显、扣料/耗时/产出正确
- [ ] 工作台建造成本与工时（待平衡）
- [ ] 后续：研究前置「炼金学」

## 5. M3 · 煎药与药剂

### 持续回血

- [ ] `HediffComp_HealOverTime` + `HediffCompProperties_HealOverTime`
  - [ ] `healAmount`、`intervalTicks` 暴露到 XML
  - [ ] 用 `parent.ageTicks` 推导，避免额外存档字段（或补 `ExposeData`）
- [ ] `Oks_RimWitch_Hediff_TrollDecoction`
  - [ ] 持续 1800 秒（30000 ticks）
  - [ ] 初始参数 1 HP / 60 ticks（待平衡，见 §8）

### 毒性

- [ ] `Oks_RimWitch_Hediff_Toxicity`
  - [ ] 煎药 +50、药剂 +25
  - [ ] 每日衰减 -10
  - [ ] 阶段阈值 50 / 100 与对应文案（可选惩罚）

### 即时治疗

- [ ] `IngestibleOutcomeDoer_Heal`
  - [ ] 依次治疗 `Hediff_Injury`，总计 10 HP
- [ ] `Oks_RimWitch_Item_WhiteRaffard` 接线（使用 → 治疗 + 毒性）
- [ ] 使用限制：倒地/战斗中是否可用（待决策）
- [ ] Hediff 图标 + 中英文文案

## 6. M4 · 野兽油

- [ ] `Oks_RimWitch_Hediff_BeastOil`（8 小时 / 20000 ticks）
- [ ] `Patch_PreApplyDamage_BeastOil`
  - [ ] 攻击者持有 BeastOil Hediff
  - [ ] 目标 `def.race.Animal`
  - [ ] 伤害 ×1.1
- [ ] 用 `[HarmonyPriority]` 与银器补丁排序
- [ ] 验证：动物 +10%，人形/机械/昆虫不加成
- [ ] 后续：按 `MonsterType` 扩展油种

## 7. M5 · 收尾

- [ ] 存档/读档回归（物品、配方、Hediff）
- [ ] 平衡校准（§8 全部完成）
- [ ] 新内容中英文案（Languages 扩展）
- [ ] 贴图替换占位图
- [x] 更新 `README.md` / `README.zh-CN.md` / `docs/develop.md`
  - [x] `develop.md` 改为架构指南，细节交给代码注释 / Doxygen
  - [x] README 版本与现状同步（0.1.0、三种药材）
  - [ ] README 功能列表加入炼金内容（仅上线后）
- [ ] 兼容性测试（Combat Extended 等）
- [ ] 发布 0.1.0 tag（首个 tag）

## 8. 数值平衡（本轮重点）

> **已知现状**：三种植物除**命名与产物**不同外，所有涉及平衡的数值完全一致（同一份模板复制），需要在本轮做差异化。

### 8.1 当前共用数值

| 参数                   | 当前值（三植物相同） | 位置 |
| :--------------------- | :------------------- | :--- |
| `MaxHitPoints`         | 85                   | 植物 |
| `Nutrition`            | 0.2                  | 植物 |
| `growDays`             | 5                    | 植物 |
| `fertilitySensitivity` | 0.7                  | 植物 |
| `harvestYield`         | 8                    | 植物 |
| `sowMinSkill`          | 3                    | 植物 |
| `topWindExposure`      | 0.1                  | 植物 |
| `visualSizeRange`      | 0.3~1.05             | 植物 |
| `wildOrder`            | 2                    | 植物 |
| `pathCost`             | 14                   | 植物 |
| `MarketValue`          | 0.3                  | 产物 |
| `daysToRotStart`       | 30                   | 产物 |
| `stackLimit`           | 默认                 | 产物 |
| `mass`                 | 默认                 | 产物 |

### 8.2 差异化建议（起点值，实测后调）

| 参数                   | 乌鸦眼 | 金银花 | 长叶车前 | 设计理由                     |
| :--------------------- | -----: | -----: | -------: | :--------------------------- |
| `growDays`             |      5 |    3.5 |        4 | 金银花最快，车前居中         |
| `harvestYield`         |      8 |      6 |       10 | 车前常见、产量大             |
| `fertilitySensitivity` |    0.7 |    0.4 |      0.6 | 金银花耐贫瘠（描述提到荒地） |
| `sowMinSkill`          |      3 |      2 |        2 | 常见药材门槛更低             |
| `MarketValue`          |    0.3 |   0.35 |      0.2 | 车前便宜量足，金银花略贵     |
| `daysToRotStart`       |     30 |     20 |       25 | 叶/花比根茎更易坏            |
| `MaxHitPoints`         |     85 |     60 |       70 | 灌木 vs 草本                 |
| `wildOrder`            |      2 |      1 |        1 | 常见植物优先生成             |

- [ ] 按上表修改三份植物 Def
- [ ] 同步修改对应产物（价值、腐烂、堆叠、重量）
- [ ] 抽公共父级 Def（`Abstract="True"`）承载共用字段，避免三处同步维护
- [ ] 复核 `drugForHarvestPurposes` / `harvestTag` 是否适合「药材」
- [ ] 开发者模式实测：单株收益 ÷ 生长时间、播种技能门槛
- [ ] 确认不存在「某一种完全优于其他」
- [ ] 最终值回写 `docs/Plan.md` 与中文描述

### 8.3 其它平衡项

- [ ] 怪物产物：市场价值、重量 0.5、堆叠 75、是否腐烂
- [ ] 成品市场价值：确保「材料成本 < 成品售价」，避免倒卖套利
- [ ] 成品腐烂：煎药/药剂/剑油建议不腐烂
- [ ] 配方 `workAmount`（1200 / 900 / 600 为占位）
- [ ] 煎药回血总量：500 HP → **30–50 HP**（如 1 HP / 600 ticks）
- [ ] 毒性：+50 / +25，衰减 -10/day，阶段 50 / 100
- [ ] 野兽油：+10% / 8 小时是否合适
- [ ] 炼金台：建材、工时、是否耗电
- [ ] 野生 `commonality`：初期地图每种可见 1–3 株
- [ ] 屠宰掉落数量（当前固定 1，无概率）
- [ ] 物品重量与堆叠（影响商队负重与仓储）

### 8.4 平衡验收

- [ ] 三种植物定位清晰：快（金银花）/ 量（车前）/ 均衡贵重（乌鸦眼）
- [ ] 单位工时收益接近，但取舍不同
- [ ] 数值表与理由写入 `docs/Plan.md`

## 9. 待决策

- [ ] 矮人烈酒：新物品 vs 复用原版 `Beer`
- [ ] 毒性是否自然衰减、速率多少
- [ ] 野兽油对昆虫/机械/人形是否生效
- [ ] 炼金台是否挂研究门禁
- [ ] 药效（煎药/药剂）是否可叠加
- [ ] 商人是否出售材料或成品

## 10. 最终验收（对应 Plan §6）

- [ ] 地图上能采集乌鸦眼、金银花、长叶车前
- [ ] 屠宰测试动物能获得三种怪物产物
- [ ] 炼金台可见三条配方，材料不足不可制作
- [ ] 巨魔煎药：出现回血 Hediff，受伤后可观测回血
- [ ] 白色拉法达：使用后立即回血
- [ ] 野兽油：对动物 +10%，对非动物不加成
- [ ] 保存读档后物品、配方、Hediff 均不丢失

---

参考：[开发计划](docs/Plan.md) · [开发文档](docs/develop.md) · [英文主页](README.md) · [中文主页](README.zh-CN.md)
