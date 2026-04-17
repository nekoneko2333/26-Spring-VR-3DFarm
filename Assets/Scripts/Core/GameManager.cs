using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 单例模式：全游戏只有一个 GameManager
    public static GameManager Instance { get; private set; }

    // 游戏状态枚举
    public enum GameState
    {
        Playing,    // 正常游玩状态（可以走动、交互）
        UIOpen,     // 打开了商店/背包面板（锁死主角移动）
        Dialogue,   // 正在和 NPC 对话（锁死移动和交互）
        Cutscene    // 睡觉过场动画中黑屏
    }

    public GameState currentState = GameState.Playing;

    void Awake()
    {
        // 经典的单例绑定写法
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // 提供一个公开的方法来切换状态
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log("游戏状态切换为: " + newState);
    }
}