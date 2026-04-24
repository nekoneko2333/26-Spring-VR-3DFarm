# D -> C 对接说明

## 1. 报错*1
在和商人对话进入商店后报错

MissingReferenceException: The object of type 'Image' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
ShopManager.ClearDetails () (at Assets/Scripts/Economy/ShopManager.cs:129)
ShopManager.OpenShop () (at Assets/Scripts/Economy/ShopManager.cs:56)
DialogueManager.OpenShopLogic () (at Assets/Scripts/Social/DialogueManager.cs:116)
DialogueManager.OnClickOption (System.Int32 index) (at Assets/Scripts/Social/DialogueManager.cs:87)
DialogueUI+<>c.<Start>b__12_0 () (at Assets/Scripts/UI/DialogueUI.cs:34)
UnityEngine.Events.InvokableCall.Invoke () (at <f68865d630f84dbe83e4490ea2afd98a>:0)
UnityEngine.Events.UnityEvent.Invoke () (at <f68865d630f84dbe83e4490ea2afd98a>:0)
UnityEngine.UI.Button.Press () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:70)
UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents.Execute (UnityEngine.EventSystems.IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents.Execute[T] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents+EventFunction`1[T1] functor) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:272)
UnityEngine.EventSystems.EventSystem:Update() (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:530)

MissingReferenceException: The object of type 'Image' has been destroyed but you are still trying to access it.
Your script should either check if it is null or you should not destroy the object.
ShopManager.ShowItemDetails (ItemData item) (at Assets/Scripts/Economy/ShopManager.cs:82)
ShopSlotUI.OnClickSlot () (at Assets/Scripts/UI/ShopSlotUI.cs:34)
UnityEngine.Events.InvokableCall.Invoke () (at <f68865d630f84dbe83e4490ea2afd98a>:0)
UnityEngine.Events.UnityEvent.Invoke () (at <f68865d630f84dbe83e4490ea2afd98a>:0)
UnityEngine.UI.Button.Press () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:70)
UnityEngine.UI.Button.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Button.cs:114)
UnityEngine.EventSystems.ExecuteEvents.Execute (UnityEngine.EventSystems.IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:57)
UnityEngine.EventSystems.ExecuteEvents.Execute[T] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents+EventFunction`1[T1] functor) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:272)
UnityEngine.EventSystems.EventSystem:Update() (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:530)

## 建议
- 最好给ui都加上轴心锚点
- 背包也加一个右上角的实体化x

--- 
## 2. 当前 C 侧需要补的内容

### 礼物拾取后进入背包数据，但不会自动显示在背包 UI

我这边已经完成了路边礼物拾取逻辑：

- 礼物物体挂 `GiftPickup.cs`
- 玩家按 `E` 触发 `Interact()`
- 调用：

```csharp
InventoryManager.Instance.AddItem(giftItem, amount);
```

也就是说，礼物已经成功进入 `InventoryManager` 的字典数据。

但现在不会显示在背包 UI / 商店 UI，原因是 C 这边当前的 UI 逻辑不是“自动根据背包数据动态生成”，而是“手动配置固定槽位”。

---

## 3. 当前确认到的原因

### 背包 UI

`InventoryUI.cs` 里现在使用的是：

```csharp
public List<FixedSlot> fixedSlots = new List<FixedSlot>();
```

刷新 UI 时，只会遍历这些已经手动配置过的 `fixedSlots`。

所以如果新的礼物 `ItemData` 没有被手动配置进某个 `FixedSlot.targetItem`，即使它已经进了背包数据，也不会显示出来。

### 商店 UI

`ShopSlotUI.cs` 里每个格子也是手动绑定一个：

```csharp
public ItemData item;
```

所以“捡到礼物”不会自动让它出现在商店里。  
如果 C 希望礼物也在商店面板显示，需要单独配置对应的商店槽位，或者明确区分“商店卖的物品”和“玩家背包里的物品”。

---

## 4. C 侧建议补充的逻辑


1. 在 `InventoryUI` 的 `fixedSlots` 中为礼物 `ItemData` 增加槽位
2. 绑定该槽位对应的：
   - `targetItem`
   - `amountText`
   - `slotButton`
3. 确保礼物物品加入背包后，`OnInventoryChanged` 能刷新到这个槽位

如果需要礼物也出现在商店里：

4. 在商店 UI 中新增一个 `ShopSlotUI`
5. 把礼物 `ItemData` 绑定到它的 `item`

### 更完整的长期方案

如果 C 后续希望支持“新物品自动显示”，建议把 `InventoryUI` 从固定槽位改成动态生成格子，而不是继续依赖手动配置 `fixedSlots`。



---
