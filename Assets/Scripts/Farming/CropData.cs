using UnityEngine;

// 允许在编辑器菜单创建数据文件
[CreateAssetMenu(fileName = "New Crop Data", menuName = "Farm/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("作物基本信息")]
    public string cropName = "新种子";

    [Header("生长设置")]
    public int growTimePerStage = 2; // 每个阶段需要的游戏分钟数

    [Header("生长阶段模型")]
    // 关键修复：使用数组存放不同阶段的模型（种子 -> 幼苗 -> 成熟）
    public GameObject[] growthStages;
}