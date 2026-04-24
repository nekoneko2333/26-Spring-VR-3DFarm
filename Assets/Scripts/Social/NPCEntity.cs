using UnityEngine;

public class NPCEntity : MonoBehaviour, IInteractable
{
    [Header("NPC 数据 (必须拖入 ScriptableObject)")]
    public NPCData npcData;

    [Header("商人设定")]
    [Tooltip("勾选后，交互将触发对话并在对话结束后开启商店")]
    public bool isMerchant;

    public string GetInteractPrompt()
    {
        if (npcData == null) return "未知 NPC"; 
        return isMerchant ? $"与 {npcData.npcName} 交易" : $"与 {npcData.npcName} 对话";
    }

    public void Interact()
    {
        if (npcData == null) return;

        // --- 核心漏洞修复 1：如果商店开着，按 E 键不准触发任何逻辑 ---
        if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeInHierarchy)
        {
            return; // 直接拦截，不让对话开启
        }

        // --- 情况 A：如果当前正在对话，这次按 E 就是为了“关闭对话” ---
        if (DialogueManager.Instance != null && DialogueManager.Instance.isConversing)
        {
            DialogueManager.Instance.EndDialogue(); 
            return; 
        }

        // --- 情况 B：如果当前没在对话，这次按 E 是为了“开启对话” ---
        if (DialogueManager.Instance != null)
        {
            if (isMerchant)
            {
                // 通过回调接口开启对话，接口没变，不影响别人
                DialogueManager.Instance.StartDialogue(npcData, () => 
                {
                    if (ShopManager.Instance != null) 
                        ShopManager.Instance.OpenShop();
                });
            }
            else
            {
                DialogueManager.Instance.StartDialogue(npcData);
            }
        }

        if (TimeManager.Instance != null)
            TimeManager.Instance.AddMinutes(isMerchant ? 5 : 8);
    }
}