# 3D 农场模拟游戏项目开发计划书 (v2.7 最终细节全覆盖版)

## 1. 项目基本信息
- **类型**: 3D 农场模拟 / 经营
- **引擎**: Unity 2021.3+ (URP)
- **开发模式**: 垂直切片 (Vertical Slicing) - 每人对自己的模块全栈负责。
- **核心交互原则**: 极简交互。走近目标按 `E` 键自动执行动作。
- **核心架构**: **分级时间驱动架构（Time-Driven Architecture）**，不支持存档（单次体验闭环）。

---

## 2. 团队垂直分工与功能包干 (四大金刚)

| 负责人 | 认领功能点 (对应需求) | 核心负责脚本 (代码资产) | 开发任务描述 |
| :--- | :--- | :--- | :--- |
| **同学 A<br>(机动与底层)** | **⑧ 运动交互**<br>**② 牧场系统**<br>**③ 房间休息** | `PlayerController.cs`<br>`PlayerInteractor.cs`<br>`PlayerAnimator.cs`<br>`AnimalEntity.cs`<br>`BarnManager.cs`<br>`AnimalData.cs`<br>`BedInteractable.cs`<br>`GameManager.cs`<br>`CameraFollow.cs`<br>`SceneTransitionManager.cs` | **主程定位（交互与架构）**。负责玩家移动（物理/动画）、输入系统、统一射线检测交互控制中心。实现牧场系统：处理动物状态机、成长计时（监听分钟广播）及产出。负责房屋场景交互，通过“床”触发跨天逻辑并通知 TimeManager。 |
| **同学 B<br>(农业与特效)** | **① 种地系统** | `SoilTile.cs`<br>`CropEntity.cs`<br>`CropData.cs`<br>`ParticleManager.cs` | **农业核心（时间驱动）**。实现土地状态管理（荒地→耕地→播种→生长→成熟）。CropEntity 监听 TimeManager 分钟广播进行成长判定。负责所有农业动作（锄地、浇水、收获）的粒子特效表现与视觉反馈。 |
| **同学 C<br>(经济与界面)** | **④ 商店系统**<br>**⑦ 系统UI** | `InventoryManager.cs`<br>`ItemData.cs`<br>`ShopManager.cs`<br>`ShippingBin.cs`<br>`UIManager.cs`<br>`InventoryUI.cs`<br>`ShopUI.cs`<br>`TimeUI.cs`<br>`PauseMenuUI.cs`<br>`MainMenuUI.cs` | **数据中枢（UI负责人）**。实现全局唯一单例 InventoryManager，作为金币和物品的数据源。负责商店买卖逻辑、出货箱缓存与次日金币结算（监听天级广播）。负责所有 UI 面板的生命周期管理、网格刷新与交互响应。 |
| **同学 D<br>(世界与社交)** | **⑤ 居民与送礼**<br>**⑥ 昼夜光照**<br>**⑨ 音效制作** | `TimeManager.cs`<br>`ITimeObserver.cs`<br>`LightingController.cs`<br>`NPCEntity.cs`<br>`NPCData.cs`<br>`DialogueManager.cs`<br>`DialogueUI.cs`<br>`AudioManager.cs`<br>`EventManager.cs`<br>`WeatherManager.cs` | **环境专家（时间控制者）**。实现 TimeManager，负责分钟、小时、天三级委托广播。负责昼夜光照平滑过渡（监听小时广播）。负责 NPC 行为逻辑、对话系统与送礼好感度判定。负责全局 BGM 切换与各类交互音效的触发管理。 |

---

## 3. 时间系统驱动规则
- **1游戏分钟 = 1秒真实时间**（可在 TimeManager 中调节）。
- **OnMinuteChanged**: 触发作物生长、动物产出进度、TimeUI 刷新。
- **OnHourChanged**: 触发 LightingController 旋转太阳角度、NPC 行为点切换。
- **OnDayPassed**: 触发出货箱结算金币、土地干涸判定、NPC 好感度更新。

---

## 4. 完整目录结构 (全脚本/资源覆盖)

