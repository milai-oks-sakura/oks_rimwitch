# RimWitch 开发文档

> - 面向维护者：代码结构、Def 约定、构建方式、已知坑位
> - 当前内容版本：0.1（银器弱点 + 乌鸦眼）
> - 相关文档：[开发计划](Plan.md) · [英文主页](../README.md) · [中文主页](../README.zh-CN.md)

## 1. 仓库结构

```text
RimWitch/
├─ About/
│  ├─ About.xml                   # 元数据、依赖、载入顺序
│  └─ Preview.png                 # 预览图
├─ Textures/                      # 贴图（版本无关）
│  ├─ RimWitch_Items/Crow_Eye/Crows_Eye_Fruit.png
│  └─ RimWitch_Plant/Crow_Eye/Crows_EyeA.png
├─ 1.6/
│  └─ Source/RimWitchCore/        # C# 工程（构建时使用，运行时不加载 .cs）
├─ 1.7/
│  ├─ Defs/                       # 运行时 Defs
│  └─ Assemblies/                 # 运行时 DLL（当前仓库内缺失）
├─ LICENSE
└─ .gitignore
```

## 2. 加载机制

- RimWorld 按游戏版本加载同名文件夹：**1.7 只加载 `1.7/` 与公共目录**（`About`、`Textures` 等）。
- `1.7/Defs/` 下的 XML 由 Def 系统加载；`1.7/Assemblies/` 下的 DLL 在启动时载入。
- `About/About.xml` 声明 `packageId`、依赖与 `loadAfter`。
- 源码放在 `1.6/Source/` 只是历史位置；**改源码不会生效**，必须编译成 DLL 并放到 `1.7/Assemblies/` 后重启游戏。

## 3. 代码结构

```text
1.6/Source/RimWitchCore/
├─ ModExtension/
│  ├─ MonsterModExtension.cs      # 怪物标记 + MonsterType 枚举
│  └─ SilverSwordModExtension.cs  # 银剑标记（空类，纯标记）
├─ Patch_PreApplyDamage_SilverWeakness.cs
├─ Properties/AssemblyInfo.cs
├─ RimWitchCore.csproj
└─ RimWtichCore.slnx
```

### 3.1 银器弱点补丁

挂载点：`Pawn.PreApplyDamage(Pawn, ref DamageInfo, ref bool absorbed)`。

```csharp
[HarmonyPatch(typeof(Pawn), nameof(Pawn.PreApplyDamage))]
public class Patch_PreApplyDamage_SilverWeakness
{
    static private float WEAKNESS = 0.1f;

    static bool Prefix(Pawn __instance, ref DamageInfo dinfo, ref bool absorbed)
    {
        if (!__instance.def.HasModExtension<MonsterModExtension>()) return true;

        Pawn attacker = dinfo.Instigator as Pawn;
        if (attacker == null) return true;

        ThingWithComps weapon = attacker.equipment?.Primary;
        if (weapon == null) { dinfo.SetAmount(dinfo.Amount * 0.1f); return true; }

        bool isSilver = weapon.def.HasModExtension<SilverSwordModExtension>()
                     || weapon.Stuff?.defName == "Silver";
        if (!isSilver) dinfo.SetAmount(dinfo.Amount * WEAKNESS);

        return true; // 继续原版伤害流程
    }
}
```

要点：

- 只改 `dinfo.Amount`，护甲、部位、状态全部交还原版。
- 没有 `Instigator`（陷阱、爆炸、环境）直接放行，不减免。
- 空手/爪牙走 `weapon == null` 分支，同样 ×0.1——注意这里写死了 `0.1f`，与 `WEAKNESS` 重复。
- `absorbed` 目前未使用，保留 `ref` 只为匹配原方法签名，后续可在此接入护盾/免疫。

### 3.2 ModExtension

**MonsterModExtension**：标记为怪物，并带一个分类枚举（当前仅数据，无行为）。

