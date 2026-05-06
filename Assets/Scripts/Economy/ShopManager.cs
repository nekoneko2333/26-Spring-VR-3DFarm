using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("商店大底板 (用来控制开关)")]
    public GameObject shopPanel; 

    [Header("右侧详情面板 UI 绑定")]
    public Image detailIcon;
    public TextMeshProUGUI detailNameText;
    public TextMeshProUGUI detailPriceText;
    public TextMeshProUGUI detailOwnedText;
    public Button buyButton;

    private ItemData currentItem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("场景中出现了第二个 ShopManager，正在销毁多余的！");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

private void OnDestroy()
{
    if (Instance == this)
    {
        Instance = null;
    }
}

    private void Start()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuyCurrentItem);
        }
    }

    // ==========================================
    // 外部调用的开关门接口
    // ==========================================
    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            ClearDetails(); // 每次进店，先清空右侧显示
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    // ==========================================
    // 原有的购买与展示逻辑 (已增加完美防错判空)
    // ==========================================
    public void ShowItemDetails(ItemData item)
    {
        currentItem = item;

        // 【防弹衣逻辑】：确认 detailIcon 存活才操作
        if (detailIcon != null)
        {
            detailIcon.sprite = item.itemIcon;
            detailIcon.enabled = true; 
        }
        else Debug.LogWarning("ShopManager 找不到 detailIcon！图片丢失或未绑定。");
        
        if (detailNameText != null) detailNameText.text = item.itemName;
        if (detailPriceText != null) detailPriceText.text = "价格:" + item.buyPrice;

        RefreshOwnedAmount(); 

        if (buyButton != null) buyButton.interactable = true; 
    }

    private void BuyCurrentItem()
    {
        if (currentItem == null) return;

        if (InventoryManager.Instance.SpendGold(currentItem.buyPrice))
        {
            InventoryManager.Instance.AddItem(currentItem, 1);
            Debug.Log($"购买成功：{currentItem.itemName}，花费了 {currentItem.buyPrice} 金币");
            
            RefreshOwnedAmount();
        }
        else
        {
            Debug.LogWarning("购买失败：金币不足");
        }
    }

    private void RefreshOwnedAmount()
    {
        if (currentItem == null || detailOwnedText == null) return;

        Dictionary<ItemData, int> dict = InventoryManager.Instance.GetInventoryDict();
        int amount = 0;
        if (dict.ContainsKey(currentItem)) amount = dict[currentItem];

        detailOwnedText.text = "已拥有:" + amount;
    }

    private void ClearDetails()
    {
        currentItem = null;

        // 【防弹衣逻辑】：依次确认组件存活才清空
        if (detailIcon != null) detailIcon.enabled = false;
        if (detailNameText != null) detailNameText.text = "请选择商品";
        if (detailPriceText != null) detailPriceText.text = "价格:-";
        if (detailOwnedText != null) detailOwnedText.text = "已拥有:-";
        if (buyButton != null) buyButton.interactable = false; 
    }
}