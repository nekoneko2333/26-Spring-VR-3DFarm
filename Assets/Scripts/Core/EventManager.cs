using System; // 必须引入 System 才能使用 Action
using UnityEngine;

// 不需要继承 MonoBehaviour，不需要挂载到任何物体上！
// 这是一个纯静态（static）的公共广播站
public static class EventManager
{
    // ================= [ 时间相关事件 ] =================
    // 当新的一天开始时触发（环境/时间系统广播，农作物/动物监听）
    public static event Action OnNewDayStart;
    public static void TriggerNewDayStart()
    {
        OnNewDayStart?.Invoke();
    }

    // ================= [ 经济/背包相关事件 ] =================
    // 当金币发生变化时触发，传递变化后的总金币数（商店系统广播，UI系统监听刷新文字）
    public static event Action<int> OnMoneyChanged;
    public static void TriggerMoneyChanged(int newAmount)
    {
        OnMoneyChanged?.Invoke(newAmount);
    }

    // ================= [ 交互/UI相关事件 ] =================
    // 当玩家看向某个物体时，通知 UI 弹出提示文字（玩家广播，UI监听）
    public static event Action<string> OnShowInteractPrompt;
    public static void TriggerShowInteractPrompt(string text)
    {
        OnShowInteractPrompt?.Invoke(text);
    }

    public static event Action OnHideInteractPrompt;
    public static void TriggerHideInteractPrompt()
    {
        OnHideInteractPrompt?.Invoke();
    }
}