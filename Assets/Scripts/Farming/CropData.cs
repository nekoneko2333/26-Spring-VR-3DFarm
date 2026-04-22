using UnityEngine;

// 允许在编辑器菜单创建数据文件
[CreateAssetMenu(fileName = "New Crop Data", menuName = "Farm/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("作物基本信息")]
    public string cropName = "新种子";

    // ========== 与同学 C 对接的核心数据 ==========
    [Header("对接背包系统 (让C同学填入他做的ItemData)")]
    public ItemData seedItem;      // 种地时要扣除的种子物品
    public ItemData harvestItem;   // 收割时发给玩家的果实物品
    // ==========================================

    [Header("生长设置")]
    public int growTimePerStage = 2; // 每个阶段需要的游戏分钟数

    [Header("生长阶段模型")]
    // 使用数组存放不同阶段的模型（种子 -> 幼苗 -> 成熟）
    public GameObject[] growthStages;
}