/* ==============================================================================
 * 【背包界面表现层】 InventoryUI.cs (极简纯净版：完美贴合策划案)
 * 负责人：同学 C (经济与系统 UI)
 * ============================================================================== */

using UnityEngine;
using TMPro; 
using System.Collections.Generic;
using System; 
using UnityEngine.UI; 

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    // 删除了 Shop 状态，现在只有 正常(Normal) 和 送礼(Gifting) 两种状态
    public enum InventoryState { Normal, Gifting }
    public InventoryState currentState = InventoryState.Normal;

    [Header("背包主面板")]
    public GameObject inventoryPanel; 

    [Header("动态生成配置")]
    public GameObject slotPrefab; 
    public Transform slotContainer; 

    [Header("核心多功能交互区")]
    public Button actionButton;       
    public TextMeshProUGUI actionButtonText; 
    
    // 【新增】你允许加入的名字显示组件
    public TextMeshProUGUI itemNameText; 

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
        if (itemNameText != null) itemNameText.text = "请选择物品";

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

    // ==========================================================
    // 动态生成格子
    // ==========================================================
    private void RefreshUI()
    {
        if (slotPrefab == null || slotContainer == null) return;

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        Dictionary<ItemData, int> currentDict = InventoryManager.Instance.GetInventoryDict();

        foreach (var pair in currentDict)
        {
            ItemData item = pair.Key;
            int amount = pair.Value;

            if (amount <= 0) continue; 

            GameObject newSlot = Instantiate(slotPrefab, slotContainer);

            Transform imageTransform = newSlot.transform.Find("Image");
            Transform amountTransform = newSlot.transform.Find("amount");

            if (imageTransform != null)
            {
                Image icon = imageTransform.GetComponent<Image>();
                if (icon != null) icon.sprite = item.itemIcon;

                Button btn = imageTransform.GetComponent<Button>();
                if (btn != null)
                {
                    ItemData boundItem = item; 
                    btn.onClick.AddListener(() => OnClickSlot(boundItem));
                }
            }

            if (amountTransform != null)
            {
                TextMeshProUGUI amountTxt = amountTransform.GetComponent<TextMeshProUGUI>();
                if (amountTxt != null) amountTxt.text = amount.ToString();
            }
        }

        RefreshActionButton(); 
    }

    // ==========================================================
    // 同学 D 专属送礼呼出接口 (不再有 Shop 接口)
    // ==========================================================
    public void OpenForGifting(Action<ItemData, int> callback)
    {
        currentState = InventoryState.Gifting; 
        currentGiftCallback = callback;
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
        RefreshActionButton(); 
    }

    // ==========================================================
    // 交互逻辑核心 (严格遵循 3 种物品设定)
    // ==========================================================
    public void OnClickSlot(ItemData item)
    {
        selectedItem = item;
        
        // 选中物品时，显示它的名字
        if (itemNameText != null) itemNameText.text = item.itemName;
        
        RefreshActionButton(); 
    }

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
                // 1. 正常打开背包：只能装备种子，或随时卖出农产品
                if (selectedItem.itemType == ItemType.Seed) 
                {
                    actionButtonText.text = "装备种子";
                }
                else if (selectedItem.itemType == ItemType.Crop && selectedItem.isSellable)
                {
                    actionButtonText.text = $"卖出 (+{selectedItem.sellPrice}G)";
                }
                else 
                {
                    // 礼物不能在这里用
                    actionButton.interactable = false; 
                    actionButtonText.text = "不可操作";
                }
                break;

            case InventoryState.Gifting: 
                // 2. 被同学 D 的 NPC 呼出时：只能送出礼物/农产品
                // (你可以根据你们定义的礼物 ItemType 调整这里，假设礼物是 Material 或 Crop)
                if (selectedItem.itemType == ItemType.Material || selectedItem.itemType == ItemType.Crop) 
                {
                    actionButtonText.text = "确认赠送";
                }
                else 
                {
                    actionButton.interactable = false;
                    actionButtonText.text = "不可赠送";
                }
                break;
        }
    }

    private void OnActionButtonClicked()
    {
        if (selectedItem == null) return;

        switch (currentState)
        {
            case InventoryState.Normal:
                if (selectedItem.itemType == ItemType.Seed)
                {
                    if (PlayerFarming.Instance != null) PlayerFarming.Instance.EquipItem(selectedItem);
                    CloseInventory();
                }
                else if (selectedItem.itemType == ItemType.Crop && selectedItem.isSellable)
                {
                    SellItemProcess(selectedItem);
                }
                break;

            case InventoryState.Gifting:
                currentGiftCallback?.Invoke(selectedItem, 1); 
                CloseInventory();
                break;
        }
    }

    private void SellItemProcess(ItemData item)
    {
        InventoryManager.Instance.RemoveItem(item, 1);
        InventoryManager.Instance.AddGold(item.sellPrice); 
        Debug.Log($"[经济系统] 随时卖出成功！扣除 {item.itemName} x1，获得 {item.sellPrice} 金币");
        
        // 卖掉东西后，如果这个东西没库存了，把名字重置一下
        if (!InventoryManager.Instance.GetInventoryDict().ContainsKey(item) || InventoryManager.Instance.GetInventoryDict()[item] <= 0)
        {
            selectedItem = null;
            if (itemNameText != null) itemNameText.text = "请选择物品";
        }
        
        RefreshUI(); 
    }

    public void CloseInventory()
    {
        if (currentState == InventoryState.Gifting) currentGiftCallback?.Invoke(null, 0);

        currentState = InventoryState.Normal; 
        selectedItem = null;
        
        // 关掉背包时，把名字清理掉
        if (itemNameText != null) itemNameText.text = "请选择物品";
        
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }
}