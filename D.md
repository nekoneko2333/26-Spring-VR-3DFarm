## 一 调用示例
### 1. 时间系统
```c#
// 1. 推进游戏时间（任何需要“消耗时间”的交互都调用）
TimeManager.Instance.AddMinutes(5);

// 2. 订阅 / 取消订阅时间广播（作物、动物、光照等使用）
private void OnEnable() { TimeManager.Instance.OnMinuteChanged += OnMinuteChanged; }
private void OnDisable() { if (TimeManager.Instance != null) TimeManager.Instance.OnMinuteChanged -= OnMinuteChanged; }
private void OnMinuteChanged(int totalMinutes) { /* 生长/计时逻辑写这里 */ }
```

### 2. 事件总线（跨模块通知）
```c#
// 触发玩家睡觉（通常在 A 的 BedInteractable 中调用）
EventManager.TriggerPlayerSleep();

// 监听睡觉事件（例如 C 的出货结算）
private void OnEnable() { EventManager.OnPlayerSleep += MySettleFunction; }
private void OnDisable() { EventManager.OnPlayerSleep -= MySettleFunction; }
```

### 3. 对话
```c#
// 开启对话（可选回调用于在对话结束后执行额外逻辑，例如打开商店）
DialogueManager.Instance.StartDialogue(npcData, () => { /* onComplete */ });

// 强制结束当前对话
DialogueManager.Instance.EndDialogue();
```

### 4. 音频
```c#
// 播放短音效（全组统一调用音效接口）
AudioManager.Instance.PlaySFX(myAudioClip);

// 注意：BGM 会根据 TimeManager 的小时广播自动切换（日/夜）
```

### 5. NPC 交互（如何触发商店/对话）
```c#
// NPCEntity 的 Interact 已封装：按 E 调用后会自动
// - 启动/结束对话
// - 根据 isMerchant 字段在对话结束回调中打开商店
// - 自动调用 TimeManager.AddMinutes(5 或 8)
// 因此其他同学只需调用 NPCEntity.Interact()（由 PlayerInteractor 统一触发）
```

## 二 我已完成的工作（截至当前仓库状态）
- 实现并发布 `TimeManager`：分钟/小时/天 分级广播，`AddMinutes()` 核心方法。
- 实现 `ITimeObserver` 接口（示例与使用规范已写入文件）。
- 实现 `EventManager` 静态事件总线（TriggerPlayerSleep / OnPlayerSleep / OnShippingSettled 等）。
- 实现 `DialogueManager`（对话启动/结束、回调支持）与 `DialogueUI` 的初版。
- 实现 `AudioManager`：统一 `PlaySFX()` 接口、按小时自动切换 BGM。
- 实现 `LightingController`：监听时间广播并驱动昼夜光照曲线。
- 实现 `NPCEntity` 与 `NPCData` 基础结构（含 `isMerchant` 逻辑）。


## 三 给其他同学的对接说明
- 时间监听与生长逻辑（给 A、B）：
  - 在 `Start()` 或 `OnEnable()` 中订阅 `TimeManager.Instance.OnMinuteChanged`。
  - 在 `OnDestroy()` 或 `OnDisable()` 中取消订阅，避免空引用。
  - 不要在 `Update()` 做任何业务计时。

- 触发睡觉 / 次日结算（给 A、C）：
  - 当玩家确认睡觉时，只需调用：`EventManager.TriggerPlayerSleep();`。
  - 出货箱在收到 OnPlayerSleep 时会统一结算（C 已实现）。

- 对话与 NPC（给 A、C）：
  - 开启对话：`DialogueManager.Instance.StartDialogue(npcData, optionalCallback);`
  - 对话内部会阻断商店同时打开的情况，请放心调用。

- 音效（给所有人）：
  - 所有短音效请使用 `AudioManager.Instance.PlaySFX(clip);`。
  - 请在 Inspector 中把 `AudioClip` 拖到你的脚本的公开字段，然后按需调用。

## 四 场景与资源配置小贴士（给美术/场景搭建同学）
- `TimeManager`：请把 `TimeManager` Prefab 放到主场景并保证为单例（默认 Awake 会处理）。
- `AudioManager`：在场景中创建 `AudioManager`，并在 Inspector 中按脚本头部注释拖入 `bgmSource`、`sfxSource`、`dayBGM`、`nightBGM`。
- `LightingController`：把 `Directional Light` 拖给脚本的 `directionalLight`，并在面板里调好 `directionalColor`、`ambientColor`、`lightIntensity` 曲线。

---

如果你希望我把 `WeatherManager` 的初版也实现（自动下雨/停雨并触发地表湿度影响作物成长），我可以继续实现并把对接点写在这里。需要我继续吗？
