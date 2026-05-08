# D 模块说明：时间、世界、社交与音频

本文档记录 D 模块当前已经完成的内容，以及给其他同学使用的 NPC 添加操作指南。D 模块主要负责游戏内时间、昼夜光照、NPC 对话、送礼、路边礼物拾取、音频播放和跨模块事件通知。

## 一、已完成内容

### 1. 时间系统 TimeManager

已实现全局时间管理器 `TimeManager`，作为游戏内时间推进中心。

已完成能力：

- 以 `gameMinutesPerRealSecond` 控制真实时间到游戏时间的换算。
- 维护当前天数、小时和分钟：`currentDay`、`currentHour`、`currentMinute`。
- 提供 `AddMinutes(int amount)`，供其他模块主动推进时间。
- 广播三个时间事件：
  - `OnMinuteChanged`
  - `OnHourChanged`
  - `OnDayPassed`
- 对接睡觉事件，玩家睡觉后自动快进到次日 06:00。
- 当游戏处于对话、UI 打开或过场状态时，暂停自然时间流逝。

其他模块如果需要接入时间，不要自己写独立计时器，优先监听 `TimeManager` 的事件。

```csharp
private void OnEnable()
{
    if (TimeManager.Instance != null)
        TimeManager.Instance.OnMinuteChanged += OnMinuteChanged;
}

private void OnDisable()
{
    if (TimeManager.Instance != null)
        TimeManager.Instance.OnMinuteChanged -= OnMinuteChanged;
}

private void OnMinuteChanged(int totalMinutes)
{
    // 作物成长、产出计时、UI 刷新等逻辑
}
```

### 2. 事件总线 EventManager

已实现 `EventManager` 作为跨模块通知入口。

当前主要用于：

- `OnPlayerSleep`：玩家睡觉事件。
- `TriggerPlayerSleep()`：由床或休息交互触发，让时间系统跳到次日早晨。
- `OnShippingSettled`：出货结算事件。
- `TriggerShippingSettled(int totalGold)`：通知出货收益结算。

睡觉调用方式：

```csharp
EventManager.TriggerPlayerSleep();
```

### 3. 昼夜光照 LightingController

已实现 `LightingController`，根据 `TimeManager` 的时间变化控制环境光。

已完成能力：

- 监听 `TimeManager.OnMinuteChanged`。
- 根据当前 24 小时进度旋转方向光。
- 根据 `Gradient` 调整方向光颜色和环境光颜色。
- 根据 `AnimationCurve` 调整光照强度。

场景中使用时，需要绑定：

- `directionalLight`
- `directionalColor`
- `ambientColor`
- `lightIntensity`

### 4. 音频管理 AudioManager

已实现全局 `AudioManager`。

已完成能力：

- 管理 BGM 播放源：`bgmSource`。
- 管理短音效播放源：`sfxSource`。
- 根据小时切换白天和夜晚 BGM。
- 提供统一短音效接口：

```csharp
AudioManager.Instance.PlaySFX(clip);
```

其他模块需要播放短音效时，优先调用这个接口，不要自己临时创建 AudioSource。

### 5. NPC 数据 NPCData

已实现 `NPCData`，用于配置 NPC 的对话、头像、喜好和好感度。

当前字段包括：

- `npcName`：NPC 显示名称。
- `portrait`：NPC 头像。
- `defaultDialogues`：默认对话池，每次对话会随机抽一句。
- `lovedItemID`：该 NPC 最喜欢的物品 ID，需要和 `ItemData.itemID` 对应。
- `giftRewardFriendship`：送出喜欢礼物时增加的好感度。
- `loveGiftDialogue`：送对礼物后的反馈文本。
- `normalGiftDialogue`：普通礼物反馈文本。
- `friendshipPoints`：运行时好感度，本项目当前不做存档，只在单次运行中有效。

### 6. NPC 实体 NPCEntity

已实现 `NPCEntity`，作为场景中 NPC 的可交互入口。

已完成能力：

