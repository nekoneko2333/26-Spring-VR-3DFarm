using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("商店大底板 (用来控制开关)")]
    public GameObject shopPanel; // 回到 Unity，把你的商店背景大面板拖进来

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
        // 游戏一开始，确保商店是关着的
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        // 给右下角的购买按钮绑定点击事件
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyCurrentItem);
        }
    }

    // ==========================================
    // 新增：给外部调用的开关门接口
    // ==========================================

    /// <summary>
    /// 打开商店（由同学 D 的 NPC 交互代码来调用）
    /// </summary>
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            ClearDetails(); // 每次进店，先清空右侧显示
        }
    }

    /// <summary>
    /// 关闭商店（绑给你的右上角叉叉按钮）
    /// </summary>
    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    // ==========================================
    // 原有的购买与展示逻辑
    // ==========================================

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

        // 1. 检查金币够不够 
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