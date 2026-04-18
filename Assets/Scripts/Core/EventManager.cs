/* ==============================================================================
 * 【全局事件总线】 EventManager.cs
 * 负责人：同学 D (架构解耦与统筹)
 * * 核心功能：
 * 作为一个“中介”，让不同同学写的代码在互相不知道对方存在的情况下进行通信。
 * 避免脚本互相 GetComponent，减少耦合和报错。
 * * ?? 【场景配置说明】 (全组必看)：
 * ?? 注意：这是一个静态类 (static class)！
 * 绝对不要把这个脚本拖到场景里的任何物体上！它只要存在于 Scripts 文件夹里就能全剧生效。
 * * ? 【代码调用说明】 (A、B、C同学必看)：
 * * ? 场景一：你想【发通知】(比如同学A触发了睡觉)
 * 只需要一行代码：
 * EventManager.TriggerPlayerSleep();
 * * ? 场景二：你想【听通知】(比如同学C出货箱要在睡觉时结算)
 * 需要在你的脚本里写订阅和取消订阅：
 * private void OnEnable()  { EventManager.OnPlayerSleep += MySettleFunction; }
 * private void OnDisable() { EventManager.OnPlayerSleep -= MySettleFunction; }
 * ============================================================================== */

using System;
using UnityEngine;

public static class EventManager
{
    // ==========================================
    // ?? 睡眠跨天事件
    // ==========================================

    /// <summary>
    /// 事件：当玩家确认睡觉时触发。
    /// 订阅者：TimeManager(同学D)快进时间, ShippingBin(同学C)触发结算
    /// </summary>
    public static event Action OnPlayerSleep;

    /// <summary>
    /// 触发器：由 同学A 的 BedInteractable.cs 调用
    /// </summary>
    public static void TriggerPlayerSleep() 
    {
        OnPlayerSleep?.Invoke();
    }


    // ==========================================
    // ? 出货结算事件
    // ==========================================

    /// <summary>
    /// 事件：当出货箱结算完成，并将金币发放给玩家后触发。
    /// 参数 int：代表本次结算获得的总金币数。
    /// 订阅者：UIManager(同学C)弹出结算账单界面
    /// </summary>
    public static event Action<int> OnShippingSettled;

    /// <summary>
    /// 触发器：由 同学C 的 ShippingBin.cs 在结算完成后调用
    /// </summary>
    /// <param name="totalGold">本次卖出的总价钱</param>
    public static void TriggerShippingSettled(int totalGold) 
    {
        OnShippingSettled?.Invoke(totalGold);
    }
}