using UnityEngine;
using System.Collections.Generic;

public class PastureManager : MonoBehaviour
{
    public static PastureManager Instance { get; private set; }

    [Header("牧场范围配置")]
    [Tooltip("请务必在 Inspector 中拖入一个带 BoxCollider 的物体")]
    public Collider pastureArea;

    [Header("受管辖的动物列表")]
    public List<ItemData> managedAnimalItems = new List<ItemData>();

    private List<GameObject> activeAnimals = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        // 增加一点延迟确保 InventoryManager 彻底完成数据加载
        Invoke(nameof(SubscribeToInventory), 0.5f);
    }

    private void SubscribeToInventory()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += SyncPastureWithInventory;
            Debug.Log("<color=green>[牧场系统]</color> 成功连接背包事件");
            SyncPastureWithInventory();
        }
        else
        {
            Debug.LogError("[牧场系统] 找不到 InventoryManager 实例！");
        }
    }

    public void SyncPastureWithInventory()
    {
        if (InventoryManager.Instance == null) return;

        // 获取背包真实数据
        Dictionary<ItemData, int> inventory = InventoryManager.Instance.GetInventoryDict();

        foreach (var managedItem in managedAnimalItems)
        {
            if (managedItem == null || string.IsNullOrEmpty(managedItem.itemID)) continue;

            // --- 严谨匹配逻辑 ---
            int targetCount = 0;
            foreach (var kvp in inventory)
            {
                // 只有当 ID 完全一致且数量大于 0 时才统计
                if (kvp.Key != null && kvp.Key.itemID == managedItem.itemID)
                {
                    targetCount = kvp.Value;
                    break;
                }
            }

            // 统计场上已有的实体
            List<GameObject> currentSpeciesList = activeAnimals.FindAll(a =>
                a != null && a.GetComponent<AnimalEntity>() != null && a.GetComponent<AnimalEntity>().data == managedItem.animalData);

            int currentCount = currentSpeciesList.Count;

            // 只有当背包数量大于场上数量时才生成
            if (currentCount < targetCount)
            {
                int needToSpawn = targetCount - currentCount;
                Debug.Log($"<color=cyan>[同步]</color> {managedItem.itemName}: 背包有 {targetCount}，场上有 {currentCount}，补齐 {needToSpawn} 只");
                for (int i = 0; i < needToSpawn; i++)
                {
                    SpawnAnimalInPasture(managedItem.animalData);
                }
            }
        }
        activeAnimals.RemoveAll(a => a == null);
    }

    public void SpawnAnimalInPasture(AnimalData animalData)
    {
        // 报错原因：pastureArea 在面板里没拖东西
        if (pastureArea == null)
        {
            Debug.LogError("[牧场] 生成失败：未在 Inspector 面板中配置 Pasture Area！");
            return;
        }

        if (animalData == null || animalData.modelPrefab == null) return;

        Vector3 spawnPos = new Vector3(
            Random.Range(pastureArea.bounds.min.x, pastureArea.bounds.max.x),
            pastureArea.bounds.min.y,
            Random.Range(pastureArea.bounds.min.z, pastureArea.bounds.max.z)
        );

        GameObject newAnimal = Instantiate(animalData.modelPrefab, spawnPos, Quaternion.identity);

        AnimalEntity entity = newAnimal.GetComponent<AnimalEntity>();
        if (entity != null) entity.data = animalData;

        activeAnimals.Add(newAnimal);
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.OnInventoryChanged -= SyncPastureWithInventory;
    }
}