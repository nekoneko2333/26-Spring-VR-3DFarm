using UnityEngine;

public class AnimalEntity : MonoBehaviour
{
    [Header("关联数据资源")]
    public AnimalData data; // 拖入你创建的 CowData 资产

    [Header("实时状态")]
    public float currentTimer;
    public bool isMature = false;
    public float currentHunger = 100f;

    private float produceTimer;

    void Start()
    {
        // 初始化：从数据模板中读取生长时间
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
        if (!isMature && currentHunger > 20f) // 只有不饿肚子时才生长
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
                produceTimer = data.produceInterval; // 重置产出计时
            }
        }
    }

    // 成年时的逻辑
    void OnMature()
    {
        isMature = true;
        Debug.Log($"{data.animalName} 已成年！开始准备产出。");

        // 视觉反馈：比如让模型稍微变大一点
        transform.localScale *= 1.2f;
    }

    // 产生物品的逻辑
    void ProduceItem()
    {
        if (data.productPrefab != null)
        {
            // 在动物位置稍微往上一点的地方生成产出物（如牛奶瓶）
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Instantiate(data.productPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"{data.animalName} 产出了一个物品！");
        }
    }
}