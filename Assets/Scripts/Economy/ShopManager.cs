using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("右侧详情面板 UI 绑定")]
    public Image detailIcon;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailPriceText;
    public TextMeshProUGUI detailOwnedText;
    public Button buyButton;

    // 店长的记忆：当前顾客正在看哪个商品？
    private ItemData currentItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 刚打开商店，顾客还没点商品，先清空右侧屏幕
        ClearDetails();

        // 给右下角的购买按钮绑定点击事件
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyCurrentItem);
        }
    }

    // --- 导购员呼叫店长时，调用这个方法 ---
    public void ShowItemDetails(ItemData item)
    {
        currentItem = item; // 记住顾客在看啥

        // 更新右侧屏幕的文字和图片
        detailIcon.sprite = item.itemIcon;
        detailIcon.enabled = true; 
        
        detailNameText.text = item.itemName;
        detailPriceText.text = "价格:" + item.buyPrice;

        RefreshOwnedAmount(); // 查一下顾客包里有几个

        buyButton.interactable = true; // 允许点击购买
    }

    // --- 玩家点击“购买按钮”时，调用这个方法 ---
    private void BuyCurrentItem()
    {
        if (currentItem == null) return;

        // 1. 检查金币够不够 (用的就是你原本写的逻辑！)
        if (InventoryManager.Instance.SpendGold(currentItem.buyPrice))
        {
            // 2. 钱够了，给玩家发货
            InventoryManager.Instance.AddItem(currentItem, 1);
            Debug.Log($"购买成功：{currentItem.itemName}，花费了 {currentItem.buyPrice} 金币");
            
            // 3. 买完之后，马上刷新右侧显示的拥有数量
            RefreshOwnedAmount();
        }
        else
        {
            Debug.LogWarning("购买失败：金币不足");
        }
    }

    // --- 辅助方法：去背包查目前拥有多少个 ---
    private void RefreshOwnedAmount()
    {
        if (currentItem == null) return;

        Dictionary<ItemData, int> dict = InventoryManager.Instance.GetInventoryDict();
        int amount = 0;
        if (dict.ContainsKey(currentItem)) amount = dict[currentItem];

        detailOwnedText.text = "已拥有:" + amount;
    }

    // --- 辅助方法：清空屏幕 ---
    private void ClearDetails()
    {
        currentItem = null;
        detailIcon.enabled = false;
        detailNameText.text = "请选择商品";
        detailPriceText.text = "价格:-";
        detailOwnedText.text = "已拥有:-";
        buyButton.interactable = false; // 没选商品不准买
    }
}