```csharp
public enum MonsterType
{
    BEAST, CuRsedOne, INSECTOID, OGROID, SPECTER,
    RELICT, HYBRID, VAMPIRE, DRACONID, UNKNOWN
}
```

**SilverSwordModExtension**：空类，作为「这是银剑」的纯标记。

用法：

```xml
<ThingDef ParentName="BasePawn">
  <modExtensions>
    <li Class="oks.MonsterModExtension">
      <monsterType>VAMPIRE</monsterType>
    </li>
  </modExtensions>
</ThingDef>

<ThingDef ParentName="BaseMeleeWeapon_Sharp_Quality">
  <modExtensions>
    <li Class="oks.SilverSwordModExtension" />
  </modExtensions>
</ThingDef>
```

注意：`Class` 必须是程序集里的完整类型名（命名空间 `oks`）。XML 枚举值不区分大小写。

## 4. 现有 Defs

| Def | 文件 | 关键字段 |
| :--- | :--- | :--- |
| `Oks_RimWitch_Plant_CrowEye` | `1.7/Defs/ThingDefs_Plants/RimWitch_Plant_CrowEye.xml` | `growDays 5`、`harvestYield 8`、`sowMinSkill 3`、`Graphic_Random` |
| `Oks_RimWitch_Item_CrowEye` | `1.7/Defs/ThingDefs_Items/RimWitch_Item_CrowEye.xml` | `MarketValue 0.3`、`Rottable 30d`、`PlantMatter`、`DesperateOnly` |

贴图路径必须与 `texPath` 一致，且位于公共 `Textures/` 下。

## 5. 构建与环境

### 5.1 运行时目标（重要）

本机 RimWorld `1.7.4985 rev1269` 的 `Assembly-CSharp.dll` 目标框架为 **`.NETCoreApp, Version=v10.0`**（可用 `dotnet` / 反射读取 `TargetFrameworkAttribute` 验证），游戏目录内也是 CoreCLR 运行时。

而当前 `RimWitchCore.csproj` 仍使用 `net472`。理论上简单补丁可以加载，但引用体系与 API 都按 .NET Framework 编译，存在加载失败或运行时绑定失败的风险。

**建议**：把工程迁移为 SDK 风格 `net10.0`，引用游戏 DLL，构建后放入 `1.7/Assemblies/`。

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <AssemblyName>RimWtichCore</AssemblyName>
    <RootNamespace>oks</RootNamespace>
    <LangVersion>latest</LangVersion>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="Assembly-CSharp">
      <HintPath>..\..\..\..\..\Assembly-CSharp.dll</HintPath>
      <Private>false</Private>
    </Reference>
    <Reference Include="0Harmony">
      <HintPath>..\..\..\..\..\..\..\workshop\content\294100\2009463077\1.7\Assemblies\0Harmony.dll</HintPath>
      <Private>false</Private>
    </Reference>
  </ItemGroup>
</Project>
```

> 迁移前请先备份旧工程；`AssemblyName` 可以改成 `RimWitchCore`（修正拼写），但请同步改 `docs` 与打包脚本。

### 5.2 现有工程（net472）

```powershell
msbuild 1.6\Source\RimWitchCore\RimWitchCore.csproj /p:Configuration=Release
# 或
dotnet build 1.6\Source\RimWitchCore\RimWtichCore.slnx -c Release
```

| 引用 | 当前 HintPath 指向 |
| :--- | :--- |
| `Assembly-CSharp.dll` | 游戏根目录（5 级上级） |
| `0Harmony.dll` | `steamapps/workshop/content/294100/2009463077/1.6/Assemblies/` |

> `0Harmony` 指向 1.6 目录，升级到 1.7 后建议改为 `1.7/Assemblies/0Harmony.dll`。路径写死在 csproj 里，换机器/换库位需要手改。
>
> 若本机没有 .NET Framework 4.7.2 目标包，`dotnet build` 会失败，改用 Visual Studio / MSBuild，或先迁移到 `net10.0`。

构建产物：

```text
1.6/Source/RimWitchCore/bin/Release/RimWtichCore.dll
        ↓ 复制
