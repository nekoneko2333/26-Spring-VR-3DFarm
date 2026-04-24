using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("时间设置 (Time Settings)")]
    [Tooltip("真实世界1秒 = 游戏内多少分钟? (v2.7标准为2.4)")]
    public float gameMinutesPerRealSecond = 2.4f;

    [Header("当前时间 (Current Time)")]
    public int currentDay = 1;
    [Range(0, 23)] public int currentHour = 6; // 默认早上6点起
    [Range(0, 59)] public int currentMinute = 0;

    // 分级广播委托 (Action Events)
    public event Action<int> OnMinuteChanged;
    public event Action<int> OnHourChanged;
    public event Action<int> OnDayPassed;

    private float realTimer = 0f;

    private void Awake()
    {
        // 设置单例且跨场景不销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // 1. 订阅“玩家睡觉”事件
        // 当同学A的代码触发 EventManager.TriggerPlayerSleep() 时，这里就会收到通知
        EventManager.OnPlayerSleep += SleepToNextMorning;
    }

    private void OnDisable()
    {
        // 2. 取消订阅（防止场景销毁时内存泄漏）
        EventManager.OnPlayerSleep -= SleepToNextMorning;
    }

    private void Update()
    {
        // 只有这里允许使用 Time.deltaTime
        realTimer += Time.deltaTime;

        // 计算经过的真实时间是否足够推进游戏分钟
        float realSecondsPerGameMinute = 1f / gameMinutesPerRealSecond;
        
        if (realTimer >= realSecondsPerGameMinute)
        {
            int minutesToAdd = Mathf.FloorToInt(realTimer / realSecondsPerGameMinute);
            realTimer -= minutesToAdd * realSecondsPerGameMinute;
            AddMinutes(minutesToAdd);
        }
    }

    /// <summary>
    /// 核心方法：增加游戏时间 (供所有交互行为调用，如对话、睡觉)
    /// </summary>
    public void AddMinutes(int amount)
    {
        currentMinute += amount;

        // 处理进位逻辑
        while (currentMinute >= 60)
        {
            currentMinute -= 60;
            currentHour++;
            
            if (currentHour >= 24)
            {
                currentHour -= 24;
                currentDay++;
                OnDayPassed?.Invoke(currentDay);
            }
            // 触发小时广播
            OnHourChanged?.Invoke(currentHour);
        }

        // 触发分钟广播
        int totalMinutes = (currentDay * 24 * 60) + (currentHour * 60) + currentMinute;
        OnMinuteChanged?.Invoke(totalMinutes);
    }

    /// <summary>
    /// 处理玩家睡觉的逻辑，直接快进到第二天早上 6 点
    /// </summary>
    private void SleepToNextMorning()
    {
        // 计算今天还剩下多少分钟过完 (24:00)
        int minutesLeftToday = (24 * 60) - (currentHour * 60 + currentMinute);
        
        // 加上第二天早上到 6:00 的分钟数 (6 * 60 = 360分钟)
        int minutesToNextMorning = minutesLeftToday + 360;

        Debug.Log($"玩家睡觉，时间快进 {minutesToNextMorning} 分钟，直接到第 {currentDay + 1} 天早上 6:00");

        // 直接调用核心方法，它会自动处理进位并触发 OnDayPassed 和 OnHourChanged 广播！
        AddMinutes(minutesToNextMorning);
    }
}