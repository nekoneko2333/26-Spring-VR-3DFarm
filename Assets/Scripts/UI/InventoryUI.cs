/* ==============================================================================
 * 【背包界面表现层】 InventoryUI.cs (全自动按钮绑定版)
 * 负责人：同学 C (经济与系统 UI)
 * ============================================================================== */

using UnityEngine;
using TMPro; 
using System.Collections.Generic;
using System; 
using UnityEngine.UI; 

[System.Serializable]
public struct FixedSlot
{
    public ItemData targetItem;        
    public TextMeshProUGUI amountText; 
    public Button slotButton; 
}

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    public enum InventoryState { Normal, Gifting, Shop }
    public InventoryState currentState = InventoryState.Normal;

    [Header("背包主面板")]
    public GameObject inventoryPanel; 
    public List<FixedSlot> fixedSlots = new List<FixedSlot>();

    [Header("核心多功能交互区")]
    public Button actionButton;       
    public TextMeshProUGUI actionButtonText; 

    private ItemData selectedItem;  
    private Action<ItemData, int> currentGiftCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (actionButton != null) actionButton.onClick.AddListener(OnActionButtonClicked);

        foreach (var slot in fixedSlots)
        {
            if (slot.slotButton != null && slot.targetItem != null)
            {
                // 必须存一个局部变量，这叫“闭包防坑”
                ItemData boundItem = slot.targetItem; 
                
                // 让代码自动帮你把 OnClickSlot 方法绑到按钮上，并自动传参！
                slot.slotButton.onClick.AddListener(() => OnClickSlot(boundItem));
            }
        }
        // ====================================================================

        if (InventoryManager.Instance == null) return;

        RefreshUI();
        InventoryManager.Instance.OnInventoryChanged += RefreshUI;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (inventoryPanel != null && inventoryPanel.activeSelf) 
                CloseInventory(); 
            else 
            {
                currentState = InventoryState.Normal; 
                inventoryPanel.SetActive(true);
                RefreshActionButton(); 
            }
        }
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        Dictionary<ItemData, int> currentDict = InventoryManager.Instance.GetInventoryDict();
        foreach (var slot in fixedSlots)
        {
            if (slot.targetItem != null && slot.amountText != null)
            {
                if (currentDict.ContainsKey(slot.targetItem)) slot.amountText.text = currentDict[slot.targetItem].ToString();
                else slot.amountText.text = "0"; 
            }
        }
        RefreshActionButton(); 
    }

    // 外部系统调用接口区
    public void OpenForGifting(Action<ItemData, int> callback)
    {
        currentState = InventoryState.Gifting; 
        currentGiftCallback = callback;
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        RefreshActionButton(); 
    }

    public void OpenForShop()
    {
        currentState = InventoryState.Shop;
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        RefreshActionButton(); 
    }

    // UI 内部交互逻辑区
    public void OnClickSlot(ItemData item)
    {
        selectedItem = item;
        RefreshActionButton(); 
    }

/// <summary>
    /// 【逻辑优化】支持随时随地卖出的按钮刷新
    /// </summary>
    private void RefreshActionButton()
    {
        if (actionButton == null || actionButtonText == null) return;

        Dictionary<ItemData, int> dict = InventoryManager.Instance.GetInventoryDict();
        
        if (selectedItem == null || !dict.ContainsKey(selectedItem) || dict[selectedItem] <= 0)
        {
            actionButton.interactable = false;
            actionButtonText.text = "请选择物品";
            return;
        }

        actionButton.interactable = true; 

        switch (currentState)
        {
            case InventoryState.Normal: 
                // 1. 如果是种子或工具 -> 走装配逻辑
                if (selectedItem.itemType == ItemType.Seed) actionButtonText.text = "装备种子";
                else if (selectedItem.itemType == ItemType.Tool) actionButtonText.text = "装备工具";
                
                // 2. 核心修改：如果是农产品且可出售 -> 随时随地显示“卖出”
                else if (selectedItem.itemType == ItemType.Crop && selectedItem.isSellable)
                {
                    actionButtonText.text = $"卖出 (+{selectedItem.sellPrice}G)";
                }
                else 
                {
                    actionButton.interactable = false; 
                    actionButtonText.text = "不可使用";
                }
                break;

            case InventoryState.Gifting: 
                // 送礼逻辑保持不变，优先响应同学 D 的需求
                if (selectedItem.itemType == ItemType.Crop || selectedItem.itemType == ItemType.Material) 
                    actionButtonText.text = "确认赠送";
                else 
                {
                    actionButton.interactable = false;
                    actionButtonText.text = "不可赠送";
                }
                break;
        }
    }

    /// <summary>
    /// 【逻辑优化】按钮点击分发
    /// </summary>
    private void OnActionButtonClicked()
    {
        if (selectedItem == null) return;

        switch (currentState)
        {
            case InventoryState.Normal:
                if (selectedItem.itemType == ItemType.Seed || selectedItem.itemType == ItemType.Tool)
                {
                    // 对接同学 B 的装配逻辑
                    Debug.Log($"[装配系统] 玩家装备了: {selectedItem.itemName}");
                }
                else if (selectedItem.itemType == ItemType.Crop && selectedItem.isSellable)
                {
                    // 【随时随地卖出】核心实现
                    SellItemProcess(selectedItem);
                }
                break;

            case InventoryState.Gifting:
                currentGiftCallback?.Invoke(selectedItem, 1); 
                CloseInventory();
                break;
        }
    }

    // 封装一个通用的卖出处理函数
    private void SellItemProcess(ItemData item)
    {
        // 1. 扣除物品
        InventoryManager.Instance.RemoveItem(item, 1);
        
        // 2. 增加金币 (假设你有一个 MoneyManager)
        // MoneyManager.Instance.AddMoney(item.sellPrice);
        
        Debug.Log($"[经济系统] 随时卖出成功！获得 {item.sellPrice} 金币");
        
        // 3. 卖完后不需要关闭背包，允许玩家继续点击其他物品卖出
        RefreshUI(); 
    }
    public void CloseInventory()
    {
        if (currentState == InventoryState.Gifting) currentGiftCallback?.Invoke(null, 0);

        currentState = InventoryState.Normal; 
        selectedItem = null;
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }
}