// Author: mcf
// Controls NPC dialogue, merchant entry, gift handling, and interaction time cost.

using System;
using UnityEngine;

public enum DialogueState { Talking, Selecting, Gifting }

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    public bool isConversing { get; private set; }

    private const int ShopOption = 1;
    private const int GiftOption = 2;
    private const int LeaveOption = 3;

    private Action onDialogueEndCallback;
    private DialogueState currentState;
    private NPCData currentNPC;
    private bool currentIsMerchant;
    private int pendingInteractionMinutes;
    private bool waitingForShopClose;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        CheckForShopClose();

        if (!isConversing) return;

        if (currentState == DialogueState.Talking)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ShowOptions();
            }
        }
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
        pendingInteractionMinutes = isMerchant ? 5 : 8;
        waitingForShopClose = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Dialogue);
        }

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
        if (index == ShopOption)
        {
            if (currentIsMerchant) OpenShopLogic();
            return;
        }

        if (index == GiftOption)
        {
            StartGifting();
            return;
        }

        if (index == LeaveOption)
        {
            EndDialogue();
        }
    }

    private void OpenShopLogic()
    {
        isConversing = false;
        waitingForShopClose = true;

        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.HideDialogue();
            DialogueUI.Instance.HideOptions();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.UIOpen);
        }

        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.OpenShop();
        }
        else
        {
            waitingForShopClose = false;
            CompleteInteraction();
        }
    }

    private void StartGifting()
    {
        currentState = DialogueState.Gifting;
        if (DialogueUI.Instance != null) DialogueUI.Instance.HideOptions();

        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.OpenForGifting(HandleGiftReceived);
        }
    }

    private void HandleGiftReceived(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            ShowOptions();
            return;
        }

        bool isLoved = item.itemID == currentNPC.lovedItemID;

        int basePoints = isLoved ? currentNPC.giftRewardFriendship : 5;
        int totalAdd = basePoints * amount;
        currentNPC.friendshipPoints += totalAdd;

        InventoryManager.Instance.RemoveItem(item, amount);

        currentState = DialogueState.Talking;
        string feedback = isLoved ? currentNPC.loveGiftDialogue : currentNPC.normalGiftDialogue;

        if (DialogueUI.Instance != null)
        {
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

        CompleteInteraction();
    }

    private void CheckForShopClose()
    {
        if (!waitingForShopClose) return;

        bool isShopOpen = ShopManager.Instance != null &&
                          ShopManager.Instance.shopPanel != null &&
                          ShopManager.Instance.shopPanel.activeInHierarchy;

        if (isShopOpen) return;

        waitingForShopClose = false;
        CompleteInteraction();
    }

    private void CompleteInteraction()
    {
        if (pendingInteractionMinutes > 0 && TimeManager.Instance != null)
        {
            TimeManager.Instance.AddMinutes(pendingInteractionMinutes);
        }

        pendingInteractionMinutes = 0;
        currentNPC = null;
        currentIsMerchant = false;

        if (onDialogueEndCallback != null)
        {
            onDialogueEndCallback.Invoke();
            onDialogueEndCallback = null;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        }
    }
}