1.7/Assemblies/RimWtichCore.dll
```

### 5.3 调试

- 日志：`%USERPROFILE%\AppData\LocalLow\Ludeon Studios\RimWorld by Ludeon Studios\Player.log`（CoreCLR 版可能略有路径差异，可在游戏目录下搜索 `Player.log`）。
- 在 `Startup`/静态构造里用 `Log.Message` 打点，确认补丁是否加载。
- 需要看 Harmony 异常时，可在临时调试版中开启 `HarmonyLib.Harmony.DEBUG = true`。
- 测试用开发者模式：`God mode` + `Destroy`/`Spawn` 工具刷物品，比走完整经济循环快。

## 6. 规范

- defName：`Oks_RimWitch_<Type>_<Name>`（`Type` 取 `Plant`/`Item`/`Building`/`Recipe`/`Hediff`/`ThingCategory`）。
- 命名空间统一 `oks`，类名 `PascalCase`，文件名与类名一致。
- 每个 XML 一文件一类内容；Def 之间用 `defName` 交叉引用，避免硬编码路径。
- 新增 `.cs` 需同步 `csproj` 的 `<Compile Include>`（除非已迁移 SDK 风格）。
- 公共资源 `Textures/`、`About/` 放在根目录，版本相关的 `Defs/`、`Assemblies/` 放 `1.7/`。

## 7. 已知问题与技术债

| 位置 | 问题 | 建议 |
| :--- | :--- | :--- |
| `About/About.xml` | `<autho>` 不是有效元素（应为 `<author>`），作者信息可能为空 | 改名 |
| `About/About.xml` | `<loadAfter>` 里嵌套了 `<li><li>…</li></li>`，列表结构非法，可能导致载入顺序解析报错 | 改成并列的 `<li>` |
| `Patch_PreApplyDamage_SilverWeakness.cs` | 空手分支硬编码 `0.1f`，与常量重复 | 统一用 `WEAKNESS` |
| 同上 | `absorbed` 未使用 | 保留或明确注释用途 |
| `RimWitchCore.csproj` | 目标 `net472`，运行时为 .NET 10 | 迁移到 `net10.0` |
| `RimWitchCore.csproj` | `AssemblyName`/文件名拼写 `RimWtichCore`（少 `c`） | 可修正，需同步文档与发布说明 |
| `1.7/` | 没有 `Assemblies/`，补丁不会加载 | 构建后放入 DLL |
| `.gitignore` | `.vscode/           # 注释` 这种行内注释会让模式失效 | 注释单独占行 |
| `MonsterType` | 枚举未被任何逻辑读取 | 见[计划](Plan.md)中的抗性/掉落扩展 |
| 全部 Defs | 无本地化文件，仅英文 | 需要 `Languages/ChineseSimplified` |
| `Oks_RimWitch_Item_CrowEye` | 继承 `RoughPlantBase`（植物基类）而非资源基类 | 核查继承字段，必要时换成 `ResourceBase` |

## 8. 扩展指南

- **加怪物**：给 Pawn 的 ThingDef 挂 `MonsterModExtension` 即可获得银器弱点。
- **加银剑**：给武器挂 `SilverSwordModExtension`，或使用原版 `Silver` 材质。
- **加植物**：复制 `RimWitch_Plant_CrowEye.xml`，改 `defName`、`texPath`、`harvestedThingDef`。
- **加炼金内容**：按 [Plan.md](Plan.md) 的 M0–M5 执行，先跑通三条配方。

## 9. 相关文档

- [开发计划（Demo 范围、配方、验收）](Plan.md)
- [英文主页](../README.md)
- [中文主页](../README.zh-CN.md)
- [RimWorld Modding Wiki](https://rimworldwiki.com/wiki/Modding_Tutorials)
- [Harmony 文档](https://harmony.pardeike.net/)
