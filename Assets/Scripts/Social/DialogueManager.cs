using UnityEngine;
using System;

// 定义对话状态
public enum DialogueState { Talking, Selecting, Gifting }

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public bool isConversing { get; private set; }
    
    private Action onDialogueEndCallback;
    private DialogueState currentState;
    private NPCData currentNPC;
    private bool currentIsMerchant;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (!isConversing) return;

        // 状态 1：正在看对话，按 E 进入选项界面
        if (currentState == DialogueState.Talking)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShowOptions();
            }
        }
        // 状态 2：正在选分支，支持 1, 2, 3 键快捷操作
        else if (currentState == DialogueState.Selecting)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) OnClickOption(1);
            if (Input.GetKeyDown(KeyCode.Alpha2)) OnClickOption(2);
            if (Input.GetKeyDown(KeyCode.Alpha3)) OnClickOption(3);
        }
    }

    public void StartDialogue(NPCData npc, bool isMerchant = false, Action onComplete = null)
    {
        if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeInHierarchy)
            return;

        if (isConversing) return;
        
        isConversing = true;
        currentState = DialogueState.Talking;
        currentNPC = npc;
        currentIsMerchant = isMerchant;
        onDialogueEndCallback = onComplete;

        // 随机挑选一句开场白
        string textToShow = npc.defaultDialogues[UnityEngine.Random.Range(0, npc.defaultDialogues.Length)];
        
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowDialogue(npc.npcName, textToShow);
            DialogueUI.Instance.HideOptions(); 
        }
    }

    private void ShowOptions()
    {
        currentState = DialogueState.Selecting;
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.ShowOptions(currentIsMerchant);
        }
    }

    public void OnClickOption(int index)
    {
        if (currentIsMerchant)
        {
            if (index == 1) OpenShopLogic(); // 商店
            else if (index == 2) StartGifting(); // 送礼
            else EndDialogue(); // 离开
        }
        else
        {
            if (index == 1) StartGifting(); // 送礼
            else if (index == 2) EndDialogue(); // 离开
        }
    }

    private void OpenShopLogic()
    {
        EndDialogue();
        if (ShopManager.Instance != null) ShopManager.Instance.OpenShop();
    }

    private void StartGifting()
    {
        currentState = DialogueState.Gifting;
        if (DialogueUI.Instance != null) DialogueUI.Instance.HideOptions();
        
        // --- 核心修改：回调现在接收两个参数 (物品, 数量) ---
        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.OpenForGifting(HandleGiftReceived);
        }
    }

    // 接收来自 InventoryUI 的数据
    private void HandleGiftReceived(ItemData item, int amount)
    {
        // 玩家取消送礼或数量非法
        if (item == null || amount <= 0) 
        {
            ShowOptions();
            return;
        }

        // 1. 判定喜好
        bool isLoved = (item.itemID == currentNPC.lovedItemID);
        
        // 2. 计算好感度增加值 (基础值 * 数量)
        int basePoints = isLoved ? currentNPC.giftRewardFriendship : 5;
        int totalAdd = basePoints * amount;
        currentNPC.friendshipPoints += totalAdd;

        // 3. 扣除物品数量
        InventoryManager.Instance.RemoveItem(item, amount);

        // 4. 显示 NPC 反馈文本
        currentState = DialogueState.Talking;
        string feedback = isLoved ? currentNPC.loveGiftDialogue : currentNPC.normalGiftDialogue;
        
        if (DialogueUI.Instance != null)
        {
            // 在对话框里额外提示好感度增加的具体数值，增强反馈感
            DialogueUI.Instance.ShowDialogue(currentNPC.npcName, $"{feedback}\n<size=80%><color=#FFA500>(好感度 +{totalAdd})</color></size>");
        }
        
        Debug.Log($"送礼成功：{item.itemName} x{amount}，当前好感度：{currentNPC.friendshipPoints}");
    }

    public void EndDialogue()
    {
        if (!isConversing) return;
        isConversing = false;
        
        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.HideDialogue();
            DialogueUI.Instance.HideOptions();
        }

        if (onDialogueEndCallback != null)
        {
            onDialogueEndCallback.Invoke();
            onDialogueEndCallback = null; 
        }
    }
}