- 实现 `IInteractable`，可被玩家交互检测命中。
- 绑定一个 `NPCData` 数据资产。
- 支持普通 NPC 和商人 NPC 区分。
- 商人 NPC 会在对话选项中显示商店入口。
- 对话开始时 NPC 可以自动转向玩家。
- 对话结束后可恢复原朝向。

关键配置：

- `npcData`：必须绑定。
- `isMerchant`：勾选后作为商人 NPC。
- `facePlayerOnInteract`：是否对话时转向玩家。
- `faceYawOffset`：如果模型正面方向不对，用它补角度。

### 7. 对话与送礼 DialogueManager

已实现 `DialogueManager`，负责 NPC 对话流程。

已完成能力：

- 开始对话：`StartDialogue(NPCData npc, bool isMerchant, Action onComplete)`。
- 结束对话：`EndDialogue()`。
- 对话后进入选项状态。
- 普通 NPC 的数字键选项为：`1` 送礼，`2` 离开。
- 商人 NPC 的数字键选项为：`1` 商店，`2` 送礼，`3` 离开。
- 商人 NPC 可以打开 `ShopManager`。
- 送礼时打开 `InventoryUI.OpenForGifting()`，等待背包 UI 回调。
- 根据 `ItemData.itemID` 和 `NPCData.lovedItemID` 判断是否送对礼物。
- 自动增加 NPC 好感度。
- 显示送礼反馈文本。
- 对话或商店结束后统一结算交互耗时：
  - 普通对话：8 分钟。
  - 商店交互：5 分钟。

### 8. 对话 UI DialogueUI

已实现 `DialogueUI`，作为对话文字和选项按钮的 UI 门面。

已完成能力：

- 显示 NPC 名字和当前对话文本。
- 显示或隐藏对话面板。
- 显示或隐藏选项面板。
- 商人 NPC 才显示商店按钮。
- 按钮点击会调用 `DialogueManager.OnClickOption()`。

需要在场景中绑定：

- `dialoguePanel`
- `nameText`
- `contentText`
- `optionsPanel`
- `shopButton`
- `giftButton`
- `leaveButton`

### 9. 路边礼物拾取 GiftPickup

已实现 `GiftPickup`，用于制作可拾取礼物。

已完成能力：

- 实现 `IInteractable`。
- 玩家按 `E` 拾取后调用 `InventoryManager.Instance.AddItem(giftItem, amount)`。
- 拾取后销毁场景中的礼物物体。

需要绑定：

- `giftItem`：拾取后进入背包的 `ItemData`。
- `amount`：拾取数量。

## 二、其他模块对接说明

### 时间监听

需要计时的逻辑优先监听 `TimeManager`。

```csharp
TimeManager.Instance.OnMinuteChanged += OnMinuteChanged;
TimeManager.Instance.OnHourChanged += OnHourChanged;
TimeManager.Instance.OnDayPassed += OnDayPassed;
```

注意：订阅事件后必须在 `OnDisable()` 或 `OnDestroy()` 中取消订阅，避免对象销毁后仍被调用。

### 推进时间

如果某个交互需要消耗游戏时间，直接调用：

```csharp
TimeManager.Instance.AddMinutes(5);
```

### 睡觉跨天

床或休息交互触发：

```csharp
EventManager.TriggerPlayerSleep();
```

### 播放音效

短音效统一调用：

```csharp
AudioManager.Instance.PlaySFX(clip);
```

### 送礼背包入口

D 侧送礼流程会调用 C 侧背包 UI：

```csharp
InventoryUI.Instance.OpenForGifting(Action<ItemData, int> callback);
```

如果新增礼物拾取后背包 UI 中没有显示，通常不是 `GiftPickup` 的问题，而是该礼物的 `ItemData` 没有被 C 侧背包 UI 纳入显示列表或动态生成逻辑。

## 三、NPC 添加操作指南

下面是给其他同学添加新 NPC 时使用的标准流程。按这个流程做，NPC 就能被玩家按 `E` 对话、送礼；如果勾选商人，还能打开商店。

