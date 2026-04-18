/* ==============================================================================
 * 【对话界面表现层】 DialogueUI.cs
 * 负责人：同学 C (经济与系统 UI)
 * * 核心功能：
 * 负责接收同学 D (DialogueManager) 传来的名字和台词，并把它们渲染到屏幕上。
 * * ------------------------------------------------------------------------------
 * 🛠️ 【UI 拼装与配置说明】 (同学 C 必看)：
 * 1. 在场景的 Canvas 下新建一个 Panel，命名为 "DialoguePanel"。
 * 2. 在 Panel 下新建两个 Text (或者 TextMeshPro)：
 * - 一个叫 "NameText"，用来显示 NPC 名字。
 * - 一个叫 "ContentText"，用来显示对话内容。
 * 3. 在 Panel 下新建一个 Button，叫 "CloseButton" (或者做一个全屏透明按钮用来点击继续)。
 * 4. 把这个 `DialogueUI.cs` 脚本挂到 Canvas 上。
 * 5. 把刚才建的 Panel、NameText、ContentText 拖进脚本对应槽位。
 * * ------------------------------------------------------------------------------
 * 🤝 【致命协同点】 (防卡死必看)：
 * ⚠️ 极其重要：
 * 选中你的 CloseButton，在 Inspector 里的 `On Click ()` 事件中，点击 "+" 号。
 * 把挂着 DialogueUI 的物体拖进去，在下拉菜单选择 `DialogueUI -> OnCloseButtonClicked`。
 * * 如果你不绑定这个按钮事件，玩家看完对话就关不掉窗口，同学 D 那边的状态锁也不会解开，
 * 整个游戏的交互就彻底卡死了！
 * ============================================================================== */

using UnityEngine;
using UnityEngine.UI; // 如果后续升级画面，强烈建议替换为 TMPro (TextMeshPro)

public class DialogueUI : MonoBehaviour
{
    // 单例模式，方便同学 D 的 DialogueManager 直接发号施令
    public static DialogueUI Instance { get; private set; }

    [Header("UI 组件引用 (请拖入 Canvas 里的对应物体)")]
    public GameObject dialoguePanel; // 整个对话框的父节点
    public Text nameText;            // 显示 NPC 名字的组件
    public Text contentText;         // 显示对话文本的组件

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