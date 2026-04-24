
using UnityEngine;
using TMPro; 
using UnityEngine.UI; // 必须引入这个，才能识别 Button 组件

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI 组件引用 (基础对话文字)")]
    public GameObject dialoguePanel; 
    public TextMeshProUGUI nameText; 
    public TextMeshProUGUI contentText;

    [Header("UI 组件引用 (分支选项面板)")]
    public GameObject optionsPanel;  // 新增：拖入用来装三个按钮的父物体面板
    public Button shopButton;        // 新增：拖入商店按钮
    public Button giftButton;        // 新增：拖入送礼按钮
    public Button leaveButton;       // 新增：拖入离开按钮

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 游戏刚开始时，隐藏对话框和选项面板
        HideDialogue();
        HideOptions();

        // 【关键】用代码自动帮按钮绑定同学 D 需要的事件，省得你在 Unity 里手动拖拽
        if (shopButton != null) shopButton.onClick.AddListener(() => DialogueManager.Instance.OnClickOption(1));
        if (giftButton != null) giftButton.onClick.AddListener(() => DialogueManager.Instance.OnClickOption(2));
        if (leaveButton != null) leaveButton.onClick.AddListener(() => DialogueManager.Instance.OnClickOption(3));
    }

    /// <summary>
    /// 显示对话框文字
    /// </summary>
    public void ShowDialogue(string npcName, string content)
    {
        dialoguePanel.SetActive(true);
        nameText.text = npcName;
        contentText.text = content;
    }

    /// <summary>
    /// 隐藏整个对话框
    /// </summary>
    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// 老的关闭按钮逻辑 (如果以后不用这个按钮了，可以删掉)
    /// </summary>
    public void OnCloseButtonClicked()
    {
        DialogueManager.Instance.EndDialogue();
    }

    // ==========================================================
    // 下面是这次为了配合同学 D 新加的接口方法
    // ==========================================================

    /// <summary>
    /// 显示对话分支按钮面板
    /// </summary>
    /// <param name="isMerchant">是否是商人</param>
    public void ShowOptions(bool isMerchant)
    {
        optionsPanel.SetActive(true);
        // 如果是商人，激活商店按钮；如果是普通NPC，隐藏商店按钮
        if (shopButton != null) shopButton.gameObject.SetActive(isMerchant);
    }

    /// <summary>
    /// 隐藏对话分支按钮面板
    /// </summary>
    public void HideOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }
}