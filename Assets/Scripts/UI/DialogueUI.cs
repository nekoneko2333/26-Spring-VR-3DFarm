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

        BindOptionButton(shopButton, 1);
        BindOptionButton(giftButton, isMerchant ? 2 : 1);
        BindOptionButton(leaveButton, isMerchant ? 3 : 2);
    }

    public void HideOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    private void BindOptionButton(Button button, int optionIndex)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => DialogueManager.Instance.OnClickOption(optionIndex));
    }
}
