using UnityEngine;

// 这句代码非常神奇，它会在你 Unity 的右键菜单（Create）里加一个选项
[CreateAssetMenu(fileName = "New Item", menuName = "FarmGame/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基础信息")]
    public string itemID;        // 物品唯一ID（比如 "seed_cabbage"）
    public string itemName;      // 在 UI 上显示的中文名（"白菜种子"）
    [TextArea]
    public string description;   // 物品描述

    [Header("视觉表现")]
    public Sprite itemIcon;      // 在背包和商店里显示的 2D 图标
    public GameObject itemPrefab; // 如果它能被扔在地上，或者种在地里，它对应的 3D 模型预制体是什么

    [Header("经济价值")]
    public bool isSellable = true; // 能不能卖
    public int buyPrice;         // 商店卖多少钱
    public int sellPrice;        // 扔进出货箱能换多少钱

    [Header("物品类型")]
    public ItemType itemType;    // 使用枚举分类，方便商店和背包做筛选
}

// 定义物品类型的枚举
public enum ItemType
{
    Seed,       // 种子
    Crop,       // 农产品
    Animal,     // 动物（购买后生成在牧场）
    Tool,       // 工具（水壶、化肥等）
    Material    // 材料（木头、石头）
}