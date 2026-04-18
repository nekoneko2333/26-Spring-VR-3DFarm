/* ==============================================================================
 * 【对话界面表现层】 DialogueUI.cs (TextMeshPro 升级版)
 * 负责人：同学 C (经济与系统 UI)
 * * 核心功能：
 * 负责接收同学 D (DialogueManager) 传来的名字和台词，并把它们渲染到屏幕上。
 * ============================================================================== */

using UnityEngine;
using TMPro; // 引入 TextMeshPro 命名空间

public class DialogueUI : MonoBehaviour
{
    // 单例模式，方便同学 D 的 DialogueManager 直接发号施令
    public static DialogueUI Instance { get; private set; }

    [Header("UI 组件引用 (请拖入 Canvas 里的对应物体)")]
    public GameObject dialoguePanel; 
    
    // 把原来的 Text 替换成了 TextMeshProUGUI
    public TextMeshProUGUI nameText;     // 显示 NPC 名字的组件
    public TextMeshProUGUI contentText;  // 显示对话文本的组件

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 游戏刚开始时，确保对话框是隐藏状态
        HideDialogue();
    }

    /// <summary>
    /// 显示对话框 (由同学 D 的 DialogueManager 自动调用，你不用管)
    /// </summary>
    public void ShowDialogue(string npcName, string content)
    {
        dialoguePanel.SetActive(true);
        nameText.text = npcName;
        contentText.text = content;
    }

    /// <summary>
    /// 隐藏对话框
    /// </summary>
    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// 【重点】UI 按钮点击事件
    /// 必须绑定到你的“关闭/继续”按钮上！
    /// </summary>
    public void OnCloseButtonClicked()
    {
        // 这行代码是全村的希望，它负责通知同学 D：“玩家看完了，把状态锁解开吧！”
        DialogueManager.Instance.EndDialogue();
    }
}