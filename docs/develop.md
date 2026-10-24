# RimWitch 开发指南

> - **版本**：0.1.0（未打 tag 前不变）
> - **读者**：想扩展或维护本 Mod 的开发者
> - **范围**：只讲架构与扩展方式；实现细节以代码中的 XML 注释为准，后续由 Doxygen 生成 API 文档
> - **相关**：[开发计划](Plan.md) · [任务清单](../TODOLIST.md) · [项目主页](../README.md)

## 1. 设计原则

| 原则 | 含义 |
| :--- | :--- |
| 数据驱动 | 能用 Def 表达的（数值、配方、掉落、文本）不写代码 |
| 标记解耦 | `ModExtension` 只声明「这是什么」，不实现「怎么处理」 |
| 最小补丁面 | Harmony 只挂必要入口，行为集中在 Patch / Comp 中 |
| 可组合 | 其他 Mod 用同样的标记即可接入银器与怪物规则 |

## 2. 技术栈

| 项 | 现状 |
| :--- | :--- |
| 语言 / 运行时 | C#；当前 `net472`，1.7 运行时为 .NET 10，迁移计划见 [TODOLIST](../TODOLIST.md) §1 |
| 补丁库 | Harmony 2.x |
| 内容 | XML Defs + `Languages/**/DefInjected` 本地化 |
| 文档 | 代码 XML 注释；Doxygen 生成（待引入） |

## 3. 分层架构

```text
Defs/XML            内容与数值：物品、植物、配方、Hediff
   │  defName / 字段
   ▼
ModExtension        语义标记：怪物、银剑……
   │  HasModExtension<T>()
   ▼
Patch / Comp        规则与效果：伤害、治疗、使用结果
   │  游戏事件（受伤、使用、tick）
   ▼
RimWorld
```

| 层 | 位置 | 职责 |
| :--- | :--- | :--- |
| 数据层 | `1.7/Defs/**` | 定义有什么内容、数值多少 |
| 标记层 | `1.7/Sources/RimWitchCore/ModExtension/` | 给 Def 附加语义，不含逻辑 |
| 行为层 | `Patch_*.cs`、`HediffComp/`、`Ingestible/` | 读取标记，接管或补充游戏行为 |
| 文本层 | `Languages/**/DefInjected/` | 按 defName 注入译文 |

## 4. 模块职责

| 模块 | 职责 | 扩展方式 |
| :--- | :--- | :--- |
| `ModExtension/` | 纯标记类（`MonsterModExtension`、`SilverSwordModExtension`） | 新增标记类，XML 用 `Class=` 引用 |
| `Patch_*.cs` | 挂游戏事件，读取标记并调整行为 | 新规则优先复用现有入口 |
| `HediffComp/`（规划） | 持续效果，参数通过 `HediffCompProperties` 暴露给 XML | 新增 Comp，不改补丁 |
| `Ingestible/`（规划） | 一次性使用效果 | 新增 `IngestibleOutcomeDoer` |
| `Defs/` | 内容与数值 | 新增 Def，不改代码 |
| `Languages/` | 本地化 | 随 defName 增加文件 |

## 5. 关键流程

### 5.1 启动与加载

`About/About.xml` → 版本目录 `1.7/Defs` → `1.7/Assemblies/*.dll` → Harmony 注入补丁。
源码在 `1.7/Sources/`，运行时不读取，改代码必须重新构建并复制 DLL。

### 5.2 伤害规则（现有）

入口：`Pawn.PreApplyDamage`，实现见 `Patch_PreApplyDamage_SilverWeakness.cs`。

判断顺序：目标带 `MonsterModExtension` → 攻击者是 Pawn → 武器是否银制 → 调整 `dinfo.Amount`。
分支含义与已知问题写在代码注释里，这里不重复。

### 5.3 炼金链路（规划）

`RecipeDef`（材料）→ 成品 Def（挂 `Hediff` 或 `IngestibleOutcomeDoer`）→ 玩家使用 → 效果生效。
内容设计见 [Plan.md](Plan.md)，实施顺序见 [TODOLIST](../TODOLIST.md) §2–§6。

## 6. 扩展点速查

- **加怪物**：给 Pawn 的 ThingDef 挂 `MonsterModExtension`，即可获得银器弱点。
- **加银剑**：武器挂 `SilverSwordModExtension`，或使用原版 `Silver` 材质。
- **加持续效果**：实现 `HediffComp` + `HediffCompProperties`，在 XML 中填 `compClass`。
- **加即时效果**：实现 `IngestibleOutcomeDoer`，挂到成品的 `ingestible.outcomeDoers`。
- **加配方**：新增 `RecipeDef`，通过 `recipeUsers` 绑定工作台。

```xml
<modExtensions>
  <li Class="oks.MonsterModExtension">
    <monsterType>VAMPIRE</monsterType>
  </li>
</modExtensions>

<modExtensions>
  <li Class="oks.SilverSwordModExtension" />
</modExtensions>
```

## 7. 编码约定

- 命名空间统一 `oks`；defName 前缀 `Oks_RimWitch_`，按 `_Plant_` / `_Item_` / `_Hediff_` / `_Recipe_` 分段。
- 文件名与类名一致，一个文件一个职责。
- 公共类型与成员写 `/// <summary>`；复杂分支写「为什么这样做」，不写逐行翻译。
- 调试日志用完即删，不留在发布构建里。
- 新增 `.cs` 需同步 csproj（迁移到 SDK 风格后自动包含）。

## 8. 文档与注释策略

- **单一事实来源**：实现细节写在代码注释里，`docs/` 不复制。
- **Doxygen（待引入）**：`Doxyfile` + CI 生成 `docs/api/`，生成物不入库。
- **`docs/` 只保留三类**：架构（本文）、计划（`Plan.md`）、任务（`TODOLIST.md`）。
- 新增 public API 未写注释，视为未完成。

## 9. 构建与冒烟

- 构建：引用游戏 `Assembly-CSharp.dll` 与 Harmony `0Harmony.dll`，产物放入 `1.7/Assemblies/`，重启游戏生效。
- 迁移与构建细节见 [TODOLIST](../TODOLIST.md) §1。
- 冒烟测试对照 [TODOLIST](../TODOLIST.md) §10 的验收项。

## 10. 相关文档

- [开发计划](Plan.md)
- [任务清单](../TODOLIST.md)
- [英文主页](../README.md) · [中文主页](../README.zh-CN.md)
- [RimWorld Modding Wiki](https://rimworldwiki.com/wiki/Modding_Tutorials) · [Harmony 文档](https://harmony.pardeike.net/)
