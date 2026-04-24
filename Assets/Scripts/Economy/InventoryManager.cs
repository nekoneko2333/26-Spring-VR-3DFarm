using System;
using System.Collections.Generic;
using UnityEngine;

// 定义一个简单的结构体，用于在 Inspector 面板中方便地配置初始物品
[Serializable]
public struct ItemEntry
{
    public ItemData itemData;
    public int amount;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("玩家财富")]
    public int currentGold = 500;

    [Header("初始物品配置")]
    // 在面板里点击 "+" 号添加物品，如果不添加，默认就是空的（即所有物品数量为0）
    public List<ItemEntry> initialItems = new List<ItemEntry>();

    // 核心数据结构
    private Dictionary<ItemData, int> inventoryDict = new Dictionary<ItemData, int>();

    public event Action<int> OnGoldChanged;
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 游戏启动时，根据配置的列表进行初始化
        foreach (var entry in initialItems)
        {
            if (entry.itemData != null && entry.amount > 0)
            {
                // 直接存入字典
                inventoryDict[entry.itemData] = entry.amount;
            }
        }
        // 初始化完成后广播一次，让 UI 刷新
        OnInventoryChanged?.Invoke();
    }

    // 提供一个公开的方法，方便 UI 脚本获取目前的背包数据
    public Dictionary<ItemData, int> GetInventoryDict()
    {
        return inventoryDict;
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold);
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            return true;
        }
        return false;
    }

    public void AddItem(ItemData item, int amount = 1)
    {
        if (inventoryDict.ContainsKey(item))
            inventoryDict[item] += amount;
        else
            inventoryDict.Add(item, amount);

        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (inventoryDict.ContainsKey(item) && inventoryDict[item] >= amount)
        {
            inventoryDict[item] -= amount;
            if (inventoryDict[item] <= 0) inventoryDict.Remove(item);
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }
}