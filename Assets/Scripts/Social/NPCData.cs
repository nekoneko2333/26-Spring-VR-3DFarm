// Author: mcf
// ScriptableObject data for NPC dialogue, portrait, and gift preference.

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
    [Tooltip("NPC 平时会说的话。对话管理器每次会随机抽取一句展示")]
    public string[] defaultDialogues;

    [Header("喜好系统 (Gift Preferences)")]
    [Tooltip("该 NPC 最喜欢的物品 ID，需和物品数据库 ID 匹配")]
    public string lovedItemID;

    [Tooltip("玩家送出最喜欢的物品时增加的好感度")]
    public int giftRewardFriendship = 20;

    [Header("送礼反馈 (Gift Feedback)")]
    [TextArea(2, 3)]
    public string loveGiftDialogue = "哇!你怎么知道我喜欢这个?太谢谢你了!";

    [TextArea(2, 3)]
    public string normalGiftDialogue = "谢谢你的礼物,我会好好珍惜的.";

    [Header("运行时数据 (仅限调试)")]
    [Tooltip("当前 NPC 的好感度。本项目不设存档，该值仅在单次运行中有效")]
    public int friendshipPoints = 0;

    public void ResetNPC()
    {
        friendshipPoints = 0;
    }
}
