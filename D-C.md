# 报错*1
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

# 建议（不做也可以
- 最好给ui都加上轴心锚点
- 背包也加一个右上角的实体化x