```plaintext
Assets/
├── Animations/                 # 玩家、动物、NPC 动画状态机
├── Audio/                      # BGM、环境音、UI/交互音效 (D)
├── Materials/                  # 场景、角色、作物所有材质
├── Models/                     
│   ├── Characters/             # 玩家、NPC 模型
│   ├── Crops/                  # 作物各阶段模型 (B)
│   ├── Environment/            # 房屋、农场、装饰物
│   └── Objects/                # 种子、农具、掉落物模型
├── Prefabs/                    
│   ├── Actors/                 # 玩家、NPC、动物预制体
│   ├── Interactable_objects/   # 土地、出货箱、商店 NPC 预制体
│   └── UI/                     # 各类 UI 面板预制体 (C)
├── Scenes/                     
│   ├── farm.unity              # 主农场场景
│   ├── home.unity              # 室内场景 (A)
│   └── shop.unity              # 商店/城镇场景 (C)
├── Scripts/                    
│   ├── Core/                   
│   │   ├── GameManager.cs      # 游戏主状态控制 (A)
│   │   ├── TimeManager.cs      # 时间广播中心 (D)
│   │   ├── EventManager.cs     # 全局事件总线 (D)
│   │   ├── ITimeObserver.cs    # 时间监听接口 (D)
│   │   ├── SceneTransitionManager.cs # 场景跳转 (A)
│   │   ├── CameraFollow.cs     # 相机平滑控制 (A)
│   │   └── IInteractable.cs    # 交互通用接口 (A)
│   ├── Player/                 
│   │   ├── PlayerController.cs # 移动与输入 (A)
│   │   ├── PlayerInteractor.cs # 射线检测交互中心 (A)
│   │   └── PlayerAnimator.cs   # 动画参数同步 (A)
│   ├── Farming/                
│   │   ├── SoilTile.cs         # 土地逻辑 (B)
│   │   ├── CropEntity.cs       # 作物生长逻辑 (B)
│   │   ├── CropData.cs         # 作物 SO 配置 (B)
│   │   ├── ShippingBin.cs      # 出货箱结算 (C)
│   │   └── ParticleManager.cs  # 特效管理 (B)
│   ├── Economy/                
│   │   ├── InventoryManager.cs # 物品/金币数据库 (C)
│   │   ├── ItemData.cs         # 物品 SO 配置 (C)
│   │   └── ShopManager.cs      # 商店买卖系统 (C)
│   ├── UI/                     
│   │   ├── UIManager.cs        # UI 栈管理 (C)
│   │   ├── TimeUI.cs           # 时间显示更新 (C)
│   │   ├── InventoryUI.cs      # 背包界面控制 (C)
│   │   ├── ShopUI.cs           # 商店界面控制 (C)
│   │   ├── DialogueUI.cs       # 对话框表现 (D)
│   │   ├── MainMenuUI.cs       # 主菜单逻辑 (C)
│   │   └── PauseMenuUI.cs      # 暂停菜单逻辑 (C)
│   ├── Social/                 
│   │   ├── NPCEntity.cs        # NPC 交互逻辑 (D)
│   │   ├── NPCData.cs          # NPC 属性 SO 配置 (D)
│   │   └── DialogueManager.cs  # 对话文本调度 (D)
│   ├── Ranching/               
│   │   ├── BarnManager.cs      # 动物管理中心 (A)
│   │   ├── AnimalEntity.cs     # 动物 AI 与产出 (A)
│   │   └── AnimalData.cs       # 动物 SO 配置 (A)
│   ├── Environment/            
│   │   ├── LightingController.cs# 昼夜光照控制 (D)
│   │   ├── WeatherManager.cs   # 简单天气控制 (D)
│   │   └── BedInteractable.cs  # 睡觉跨天触发器 (A)
│   └── Audio/                  
│       └── AudioManager.cs     # 音量与播放控制 (D)
├── Settings/                   # URP 渲染设置与 InputActions
└── Textures/                   # 所有的贴图、UI 图标
```

---

## 5. 项目核心规范与系统逻辑

本章节定义了所有模块必须遵守的底层逻辑。

### 5.1 资源流向与闭环 
项目严格执行“生产-暂存-结算”的单向流，确保数据安全性。

* **资源流向图**:
    `生产（种植/牧场产出）` $\rightarrow$ `Inventory（玩家背包）` $\rightarrow$ `ShippingBin（出货箱缓存）` $\rightarrow$ `次日结算（金币入账）`
* **商店准则**: 
    * 禁止通过快捷键或 UI 按钮直接打开商店。
    * 必须通过与特定的 **Merchant NPC** 触发 `Interact()` 接口后，由 NPC 脚本调用 `UIManager.OpenShop()`。
* **出货结算**:
    * 出货箱必须在 `OnDayPassed` 广播触发时，调用 `ShippingBin.Settle()` 统一核算。

---

### 5.2 时间驱动架构 
禁止使用 Unity 自带的 `Time.deltaTime` 处理业务计时，所有逻辑必须由 `TimeManager` 脉冲驱动。

#### 5.2.1 时间转换公式
$$1\text{s (Real Time)} = 2.4\text{min (Game Time)}$$
$$1\text{Day} = 24\text{Hours} = 1440\text{Minutes} \approx 10\text{min (Real Time)}$$

#### 5.2.2 行为耗时表 (Behavioral Cost)
交互行为会自动“快进”游戏时间，增强规划感：

| 行为类型 | 时间成本 (Game Minutes) | 核心触发代码 |
| :--- | :--- | :--- |
| **对话交互** | 8 分钟 | `TimeManager.AddMinutes(8);` |
| **商店交易** | 5 分钟 | `TimeManager.AddMinutes(5);` |
| **播种/收获** | 2 分钟 | `TimeManager.AddMinutes(2);` |
| **动物产出收集** | 5 分钟 | `TimeManager.AddMinutes(5);` |

