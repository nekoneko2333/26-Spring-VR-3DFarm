using UnityEngine;

public class GiftPickup : MonoBehaviour, IInteractable
{
    [Header("对应要放进背包的礼物数据")]
    public ItemData giftItem;

    [Header("拾取数量")]
    public int amount = 1;

    public void Interact()
    {
        if (giftItem == null)
        {
            Debug.LogWarning("GiftPickup 没有绑定 giftItem");
            return;
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(giftItem, amount);
            Debug.Log($"拾取了礼物：{giftItem.itemName} x{amount}");
        }

        Destroy(gameObject);
    }

    public string GetInteractPrompt()
    {
        if (giftItem == null) return "拾取物品";
        return $"拾取 {giftItem.itemName}";
    }
}
