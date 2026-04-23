using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class MainMenuUI : MonoBehaviour
{
    [Header("按钮绑定")]
    public Button startButton; // 在面板里拖入你的“开始游戏”按钮

    [Header("场景对接配置 (留给同学A填写的接口)")]
    public string firstSceneName = "UI"; 

    private void Start()
    {
        // 游戏运行时，自动给开始按钮绑定点击事件
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartGameClicked);
        }
    }

    /// <summary>
    /// 当玩家点击“开始游戏”时触发
    /// </summary>
    private void OnStartGameClicked()
    {
        // 【已删除】禁用的代码去掉了！现在你可以随便点！

        Debug.Log($"[MainMenuUI] 准备进入场景：{firstSceneName}");

        // 加载目标场景
        SceneManager.LoadScene(firstSceneName);
    }
}