### 1. 导入 NPC 模型

1. 将 NPC 模型文件放入项目目录，例如：
   - `Assets/Models/Characters/你的NPC名称/`
2. 支持常见 Unity 模型格式，例如 `.fbx`、`.glb`。
3. 导入后检查模型材质和贴图是否正常。
4. 将模型拖入目标场景，调整位置、旋转和缩放。
5. 如果模型正面方向不对，先尽量调整模型根物体的旋转；如果仍不对，后续可用 `NPCEntity.faceYawOffset` 补角度。

### 2. 创建 NPCData 文本数据

1. 在 Project 面板中进入数据目录，例如：
   - `Assets/Scripts/Data/`
2. 右键选择：
   - `Create > FarmGame > NPC Data`
3. 命名为清晰的 NPC 数据名，例如：
   - `NPC_Blacksmith`
   - `NPC_Merchant`
4. 在 Inspector 中填写：
   - `Npc Name`
   - `Portrait`(对话时的头像，可不写)
   - `Default Dialogues`（对话时会随机抽取一句）
   - `Loved Item ID`（在giftid里面找，一共就三种）
   - `Gift Reward Friendship`
   - `Love Gift Dialogue`
   - `Normal Gift Dialogue`

文本填写限制：

- 文本内容只建议包含中文汉字和英文字母，**以及英文标点**，不能用中文标点符号！
- 不要使用特殊符号、emoji、复杂标点或富文本标签。
- 当前项目的部分中文显示链路存在乱码风险；如果某段中文在 UI 里显示乱码，请临时改成英文字母，或确认 TextMeshPro 字体资产已包含对应中文字符。
- `Loved Item ID` 必须和某个 `ItemData.itemID` 完全一致，否则系统只会按普通礼物处理。

示例：

```text
npcName: Merchant
defaultDialogues:
  Welcome
  NiceToMeetYou
lovedItemID: gift_flower
loveGiftDialogue: Thanks
normalGiftDialogue: ThankYou
```

如果要写中文，示例：

```text
npcName: 商人
defaultDialogues:
  欢迎
  今天天气很好
loveGiftDialogue: 谢谢
normalGiftDialogue: 谢谢你的礼物
```

### 3. 给模型添加交互脚本

1. 选中场景中的 NPC 模型根物体。
2. 点击 `Add Component`。
3. 添加脚本：
   - `NPCEntity`
4. 将刚创建的 `NPCData` 拖到 `NPCEntity.npcData`。
5. 如果这个 NPC 是商人，勾选 `isMerchant`。
6. 如果希望对话时 NPC 朝向玩家，保持 `facePlayerOnInteract` 勾选。
7. 如果 NPC 对话时背对玩家，调整 `faceYawOffset`，常用值为：
   - `90`
   - `-90`
   - `180`

### 4. 设置图层 Layer

玩家交互检测使用 `PlayerInteractor` 的 `interactableLayer`，当前项目已有 `Interactable` 图层。

必须设置：

1. 选中 NPC 根物体。
2. 在 Inspector 顶部 `Layer` 下拉框中选择：
   - `Interactable`
3. 如果弹出是否应用到子物体，建议选择：
   - `Yes, change children`

如果不设置为 `Interactable`，玩家的 SphereCast 检测不到该 NPC，按 `E` 不会触发对话。

### 5. 设置碰撞箱 Collider

玩家交互检测依赖物理检测，所以 NPC 必须有 Collider。

推荐设置：

1. 选中 NPC 根物体。
2. 点击 `Add Component`。
3. 添加一个：
   - `Capsule Collider`（或者box collider等）
4. 调整 Collider：（可以调整得大一点，防止交互时碰不到）
   - `Center` 对准 NPC 身体中心。
   - `Height` 覆盖角色高度。
   - `Radius` 覆盖角色宽度。
5. 不要勾选 `Is Trigger`

