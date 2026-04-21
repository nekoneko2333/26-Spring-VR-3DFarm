using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 玩家点击购买按钮时调用这个方法
    /// </summary>
    public void BuyItem(ItemData item)
    {
        // 1. 检查金币够不够
        if (InventoryManager.Instance.SpendGold(item.buyPrice))
        {
            // 2. 钱够了（SpendGold已经自动扣钱了），给玩家发货
            InventoryManager.Instance.AddItem(item, 1);
            Debug.Log($"购买成功：{item.itemName}，花费了 {item.buyPrice} 金币");
        }
        else
        {
            // 钱不够，你可以在这里触发一个 UI 提示“穷鬼买不起”
            Debug.Log("购买失败：金币不足");
        }
    }
}