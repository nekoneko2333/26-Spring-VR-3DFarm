using UnityEngine;

[CreateAssetMenu(fileName = "NewAnimalData", menuName = "Pasture/Animal Data")]
public class AnimalData : ScriptableObject
{
    [Header("基本信息")]
    public string animalName;      // 动物名字
    public GameObject modelPrefab; // 动物的模型预制体

    [Header("生长属性")]
    public float growthTime;       // 成长所需总时间（秒）
    public float hungerRate;       // 饥饿下降速率

    [Header("产出属性")]
    // 保留 prefab 供可选视觉效果使用，但逻辑上主要使用 productItem
    public GameObject productPrefab;
    public float produceInterval;    // 产出间隔时间

    [Header("背包系统关联")]
    public ItemData productItem;     // 对应 InventoryManager 需要的物品数据资源
}