重要：`NPCEntity` 和 Collider 最好挂在同一个 GameObject 上。当前 `PlayerInteractor` 是对命中的 `hit.collider` 调用 `TryGetComponent<IInteractable>()`，如果 Collider 在子物体而 `NPCEntity` 在父物体，可能会检测不到。最稳妥的做法是把 Collider 和 `NPCEntity` 都放在 NPC 根物体上。

### 6. 检查玩家交互设置（这部分一般情况下不需要检查）

玩家对象上需要有 `PlayerInteractor`。

确认：

- `rayOrigin` 已绑定，通常为主摄像机；未绑定时脚本会尝试自动使用 `Camera.main`。
- `interactRange` 足够大，当前常用值为 `3`。
- `checkRadius` 足够大，当前常用值为 `0.4`。
- `interactableLayer` 包含 `Interactable`。

如果 NPC 无法交互，优先检查：

1. NPC 是否在 `Interactable` 图层。
2. NPC 是否有 Collider。
3. Collider 和 `NPCEntity` 是否在同一个物体上。
4. `NPCEntity.npcData` 是否为空。
5. 场景中是否存在 `DialogueManager` 和 `DialogueUI`。
6. 游戏是否处于 `Playing` 状态，对话或 UI 打开时不会检测新交互。

### 7. 设置对话 UI 和管理器（这部分也是）

要让 NPC 对话正常显示，场景中必须存在：

- `DialogueManager`
- `DialogueUI`
- `GameManager`
- `TimeManager`

如果要使用商人功能，还需要存在：

- `ShopManager`
- 商店 UI 面板
- 商品槽位和商品数据

如果要使用送礼功能，还需要存在：

- `InventoryManager`
- `InventoryUI`
- 可送出的 `ItemData`

### 8. 测试流程

1. 运行场景。
2. 让玩家靠近 NPC。
3. 视线对准 NPC。
4. 控制台应出现交互提示，例如：
   - `与 商人 交易`
   - `与 NPC 对话`
5. 按 `E` 开始对话。
6. 再按 `E` 显示选项。
7. 按数字键：
   - 普通 NPC：`1` 送礼，`2` 离开。
   - 商人 NPC：`1` 商店，`2` 送礼，`3` 离开。
8. 结束对话后，时间会按交互类型统一推进。

## 四、常见问题

### 按 E 没反应

优先检查 NPC 是否设置了 `Interactable` 图层和 Collider。当前交互检测不会扫描所有物体，只会检测 `PlayerInteractor.interactableLayer` 里配置的图层。

### 能检测到 NPC，但无法显示对话

检查 `NPCEntity.npcData` 是否绑定，场景中是否有 `DialogueManager` 和 `DialogueUI`。

### 对话文字乱码

当前项目部分中文文本显示可能受字体或编码影响。处理方式：

- 先确认 TextMeshPro 使用的字体资产包含中文。
- 文本只使用中文汉字和英文字母，不要混入特殊符号。
- 如果仍乱码，临时改用英文字母文本。

### NPC 面向方向不对

优先调整模型根物体旋转。若对话时转向仍偏 90 度或 180 度，在 `NPCEntity.faceYawOffset` 中补角度。

### 送礼后好感度没有按喜欢礼物增加

检查 `NPCData.lovedItemID` 是否和礼物的 `ItemData.itemID` 完全一致。大小写和下划线都必须一致。

### 商人没有商店选项

检查 `NPCEntity.isMerchant` 是否勾选。普通 NPC 不显示商店按钮。

## 五、当前状态总结

D 模块已经接通：

- 时间系统
- 跨天睡觉事件
- 昼夜光照
- BGM 和短音效
- NPC 数据配置
- NPC 场景交互
- 对话 UI
- 商人入口
- 送礼和好感度
- 路边礼物拾取
- 对话和 UI 状态下的玩家移动锁定与自然时间暂停

后续如果继续扩展，建议优先做：

- NPC 行程系统
- 不同好感度解锁不同对话
- 每日送礼次数限制
- 任务系统
- 正式存档中的 NPC 好感度保存
