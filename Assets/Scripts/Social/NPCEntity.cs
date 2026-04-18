/* ==============================================================================
 * 【NPC 实体交互组件】 NPCEntity.cs
 * 负责人：同学 D (世界与社交)
 * * 核心功能：
 * 挂载在场景中的 NPC 模型上，让 NPC 具备被玩家“按 E 键交互”的能力。
 * 实现了同学 A 定义的 IInteractable 接口，并在交互时扣除 8 分钟游戏时间。
 * * ------------------------------------------------------------------------------
 * 🛠️ 【场景配置说明】 (负责摆放 NPC 的同学必看)：
 * 1. 在场景中选中你的 NPC 3D 模型。
 * 2. 给它挂上这个 `NPCEntity.cs` 脚本。
 * 3. ⚠️ 极其重要：给这个 NPC 添加一个【Collider 组件】(如 BoxCollider 或 CapsuleCollider)。
 * 如果不加碰撞体，同学 A 的玩家射线就“看”不到这个 NPC，无法触发交互！
 * 4. 在 Project 窗口右键 -> Create -> FarmGame -> NPC Data，创建一个 NPC 数据配置文件。
 * 5. 把这个配置好的数据文件拖进本脚本的 `Npc Data` 槽位中。
 * * ------------------------------------------------------------------------------
 * 🤝 【协作说明】 (同学 A 必看)：
 * 我已经完全适配了你的 `IInteractable` 接口。
 * 当玩家射线扫到 NPC 时，你可以调用 `GetInteractPrompt()` 在屏幕上显示“与 XXX 对话”。
 * 当玩家按下 E 键时，请直接调用 `Interact()`，对话 UI 弹出和时间快进均由我这边接管。
 * ============================================================================== */

using UnityEngine;

// 必须继承 MonoBehaviour 才能挂在物体上，必须实现 IInteractable 才能被玩家交互
public class NPCEntity : MonoBehaviour, IInteractable
{
    [Header("NPC 数据 (必须拖入 ScriptableObject)")]
    public NPCData npcData;

    /// <summary>
    /// 提供给同学 A 的 UI 提示接口
    /// 当玩家准星对准 NPC 时，屏幕上显示的文字
    /// </summary>
    public string GetInteractPrompt()
    {
        // 增加安全校验，防止场景里摆了 NPC 但忘了拖入数据时报错
        if (npcData == null) 
        {
            return "未知 NPC (请检查数据配置)"; 
        }
        
        return $"与 {npcData.npcName} 对话";
    }

    /// <summary>
    /// 提供给同学 A 的核心执行接口
    /// 当玩家对准 NPC 按下交互键 (E) 时触发
    /// </summary>
    public void Interact()
    {
        // 如果没配数据，直接终止，防止游戏崩溃报错
        if (npcData == null) 
        {
            Debug.LogError($"NPC {gameObject.name} 缺少 NPCData 配置！", gameObject);
            return;
        }

        // 1. 触发社交系统：调用对话管理器弹出 UI 并显示文本
        DialogueManager.Instance.StartDialogue(npcData);

        // 2. 触发时间系统：遵循 v2.7 计划书规范，每次对话耗时 8 游戏分钟
        TimeManager.Instance.AddMinutes(8);
        
        // 3. 触发音效系统：(可选) 播放一个 NPC 专属的打招呼音效
        // AudioManager.Instance.PlaySFX(npcData.greetingSound); 
    }
}