#### 5.2.3 作物与动物成长算法
所有生长实体必须实现 `ITimeObserver` 接口，在 `OnMinuteChanged` 事件中累加时长。

```csharp
// 作物成长逻辑示例
void OnMinuteChanged(int delta) {
    currentTime += delta;
    if (currentTime >= growTime) {
        state = GrowthState.Mature; // 状态切换
    }
}
```
* **产出周期**: 鸡（30分钟/次）、牛（60分钟/次）。
* **昼夜界限**: 06:00 (日出) / 18:00 (日落)。

---

### 5.3 核心开发准则 (The Golden Rules)

#### ✅ 强制规范 (Do's)
* **数据驱动 (SO First)**: 所有数值（如作物售价、生长时长、NPC 初始好感度）必须存储在 `ScriptableObject` (.asset) 中。
* **接口隔离**: 跨模块调用必须通过 `IInteractable`、`IInventory` 或 `ITimeObserver`。
* **单点更新**: 金币增减必须通过 `InventoryManager.Instance` 的公共方法，严禁直接操作变量。

#### ❌ 严禁行为 (Don'ts)
* **禁止私存计时**: 严禁在脚本的 `Update()` 中使用私有变量计时。
* **禁止硬编码**: 代码中不得出现 `if(gold > 100)` 等字面量，应引用配置数据。
* **禁止绕过背包**: 严禁直接给玩家发放物品，所有产出必须先经过 `AddItem()` 判定背包是否已满。

---

### 5.4 协作红线
> **“数据不互改，时间不私计。”**
>
> 任何功能模块如果需要知道当前时间，必须向 **同学 D** 的 `TimeManager` 注册；任何模块如果需要钱或道具，必须向 **同学 C** 的 `InventoryManager` 申请。这样即便不写存盘系统，我们也能保证在单次运行中逻辑是绝对严密的。

---
## 6. GitHub 协作与版本控制须知

为了避免 Unity 项目在多人协作时出现“场景损坏”、“资源丢失”或“合并地狱”，全体成员必须严格遵守以下 Git 操作规范。

### 6.1 环境配置 (Must Do)
* **Git LFS (Large File Storage)**: 
    * 项目中包含大量模型 (.fbx)、贴图 (.png) 和音效 (.wav)，必须安装 Git LFS。
    * **操作**: 在本地仓库运行 `git lfs install`，并确保 `.gitattributes` 文件已正确配置追踪大型文件。
* **Unity 项目设置**:
    * `Edit > Project Settings > Editor > Asset Serialization` 必须设为 **Force Text**（便于 Git 追踪资源变化）。
    * `Version Control Mode` 必须设为 **Visible Meta Files**。
* **忽略文件 (.gitignore)**:
    * 必须使用针对 Unity 优化的 `.gitignore` 文件。
    * **严禁上传**: `Library/`, `Temp/`, `Obj/`, `Logs/`, `UserSettings/` 以及 `.csproj` 等本地生成文件。

### 6.2 规定

1.  **Meta 文件是生命线**:
    * Unity 为每个资源生成的 `.meta` 文件记录了 GUID 和引用关系。**严禁在未提交对应 .meta 文件的情况下提交资源。**
    * 如果 A 删除了资源但没删 .meta，或者 B 移动了位置没同步 .meta，会导致全组场景报“Missing Script”错误。
2.  **二进制冲突不可合并**:
    * **场景 (.unity) 和 预制体 (.prefab)** 虽是文本格式，但极其难以手动解决冲突。
    * **规范**: 同一时间**严禁两人修改同一个场景或同一个 Prefab**。
    * **建议**: 每个人在自己的独立场景（如 `farm_A.unity`, `shop_C.unity`）里开发，最后由 A 统一合并到主场景 `Main.unity`。
3.  **提交粒度**:
    * 坚持“小步快跑”。完成一个微小功能（如：写完一个接口、配好一个 SO）就 Commit 一次。
    * Commit 信息必须清晰，例如：`feat: [B] 完成作物生长逻辑与分钟广播监听`。

### 6.3 推荐工作流 （不严格遵守也可以）

* **Main 分支**: 保持绝对稳定，仅存放可运行的代码，严禁直接在 Main 上乱改。
* **Feature 分支**: 每个人根据自己的任务建立分支，例如 `feature-farming-B`。
* **Pull Request (PR)**: 
    * 功能完成后，发起 PR 合并回 Main。
    * **互检制**: 建议由组长或另一名组员 Code Review 后再通过合并，防止把报错的代码带入主分支。

### 6.4 突发状况处理
* **遇到冲突 (Conflict)**: 
    * 如果是代码冲突，手动保留双方逻辑；
    * 如果是场景/资源冲突，**严禁强行合并**。先回退，联系相关人员确认谁的改动为准，必要时重新手动同步修改。