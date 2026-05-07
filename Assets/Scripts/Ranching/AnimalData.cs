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
    public GameObject productPrefab; // 产出的东西（比如牛奶、鸡蛋）
    public float produceInterval;    // 产出间隔时间
}