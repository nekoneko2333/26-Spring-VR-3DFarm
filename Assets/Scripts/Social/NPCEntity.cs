using UnityEngine;

public class NPCEntity : MonoBehaviour, IInteractable
{
    [Header("NPC 数据 (必须拖入 ScriptableObject)")]
    public NPCData npcData;

    [Header("商人设定")]
    [Tooltip("勾选后，对话选项里会显示商店入口")]
    public bool isMerchant;

    public string GetInteractPrompt()
    {
        if (npcData == null) return "未知 NPC";
        return isMerchant ? $"与 {npcData.npcName} 交易" : $"与 {npcData.npcName} 对话";
    }

    public void Interact()
    {
        if (npcData == null) return;

        if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeInHierarchy)
        {
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.isConversing)
        {
            return;
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.StartDialogue(npcData, isMerchant);
        }
    }
}
