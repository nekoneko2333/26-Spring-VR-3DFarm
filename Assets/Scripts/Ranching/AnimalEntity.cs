using UnityEngine;

public class AnimalEntity : MonoBehaviour
{
    [Header("关联数据资源")]
    public AnimalData data;

    [Header("实时状态")]
    public float currentTimer;
    public bool isMature = false;
    public float currentHunger = 100f;

    private float produceTimer;

    void Start()
    {
        if (data != null)
        {
            currentTimer = data.growthTime;
            produceTimer = data.produceInterval;
        }
        else
        {
            Debug.LogError($"{gameObject.name} 没挂载 AnimalData 数据资产！");
        }
    }

    void Update()
    {
        if (data == null) return;

        // 1. 处理饥饿逻辑
        if (currentHunger > 0)
        {
            currentHunger -= data.hungerRate * Time.deltaTime;
        }

        // 2. 处理生长逻辑
        if (!isMature && currentHunger > 20f)
        {
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                OnMature();
            }
        }

        // 3. 处理产出逻辑 (仅限成年后)
        if (isMature && currentHunger > 20f)
        {
            produceTimer -= Time.deltaTime;
            if (produceTimer <= 0)
            {
                ProduceItem();
                produceTimer = data.produceInterval;
            }
        }
    }

    void OnMature()
    {
        isMature = true;
        Debug.Log($"{data.animalName} 已成年！开始准备产出。");
        transform.localScale *= 1.2f;
    }

    // 修改后的产生物品逻辑
    void ProduceItem()
    {
        // 如果没有配置产出物品（如小羊），直接返回，不再报 Warning
        if (data == null || data.productItem == null)
        {
            return;
        }

        // 只有在配置了 ItemData 的情况下才尝试存入背包
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(data.productItem, 1);
            Debug.Log($"[牧场系统] {data.animalName} 产出了 {data.productItem.name}，已直接存入背包！");
        }
        else
        {
            Debug.LogError("场景中缺少 InventoryManager 实例，产出无法存入背包！");
        }
    }
}