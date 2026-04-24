**对接人**：同学 D (World & Social) -> 同学 C (Economy & UI)  
**当前状态**：逻辑层核心脚本已推送。由于调用了 UI 尚未定义的接口，目前项目会报 **CS1061 / CS0117** 错误。请同学 C 配合补全以下方法以恢复项目编译。

---

## 1. DialogueUI.cs 接口补全

请在 `DialogueUI` 类中添加以下两个方法：

```csharp
/// <summary>
/// 显示对话分支按钮面板
/// </summary>
/// <param name="isMerchant">是否是商人（决定是否显示商店按钮）</param>
public void ShowOptions(bool isMerchant)
{
    // 逻辑：
    // 1. SetActive(true) 你的选项按钮父面板
    // 2. 根据 isMerchant 显隐“商店”按钮
}

/// <summary>
/// 隐藏对话分支按钮面板
/// </summary>
public void HideOptions()
{
    // 逻辑：SetActive(false) 选项按钮父面板
}
```

### UI 实现规范

- **UI 布局**：在对话框上方创建一个 `OptionsPanel`（建议使用 Vertical Layout Group）。
- **按钮创建**：创建 3 个按钮（商店、送礼、离开）。
- **显示逻辑**：
  - 商人：显示全部 3 个按钮
  - 普通 NPC：隐藏“商店”按钮

### 按钮绑定建议（UI 点击事件）

- 商店按钮：`DialogueManager.Instance.OnClickOption(1);`
- 送礼按钮：`DialogueManager.Instance.OnClickOption(2);`
- 离开按钮：`DialogueManager.Instance.OnClickOption(3);`

> 注：普通 NPC 只有送礼和离开，对应 Index 1 和 2，逻辑已在 Manager 中处理

---

## 2. InventoryUI.cs 接口补全

请确保 `InventoryUI` 具备单例引用及送礼专用入口：

```csharp
// 1. 补全单例 (供 DialogueManager 调用)
public static InventoryUI Instance { get; private set; }

private void Awake()
{
    Instance = this;
}

// 2. 补全送礼开启接口
/// <param name="callback">回调：需回传 (选择的物品 ItemData, 赠送的数量 int)</param>
public void OpenForGifting(System.Action<ItemData, int> callback)
{
    // 逻辑：
    // 1. 打开背包面板
    // 2. 玩家点击物品后，右侧详情页显示“赠送”按钮和“数量选择器”
    // 3. 数量最大值应设为该物品在 InventoryManager 中的持有量
    // 4. 确认赠送时执行：callback?.Invoke(selectedItem, amount); 并关闭面板
    // 5. 若玩家取消(点X)，请务必执行：callback?.Invoke(null, 0); 以解除对话状态
}
```

### 送礼模式 UI 行为规范

- **方法签名**：`OpenForGifting(Action<ItemData, int> callback)`
- **复用详情页**：
  - 点击左侧物品格子 → 右侧详情面板显示物品
  - 原“使用”按钮动态替换为 **“赠送”**
- **数量选择器**：
  - 使用加减按钮或 Slider
  - 最大值必须等于 `InventoryManager` 中该物品当前持有数量
- **确认赠送**：
  ```csharp
  callback.Invoke(selectedItem, selectedAmount);
  ```
  并关闭背包

---

## 3. 取消逻辑（关键）

- 如果玩家在送礼模式下直接点击 X 关闭背包（未赠送）：

```csharp
callback.Invoke(null, 0);
```

- 否则对话系统不会正确返回“选项界面”，会卡在对话中

---

## 4. 紧急修复方案

因为有报错，如果你现在急需运行项目，请先在脚本里写上上述方法名（空壳）。只要函数签名匹配，编译报错就会消失。

---

## 5. 关于 Missing Prefab 警告

我在 `UI.unity` 场景中看到有资源丢失，请检查是否有新创建的 Prefab 或 `.meta` 文件忘记提交到 Git。
