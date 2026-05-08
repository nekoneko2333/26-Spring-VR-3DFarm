# 3D 农场模拟游戏

这是一个基于 Unity 的 3D 农场模拟经营项目。玩家在农场世界中移动探索，通过统一的 `E` 键交互完成种植、收获、商店交易、NPC 对话、送礼、休息跨天和礼物拾取等操作。项目当前定位为课程/小组项目原型，已经接入玩家控制、农业、经济、UI、社交、时间、昼夜光照、音频和牧场相关模块。

## 项目概览

- **类型**：3D 农场模拟 / 轻经营
- **引擎版本**：Unity 2022.3.51f1c1
- **核心交互**：靠近目标并按 `E`
- **构建入口场景**：`Assets/Scenes/MainMenu.unity`
- **主要玩法**：种植作物、管理背包与金币、商店购买、NPC 对话送礼、动物养殖、昼夜时间推进

## 当前可用内容

### 玩家与交互

项目包含玩家移动、奔跑、跳跃、下蹲、视角控制、动画同步和交互检测。`PlayerInteractor` 使用 SphereCast 检测 `Interactable` 图层上的可交互对象，命中实现了 `IInteractable` 的组件后，按 `E` 调用对应交互逻辑。

### 农业系统

农业模块包含土地状态、作物数据、播种、浇水、成长、收获和农业特效。土地状态包括荒地、耕地、已播种、已浇水和枯萎。作物相关数据通过 `CropData` 和 `ItemData` 配置，目前资源中包含萝卜、南瓜、大白菜等作物与种子。

### 经济与背包

`InventoryManager` 统一管理玩家金币和物品数量，`ItemData` 描述物品 ID、名称、图标、价格、预制体和类型。商店系统可展示商品并处理购买逻辑，出货箱模块可用于将收获物转化为收益。

### NPC、对话与送礼

NPC 使用 `NPCData` 配置姓名、头像、随机对话、喜爱物品和送礼反馈。`NPCEntity` 负责场景交互入口，可区分普通 NPC 和商人 NPC。普通 NPC 的选项为 `1` 送礼、`2` 离开；商人 NPC 的选项为 `1` 商店、`2` 送礼、`3` 离开。

### 时间、昼夜与音频

`TimeManager` 负责游戏时间推进，并广播分钟、小时和跨天事件。对话、商店或过场状态下自然时间会暂停，交互结束后统一结算耗时。`LightingController` 根据时间驱动方向光和环境光变化，`AudioManager` 负责 BGM 切换和短音效播放。

### 牧场系统

牧场模块包含 `AnimalData`、`AnimalEntity` 和 `BarnManager`。动物拥有成长、饥饿和产出相关数据，可用于扩展牛、羊、猪、鸡等养殖内容。项目资源中已包含多种动物模型和牛奶、鸡蛋等产物资源。

## 基本操作

| 操作 | 功能 |
| --- | --- |
| `W / A / S / D` | 移动 |
| `Shift` | 奔跑 |
| `Space` | 跳跃 |
| `Ctrl` | 下蹲 |
| 鼠标拖拽 | 调整视角 |
| `E` | 交互、推进 NPC 对话 |
| 普通 NPC 选项 `1 / 2` | 送礼 / 离开 |
| 商人 NPC 选项 `1 / 2 / 3` | 商店 / 送礼 / 离开 |
| `I` | 调试查看背包 |
| `G` | 调试催熟当前作物 |

`I` 和 `G` 属于开发调试按键，正式展示或构建时可按需要移除。

## 运行方式

1. 使用 Unity Hub 打开项目根目录。
2. 使用 Unity **2022.3.51f1c1**，或尽量使用同系列 2022.3 LTS 版本。
3. 等待 Unity 导入资源并恢复 Package Manager 依赖。
4. 打开 `Assets/Scenes/MainMenu.unity`。
5. 点击 Unity 播放按钮运行。

当前 Build Settings 中启用的场景：

- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/UI.unity`

仓库内还包含开发与测试场景：

- `Assets/Scenes/farm.unity`
- `Assets/Scenes/home.unity`
- `Assets/Scenes/shop.unity`
- `Assets/Scenes/Pasture.unity`
- `Assets/Scenes/farm_B_test.unity`
- `Assets/Scenes/A_Rest_Test.unity`
- `Assets/Scenes/D_TestScene.unity`

## 目录结构

```text
Assets/
  Animations/          动画资源
  Audio/               BGM 和交互音效
  Editor/              编辑器辅助脚本
  Materials/           土地、作物、场景等材质
  Models/
    Characters/        角色和 NPC 模型
    Crops/             作物、土地与成长阶段模型
    Environment/       农场、房屋、商店等环境模型
    items/             道具和礼物模型
    Pasture/           动物和牧场相关模型
  Prefabs/
    Actors/            玩家和 NPC 预制体
    Animals/           动物与动物产物预制体
    Farming/           作物、种子和农业相关预制体
    gift_items/        礼物预制体
    Interactable_objects/ 可交互物体
    Managers/          管理器预制体
    system/            摄像机、灯光等系统预制体
    UI/                UI 预制体
    zhx_Farming/       农业测试和模块预制体
  Scenes/              Unity 场景
  Scripts/
    Audio/             音频管理
    Core/              游戏状态、时间、事件、场景切换、通用接口
    Data/              ScriptableObject 数据资产
    Economy/           背包、金币、物品、商店
    Environment/       门、床、光照等环境交互
    Farming/           土地、作物、种植、收获、农业特效
    Player/            玩家移动、动画、视角和交互检测
    Ranching/          动物数据、动物实体、牧场管理
    Social/            NPC、对话、送礼、礼物拾取
    UI/                背包、金币、商店、时间、对话、主菜单
    others/            临时或待归类脚本目录
  TextMesh Pro/        TextMesh Pro 资源
  Textures/            UI、作物、牧场和礼物贴图
Packages/              Unity 包依赖配置
ProjectSettings/       Unity 项目设置
```

## 主要脚本模块

- `Assets/Scripts/Core/TimeManager.cs`：游戏时间推进和时间事件广播。
- `Assets/Scripts/Core/GameManager.cs`：游戏状态管理。
- `Assets/Scripts/Core/IInteractable.cs`：统一交互接口。
- `Assets/Scripts/Player/PlayerInteractor.cs`：玩家交互检测和 `E` 键触发。
- `Assets/Scripts/Farming/SoilTile.cs`：土地状态和种植交互。
- `Assets/Scripts/Farming/CropEntity.cs`：作物成长逻辑。
- `Assets/Scripts/Economy/InventoryManager.cs`：背包和金币中心。
- `Assets/Scripts/Economy/ShopManager.cs`：商店购买逻辑。
- `Assets/Scripts/Social/NPCEntity.cs`：NPC 场景交互入口。
- `Assets/Scripts/Social/NPCData.cs`：NPC 对话和送礼数据。
- `Assets/Scripts/Social/DialogueManager.cs`：NPC 对话、商店入口和送礼流程。
- `Assets/Scripts/Social/GiftPickup.cs`：可拾取礼物。
- `Assets/Scripts/Environment/LightingController.cs`：昼夜光照。
- `Assets/Scripts/Audio/AudioManager.cs`：BGM 和短音效。

## 依赖包

项目通过 Unity Package Manager 管理依赖，主要包含：

- TextMesh Pro
- Unity UI
- AI Navigation
- Post Processing
- Timeline
- Visual Scripting
- glTFast

其中 `com.atteneder.gltfast` 通过 GitHub 地址引入。首次打开项目时，需要确保网络或本地缓存可以解析该包。

## 协作文档

根目录下保留了小组协作文档：

- `分工.md`：原始分工和整体设计说明。
- `C.md`：C 模块相关说明。
- `D.md`：D 模块说明，以及 NPC 添加操作指南。
- `D-C.md`：D 与 C 模块对接说明。

其中 `D.md` 已包含 NPC 添加流程：模型导入、创建 `NPCData`、挂 `NPCEntity`、设置 `Interactable` 图层、碰撞箱、对话文本、商人配置和测试步骤。

## 资源与版本管理

项目包含大量模型、贴图、音频和 Unity 预制体资源。仓库已配置 `.gitattributes` 和 `.gitignore`，建议继续使用 Git LFS 管理 `.fbx`、`.glb`、`.png`、`.wav`、`.mp3` 等大型资源文件。Unity 协作时需要同时提交资源文件和对应 `.meta` 文件，避免引用丢失。

## 当前状态

项目已经具备农场模拟游戏的核心原型：玩家移动与交互、种植流程、背包金币、商店、NPC 对话送礼、礼物拾取、牧场动物、时间推进、昼夜光照、音频和基础 UI。后续可继续完善存档、任务、NPC 行程、更多作物动物、数值平衡和正式发布构建。
