/* ==============================================================================
 * 【NPC 数据模板】 NPCData.cs
 * 负责人：同学 D (世界与社交)
 * * * 核心功能：
 * 这是一个 ScriptableObject (独立数据资产)。
 * 它就像一张“NPC 身份证”，用来静态存储不同 NPC 的名字、头像、台词和喜好。
 * 使用这种架构，我们不需要改动任何代码，就能在游戏里快速生成 100 个不同的 NPC。
 * * ------------------------------------------------------------------------------
 * 🛠️ 【数据创建与配置说明】 (负责填数据的同学必看)：
 * ⚠️ 警告：这个脚本【绝对不能】拖给场景里的任何 GameObject！
 * * * 如何创建一个新的 NPC 数据？
 * 1. 在下方的 Project (项目) 窗口中，进入一个干净的文件夹（建议是 Assets/Data/NPCs/）。
 * 2. 在空白处 右键 -> Create -> FarmGame -> NPC Data。
 * 3. 此时会生成一个类似配置表的文件，把它重命名为你要做的 NPC 名字 (比如 "村长_Data")。
 * 4. 选中这个文件，在右侧 Inspector 面板中：
 * - 填入 NPC 的名字。
 * - 拖入一张 2D 图片作为头像。
 * - 在 Dialogues 数组里增加几句话（对话时会随机抽一句播放）。
 * 5. 最后！回到场景里，选中那个 NPC 的 3D 模型，把你刚才建好的配置文件，
 * 拖进它身上挂载的 `NPCEntity` 脚本的槽位里。
 * * ------------------------------------------------------------------------------
 * 🤝 【协作说明】 (同学 C 必看)：
 * 这里的 `lovedItemID` 是为了咱们之后 Phase 4 的“送礼系统”预留的。
 * 填写数据时，请确保这里填的字符串 ID，和同学 C 你的 `InventoryManager` 里的物品 ID 完美对应！
 * ============================================================================== */

using UnityEngine;

// 这个标签让你可以在 Unity 的右键菜单里直接创建这个数据文件
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
}