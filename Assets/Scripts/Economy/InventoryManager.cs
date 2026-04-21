using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("玩家财富")]
    public int currentGold = 500; // 初始给500块钱测试用

    // 核心数据结构：背包字典。Key是物品数据，Value是拥有的数量
    private Dictionary<ItemData, int> inventoryDict = new Dictionary<ItemData, int>();

    // 广播事件：当金币或背包发生变化时，通知 UI 去刷新
    public event Action<int> OnGoldChanged;
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保证切场景（如从农场去商店）时数据不丢
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- 金币管理方法 ---
    public void AddGold(int amount)
    {
        currentGold += amount;
        OnGoldChanged?.Invoke(currentGold); // 广播：钱变啦！UI快更新！
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            OnGoldChanged?.Invoke(currentGold);
            return true; // 扣款成功
        }
        Debug.LogWarning("金币不足！");
        return false; // 扣款失败
    }

    // --- 物品管理方法 ---
    public void AddItem(ItemData item, int amount = 1)
    {
        if (inventoryDict.ContainsKey(item))
            inventoryDict[item] += amount;
        else
            inventoryDict.Add(item, amount);

        OnInventoryChanged?.Invoke(); // 广播：包里东西变啦！
        Debug.Log($"获得了 {amount} 个 {item.itemName}");
    }

    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (inventoryDict.ContainsKey(item) && inventoryDict[item] >= amount)
        {
            inventoryDict[item] -= amount;
            if (inventoryDict[item] <= 0)
            {
                inventoryDict.Remove(item); // 数量为0时移除
            }
            OnInventoryChanged?.Invoke();
            return true;
        }
        Debug.LogWarning("物品数量不足以扣除！");
        return false;
    }
}