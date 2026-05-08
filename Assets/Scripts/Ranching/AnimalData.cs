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
    public GameObject productPrefab; // 可选特效预制体
    public float produceInterval;    // 产出间隔时间

    [Header("背包系统关联")]
    public ItemData productItem;     // 产出的物品（如牛奶）
    public ItemData animalItem;      // 【新增】代表动物本身的背包物品（如奶牛卡片）


}