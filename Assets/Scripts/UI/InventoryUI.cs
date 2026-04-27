/* ==============================================================================
 * 【背包界面表现层】 InventoryUI.cs (终极版：动态生成 + 全能状态机)
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

    public enum InventoryState { Normal, Gifting, Shop }
    public InventoryState currentState = InventoryState.Normal;

    [Header("背包主面板")]
    public GameObject inventoryPanel; 

    [Header("动态生成配置")]
    [Tooltip("把做好的 bag1 预制体拖到这里")]
    public GameObject slotPrefab; 
    [Tooltip("用来装格子的父节点（挂了 GridLayoutGroup 的那个空物体）")]
    public Transform slotContainer; 

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

        if (InventoryManager.Instance == null) return;

        // 第一次打开游戏时刷新一次
        RefreshUI();
        
        // 监听后台数据变化（比如捡到礼物、买卖东西），一有变化自动重新搭台子
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
    // 🌟 核心魔法：根据字典数据动态生成格子
    // ==========================================================
    private void RefreshUI()
    {
        if (slotPrefab == null || slotContainer == null) return;

        // 1. 拆掉旧台子：清空容器里的所有现有格子
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. 拿到真实的背包数据
        Dictionary<ItemData, int> currentDict = InventoryManager.Instance.GetInventoryDict();

        // 3. 遍历数据，有啥造啥
        foreach (var pair in currentDict)
        {
            ItemData item = pair.Key;
            int amount = pair.Value;

            if (amount <= 0) continue; // 数量为0的就不造格子了

            // 4. 克隆一个新格子到容器下
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);

            // 5. 根据你 bag1 的结构，自动寻找并赋值
            // 注意：这里强依赖你的子物体名字，如果改名了这里也要跟着改
            Transform imageTransform = newSlot.transform.Find("Image");
            Transform amountTransform = newSlot.transform.Find("amount");

            if (imageTransform != null)
            {
                // 换图片
                Image icon = imageTransform.GetComponent<Image>();
                if (icon != null) icon.sprite = item.itemIcon;

                // 绑按钮事件
                Button btn = imageTransform.GetComponent<Button>();
                if (btn != null)
                {
                    ItemData boundItem = item; // 闭包防坑
                    btn.onClick.AddListener(() => OnClickSlot(boundItem));
                }
            }

            if (amountTransform != null)
            {
                // 改数量
                TextMeshProUGUI amountTxt = amountTransform.GetComponent<TextMeshProUGUI>();
                if (amountTxt != null) amountTxt.text = amount.ToString();
            }
        }

        // 刷新大按钮的状态
        RefreshActionButton(); 
    }

    // ==========================================================
    // 外部系统调用接口区
    // ==========================================================
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

    // ==========================================================
    // UI 内部交互逻辑区
    // ==========================================================
    public void OnClickSlot(ItemData item)
    {
        selectedItem = item;
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
                if (selectedItem.itemType == ItemType.Seed) actionButtonText.text = "装备种子";
                else if (selectedItem.itemType == ItemType.Tool) actionButtonText.text = "装备工具";
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
                if (selectedItem.itemType == ItemType.Crop || selectedItem.itemType == ItemType.Material) 
                    actionButtonText.text = "确认赠送";
                else 
                {
                    actionButton.interactable = false;
                    actionButtonText.text = "不可赠送";
                }
                break;

            case InventoryState.Shop:
                // 兼容老商店面板的防护（因为现在卖出合在 Normal 里了，这里其实可以不写，但保留以防万一）
                actionButton.interactable = false; 
                actionButtonText.text = "商店模式";
                break;
        }
    }

    private void OnActionButtonClicked()
    {
        if (selectedItem == null) return;

        switch (currentState)
        {
            case InventoryState.Normal:
                if (selectedItem.itemType == ItemType.Seed || selectedItem.itemType == ItemType.Tool)
                {
                    Debug.Log($"[装配系统] 玩家装备了: {selectedItem.itemName}");

                    // =========================================================
                    // 🌟 【你加的调度指令就在这里】🌟
                    // 1. 把选中的物品发送给你的 PlayerFarming 管家
                    if (PlayerFarming.Instance != null)
                    {
                        PlayerFarming.Instance.EquipItem(selectedItem);
                    }

                    // 2. 装备成功后，帮玩家自动关掉背包，方便他立刻去种地！
                    CloseInventory();
                    // =========================================================
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
        Debug.Log($"[经济系统] 随时卖出成功！获得 {item.sellPrice} 金币");
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