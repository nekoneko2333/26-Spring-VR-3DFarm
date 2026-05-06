// Author: mcf
// UI facade for dialogue text and NPC option buttons.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI 组件引用 (基础对话文字)")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;

    [Header("UI 组件引用 (分支选项面板)")]
    public GameObject optionsPanel;
    public Button shopButton;
    public Button giftButton;
    public Button leaveButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        HideDialogue();
        HideOptions();

        if (shopButton != null) shopButton.onClick.AddListener(() => DialogueManager.Instance.OnClickOption(1));
        if (giftButton != null) giftButton.onClick.AddListener(() => DialogueManager.Instance.OnClickOption(2));
        if (leaveButton != null) leaveButton.onClick.AddListener(() => DialogueManager.Instance.OnClickOption(3));
    }

    public void ShowDialogue(string npcName, string content)
    {
        dialoguePanel.SetActive(true);
        nameText.text = npcName;
        contentText.text = content;
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public void OnCloseButtonClicked()
    {
        DialogueManager.Instance.EndDialogue();
    }

    public void ShowOptions(bool isMerchant)
    {
        optionsPanel.SetActive(true);
        if (shopButton != null) shopButton.gameObject.SetActive(isMerchant);
    }

    public void HideOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }
}
