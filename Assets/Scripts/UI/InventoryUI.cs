using UnityEngine;
using TMPro; // 必须引入 TextMeshPro
using System.Collections.Generic;

// 定义一个结构体，把【物品数据】和【UI文字】绑在一起
[System.Serializable]
public struct FixedSlot
{
    public ItemData targetItem;        // 这个格子固定显示啥？（比如你的种子1）
    public TextMeshProUGUI amountText; // 对应的文字是哪个？（比如bag1下面的Text）
}

public class InventoryUI : MonoBehaviour
{
    [Header("背包主面板")]
    public GameObject inventoryPanel; // 在 Unity 里把背包的大底板拖进来

    [Header("背包槽位配置")]
    // 在面板里配置你的格子列表
    public List<FixedSlot> fixedSlots = new List<FixedSlot>();

    private void Start()
    {
        // --- 新增：游戏一开始，强制关闭背包 ---
        if (inventoryPanel != null)
        {
            inventoryPanel.SetActive(false);
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryUI 找不到 InventoryManager！");
            return;
        }

        // 1. 游戏开始时，立刻刷新一次UI
        RefreshUI();

        // 2. 极其重要：订阅背包数据变化的广播
        InventoryManager.Instance.OnInventoryChanged += RefreshUI;
    }

    // --- 新增：监听玩家按键，开关背包 ---
    private void Update()
    {
        // 监听键盘 B 键
        if (Input.GetKeyDown(KeyCode.B))
        {
            // 核心魔法：如果面板是开着的就关掉，关着的就打开 (Toggle 反转)
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            }
        }
    }

    private void OnDestroy()
    {
        // 3. 销毁时取消订阅，防止报错
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
        }
    }

    // 核心逻辑：刷新所有你配置好的格子
    private void RefreshUI()
    {
        // 拿到背包底层最新的真实数据字典
        Dictionary<ItemData, int> currentDict = InventoryManager.Instance.GetInventoryDict();

        // 挨个检查你在面板里配置的格子
        foreach (var slot in fixedSlots)
        {
            // 防错：确保你没有空着槽位没填
            if (slot.targetItem != null && slot.amountText != null)
            {
                // 如果字典里有这个物品，就把真实数量转成文字显示
                if (currentDict.ContainsKey(slot.targetItem))
                {
                    slot.amountText.text = currentDict[slot.targetItem].ToString();
                }
                // 如果字典里根本没有这个物品，说明数量是 0
                else
                {
                    slot.amountText.text = "0"; 
                }
            }
        }
    }
}