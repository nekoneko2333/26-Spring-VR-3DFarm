/* ==============================================================================
 * 【NPC 数据模板】 NPCData.cs
 * 负责人：同学 D (世界与社交)
 * ============================================================================== */

using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "FarmGame/NPC Data")]
public class NPCData : ScriptableObject
{
    [Header("基本信息 (Basic Info)")]
    [Tooltip("NPC 显示在对话框上的名字")]
    public string npcName;
    
    [Tooltip("UI 对话框里显示的 2D 头像图片")]
    public Sprite portrait; 

    [Header("对话文本 (Dialogue Pool)")]
    [TextArea(2, 5)]
    [Tooltip("NPC 平时会说的话。对话管理器每次会从这里面【随机抽取一句】展示")]
    public string[] defaultDialogues; 
    
    [Header("喜好系统 (Gift Preferences)")]
    [Tooltip("该 NPC 最喜欢的物品 ID (必须和同学C的物品数据库 ID 匹配，例如 'item_apple')")]
    public string lovedItemID; 
    
    [Tooltip("如果玩家送对了上面那个物品，增加多少好感度")]
    public int giftRewardFriendship = 20;

    // ==========================================
    // ✨ 新增字段：送礼反馈台词 (由同学 D 维护)
    // ==========================================
    
    [Header("送礼反馈 (Gift Feedback)")]
    [TextArea(2, 3)]
    [Tooltip("当玩家送对了最喜欢的物品 (lovedItemID) 时，NPC 说的台词")]
    public string loveGiftDialogue = "哇！你怎么知道我喜欢这个？太谢谢你了！";

    [TextArea(2, 3)]
    [Tooltip("当玩家送了普通物品时，NPC 说的台词")]
    public string normalGiftDialogue = "谢谢你的礼物，我会好好珍惜的。";

    // ==========================================
    // ✨ 新增字段：运行时状态
    // ==========================================
    
    [Header("运行时数据 (仅限调试)")]
    [Tooltip("当前 NPC 的好感度。注意：由于本项目不设存档，该值仅在单次游戏运行时有效")]
    public int friendshipPoints = 0;

    /// <summary>
    /// 重置 NPC 状态（建议在游戏启动或 GameManager 初始化时调用）
    /// </summary>
    public void ResetNPC()
    {
        friendshipPoints = 0;
    }
}