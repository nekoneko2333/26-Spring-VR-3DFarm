/* ==============================================================================
 * 【对话逻辑中心】 DialogueManager.cs
 * 负责人：同学 D (世界与社交)
 * * 核心功能：
 * 作为 NPC 数据和 UI 显示界面之间的“大脑”。
 * 负责抽取对话文本，并严格控制对话状态（防止玩家同时跟两个 NPC 说话导致 UI 卡死）。
 * * 🛠️ 【场景配置说明】 (场景搭建必看)：
 * 1. 在场景中新建一个空物体，命名为 "DialogueManager"。
 * 2. 把这个脚本挂载上去。
 * （注意：它只是个逻辑控制器，不包含具体的图片和文字框，UI 长什么样归 DialogueUI 管）
 * * 🤝 【协作说明】 (A、C 同学必看)：
 * - @同学 A (交互)：你不需要直接操心这个类，你的 NPCEntity 里的 Interact() 已经自动接通这里了。
 * - @同学 C (UI)：这个脚本会把 NPC 名字和台词发给你的 DialogueUI。
 * ⚠️ 关键配合：当玩家在你的 UI 上点击“结束对话”按钮，或者按 ESC 键时，
 * 请务必在你的 UI 脚本里调用一行代码：`DialogueManager.Instance.EndDialogue();`
 * 否则对话状态会被锁死，玩家就再也不能和其他人说话了！
 * ============================================================================== */

using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    // 单例模式，方便全组无缝调用
    public static DialogueManager Instance { get; private set; }

    [Tooltip("状态锁：当前是否正在对话中？防止连按 E 键重复触发")]
    public bool isConversing { get; private set; }

    private void Awake()
    {
        // 确保场景中只有一个对话管理器
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// 开启对话逻辑 (由 NPCEntity 触发)
    /// </summary>
    /// <param name="npc">传入被交互的 NPC 数据</param>
    public void StartDialogue(NPCData npc)
    {
        // 如果当前已经有对话框打开了，直接拦截，防止 UI 重叠或者逻辑崩坏
        if (isConversing) return;
        
        // 上锁
        isConversing = true;

        // 核心玩法：从该 NPC 配置的语录库中，随机抽取一句话
        string textToShow = npc.defaultDialogues[Random.Range(0, npc.defaultDialogues.Length)];
        
        // 通知同学 C 的 UI 系统，把名字和刚才抽取的台词显示到屏幕上
        DialogueUI.Instance.ShowDialogue(npc.npcName, textToShow);
    }

    /// <summary>
    /// 结束对话逻辑 (必须由同学 C 的 UI 脚本来触发)
    /// </summary>
    public void EndDialogue()
    {
        // 解锁，允许玩家进行下一次交互
        isConversing = false;
        
        // 通知 UI 系统隐藏对话面板
        DialogueUI.Instance.HideDialogue();
    }
}