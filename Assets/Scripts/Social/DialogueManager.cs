using UnityEngine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public bool isConversing { get; private set; }
    private Action onDialogueEndCallback;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartDialogue(NPCData npc, Action onComplete = null)
    {
        // --- 双重保险：如果商店开着，禁止开启对话逻辑 ---
        if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeInHierarchy)
            return;

        if (isConversing) return;
        
        isConversing = true;
        onDialogueEndCallback = onComplete;

        string textToShow = npc.defaultDialogues[UnityEngine.Random.Range(0, npc.defaultDialogues.Length)];
        
        // 关键点：调用 UI 显示
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowDialogue(npc.npcName, textToShow);
        }
    }

    public void EndDialogue()
    {
        if (!isConversing) return;

        isConversing = false;
        
        if (DialogueUI.Instance != null)
            DialogueUI.Instance.HideDialogue();

        if (onDialogueEndCallback != null)
        {
            onDialogueEndCallback.Invoke();
            onDialogueEndCallback = null; 
        }
    }
}