## 一、调用示例

### 1. 时间系统

```csharp
// 推进游戏时间
TimeManager.Instance.AddMinutes(5);

// 监听分钟广播
private void OnEnable() { TimeManager.Instance.OnMinuteChanged += OnMinuteChanged; }
private void OnDisable() { if (TimeManager.Instance != null) TimeManager.Instance.OnMinuteChanged -= OnMinuteChanged; }
private void OnMinuteChanged(int totalMinutes) { /* 生长 / 计时逻辑 */ }
```

### 2. 事件总线（跨模块通知）

```csharp
// 触发玩家睡觉
EventManager.TriggerPlayerSleep();

// 监听睡觉事件
private void OnEnable() { EventManager.OnPlayerSleep += MySettleFunction; }
private void OnDisable() { EventManager.OnPlayerSleep -= MySettleFunction; }
```

### 3. 对话

```csharp
// 开启对话
DialogueManager.Instance.StartDialogue(npcData, isMerchant);

// 强制结束当前对话
DialogueManager.Instance.EndDialogue();
```

### 4. 音频

```csharp
AudioManager.Instance.PlaySFX(myAudioClip);
```

### 5. 路边拾取礼物

```csharp
// GiftPickup 挂在可交互礼物物体上
// 玩家按 E 后会调用：
InventoryManager.Instance.AddItem(giftItem, amount);
```

---

## 二、我已经完成的工作

- 实现并发布 `TimeManager`
  - 分钟 / 小时 / 天三级广播
  - `AddMinutes()` 核心时间推进方法
- 实现 `ITimeObserver`
- 实现 `EventManager`
  - `TriggerPlayerSleep`
  - `OnPlayerSleep`
  - 其他跨模块广播接口
- 实现 `DialogueManager`
  - 开启对话
  - 结束对话
  - 选项分支
  - 送礼回调处理
- 实现 `NPCEntity` 与 `NPCData`
  - 普通 NPC / 商人区分
  - 送礼偏好字段
  - 好感度字段
- 实现送礼逻辑
  - 根据 `NPCData.lovedItemID` 判断是否送对礼物
  - 根据礼物结果增加好感度
  - 显示对应 NPC 反馈台词
- 实现路边礼物拾取 `GiftPickup`
  - 按 `E` 拾取礼物
  - 礼物加入 `InventoryManager`
- 实现对话与时间/移动状态联动
  - 对话期间玩家不能移动
  - 商店打开期间玩家不能移动
  - 对话 / 商店期间自然时间暂停
  - 时间改为交互结束后统一结算
- 实现 `AudioManager`
  - `PlaySFX()` 统一接口
  - BGM 按时间自动切换
- 实现 `LightingController`
  - 根据时间变化驱动昼夜光照

---

## 三、给其他同学的对接说明

### 给 A / B：时间监听

- 需要计时的逻辑统一监听 `TimeManager.Instance.OnMinuteChanged`
- 不要在业务逻辑里自己用 `Update()` 私设计时
- 记得在 `OnDisable()` / `OnDestroy()` 里取消订阅

### 给 A / C：睡觉与次日结算

- 玩家确认睡觉时调用：

```csharp
EventManager.TriggerPlayerSleep();
```

### 给 C：背包 / 送礼 / 礼物 UI

- 送礼时 D 侧调用：

```csharp
InventoryUI.Instance.OpenForGifting(Action<ItemData, int> callback);
```

- 路边礼物拾取时，D 侧已经调用：

```csharp
InventoryManager.Instance.AddItem(giftItem, amount);
```

- 当前礼物不显示在背包 UI 的原因，不在 D 的拾取逻辑，而在 C 的 `InventoryUI` 仍然是固定槽位 `fixedSlots`
- 如果要显示新礼物，需要 C：
  - 手动把礼物 `ItemData` 配进 `InventoryUI.fixedSlots`
  - 或把背包改成动态生成格子

### 给所有人：音效

- 所有短音效统一调用：

```csharp
AudioManager.Instance.PlaySFX(clip);
```

---

## 四、资源与配置提醒

- `TimeManager`：场景中保留单例
- `AudioManager`：正确绑定 `bgmSource` / `sfxSource` / `dayBGM` / `nightBGM`
- `LightingController`：绑定方向光并配置曲线
- `NPCData`：需要填写
  - `lovedItemID`
  - `giftRewardFriendship`
  - `loveGiftDialogue`
  - `normalGiftDialogue`
- `GiftPickup`：需要绑定
  - `giftItem`
  - `amount`

---

## 五、当前状态总结

我这边 D 模块已经把：

- 时间系统
- 对话系统
- 商人/送礼流程
- 路边礼物拾取
- 对话期间暂停时间与锁移动
- 音频与昼夜

这些内容基本接通了。

目前和 C 的主要待对接点，是“礼物进入背包后如何在 UI 中显示”。
