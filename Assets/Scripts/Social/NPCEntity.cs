using UnityEngine;

public class NPCEntity : MonoBehaviour, IInteractable
{
    [Header("NPC 数据 (必须拖入 ScriptableObject)")]
    public NPCData npcData;

    [Header("商人设定")]
    [Tooltip("勾选后，对话结束后会出现“进入商店”的选项")]
    public bool isMerchant;

    public string GetInteractPrompt()
    {
        if (npcData == null) return "未知 NPC"; 
        // 提示语现在根据身份自动切换
        return isMerchant ? $"与 {npcData.npcName} 交易" : $"与 {npcData.npcName} 对话";
    }

    public void Interact()
    {
        if (npcData == null) return;

        // --- 1. 基础拦截：如果商店面板开着，不做任何事 ---
        if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeInHierarchy)
        {
            return; 
        }

        // --- 2. 交互状态拦截 ---
        // 如果当前正在对话，直接返回。
        // 理由：对话中的“下一步（E键）”现在由 DialogueManager.cs 的 Update() 统一处理。
        // 如果这里不拦截，按一次 E 可能会同时触发这里的 Interact 和 Manager 里的下一步，导致对话直接跳过。
        if (DialogueManager.Instance != null && DialogueManager.Instance.isConversing)
        {
            return; 
        }

        // --- 3. 开启对话逻辑 ---
        if (DialogueManager.Instance != null)
        {
            // 调用新接口，传入 isMerchant 标记
            // 注意：我们移除了原来的 Action 回调，因为“开启商店”现在是对话内的一个分支选项，而不是自动开启。
            DialogueManager.Instance.StartDialogue(npcData, isMerchant);
        }

        // --- 4. 消耗时间 ---
        if (TimeManager.Instance != null)
            TimeManager.Instance.AddMinutes(isMerchant ? 5 : 8);
    }
}