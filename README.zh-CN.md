<div align="center">

[English](README.md) · **简体中文**

# 🧙 RimWitch · 边缘女巫

**为《边缘世界》1.7 带来怪物、银剑与女巫草药的扩展**

[![RimWorld](https://img.shields.io/badge/RimWorld-1.7-2b6cb0?logo=steam&logoColor=white)](https://store.steampowered.com/app/294100/RimWorld/)
[![Version](https://img.shields.io/badge/version-0.1.0-8a63d2)](About/About.xml)
[![Harmony](https://img.shields.io/badge/Requires-Harmony-e07b39)](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)
[![License](https://img.shields.io/badge/License-GPL--3.0-3da639)](LICENSE)


<img src="About/Preview.png" alt="RimWitch 预览" width="720">

</div>

> 🌏 English readers: [read the English version](README.md).

RimWitch 是一个轻量的猎魔风味扩展：有些生物无视寻常刀剑，只畏惧银器；而三株不起眼的草药，在土里藏着有用的东西。

> [!NOTE]
> RimWitch 目前是 **0.1.0** 早期原型，包含**三种药材**（乌鸦眼、金银花、长叶车前）和**银器弱点框架**，尚未加入怪物与炼金系统——这些是下一个里程碑，见[下一步](#下一步)。

## 怪物只畏惧银器

任何被标记为「怪物」的生物，承受的非银伤害只有 **10%**：

- **银器**——银制武器，或被 Mod 标记为银剑的武器——造成完整伤害。
- **其他一切**只造成十分之一：钢铁、木棍、枪械、牙齿与利爪。
- **陷阱、爆炸与环境伤害**不受减免，因为没有人挥动它们。

这套判定是数据驱动的：任何 Mod 都可以给自己的生物与武器打标记，自动加入这套规则。

> RimWitch 自身还没有加入怪物。这套框架是为后续生物（以及其他 Mod）准备的。

## 乌鸦眼

耐寒的灌木，枝叶暗如柏树，黑栗色的根茎埋在土里。地面之上平平无奇，地面之下价值不菲。

| 属性           | 数值         |
| :------------- | :----------- |
| **生长周期**   | 5 天         |
| **单次收获**   | 8 个乌鸦眼根 |
| **种植技能**   | 3            |
| **肥力敏感度** | 0.7          |
| **可种植于**   | 土地、水培盆 |
| **市场价值**   | 每个根茎 0.3 |
| **保质期**     | 30 天后腐烂  |
| **可食用**     | 仅绝望时     |

晒干碾碎的根茎能让人在疼痛中保持清醒、在恐惧中保持镇定，代价是久久不散的偏执。它只是开始：更大的炼金线——煎药、药剂与剑油——已经在计划中。

乌鸦眼只是三种药材之一。**金银花**与**长叶车前**同样可以播种、收获，各自的生长、产量与价值仍在调整中。

## 安装

1. 订阅并启用前置 **[Harmony](https://steamcommunity.com/sharedfiles/filedetails/?id=2009463077)**。
2. 将 `RimWitch` 文件夹放入：

   ```text
   …\steamapps\common\RimWorld\Mods\
   ```

3. 在 **选项 → 模组** 中启用 `[oks]rimwitch`，并让 Harmony 排在它前面。
4. **重启游戏**。Harmony 补丁在启动时注入，仅重载 Def 不会生效。

> [!IMPORTANT]
> GitHub 源码仓库不包含编译后的 DLL。直接克隆时只会加载乌鸦眼内容，银器规则不会生效。若要从源码构建，请见 [docs/develop.md](docs/develop.md)。

## 兼容性

- 仅支持 **RimWorld 1.7**。
- 必须安装 **Harmony**。
- 纯内容 Mod 可以安全组合。
- 同样重写伤害流程的战斗大修可能表现不同，遇到问题欢迎反馈。

## 下一步

下一个里程碑是一条建立在现有药材之上的炼金闭环：

- **炼金台**——把材料做成煎药、药剂与剑油
- **怪物产物**——巨魔突变诱发物、孽鬼之心、怪物之肝
- **巨魔煎药**——持续缓慢回血
- **白色拉法达**——立即回血
- **野兽油**——对野兽增伤

配方、数值与验收清单见 [docs/Plan.md](docs/Plan.md)。这些内容仍在开发中，尚未进入当前版本。

## 致谢

- [Harmony](https://github.com/pardeike/Harmony)——Andreas Pardeike，补丁基础。
- [RimWorld](https://rimworldgame.com/)——Ludeon Studios。
- 预览图素材版权归原作者所有；如有异议会立即替换。

## 许可

GPL-3.0，详见 [LICENSE](LICENSE)。

## 给开发者

代码结构、构建方式与扩展接口：[docs/develop.md](docs/develop.md)；开发计划：[docs/Plan.md](docs/Plan.md)。
