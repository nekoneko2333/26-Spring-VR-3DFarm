// Author: mcf
// Central game-time controller. Broadcasts minute, hour, and day changes.

using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("时间设置 (Time Settings)")]
    [Tooltip("真实世界 1 秒对应多少游戏分钟")]
    public float gameMinutesPerRealSecond = 2.4f;

    [Header("当前时间 (Current Time)")]
    public int currentDay = 1;
    [Range(0, 23)] public int currentHour = 6;
    [Range(0, 59)] public int currentMinute = 0;

    public event Action<int> OnMinuteChanged;
    public event Action<int> OnHourChanged;
    public event Action<int> OnDayPassed;

    private float realTimer = 0f;

    private void Awake()
    {
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
        EventManager.OnPlayerSleep += SleepToNextMorning;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerSleep -= SleepToNextMorning;
    }

    private void Update()
    {
        if (ShouldPauseNaturalTime()) return;

        realTimer += Time.deltaTime;

        float realSecondsPerGameMinute = 1f / gameMinutesPerRealSecond;

        if (realTimer >= realSecondsPerGameMinute)
        {
            int minutesToAdd = Mathf.FloorToInt(realTimer / realSecondsPerGameMinute);
            realTimer -= minutesToAdd * realSecondsPerGameMinute;
            AddMinutes(minutesToAdd);
        }
    }

    private bool ShouldPauseNaturalTime()
    {
        if (GameManager.Instance == null) return false;

        return GameManager.Instance.currentState == GameManager.GameState.Dialogue
            || GameManager.Instance.currentState == GameManager.GameState.UIOpen
            || GameManager.Instance.currentState == GameManager.GameState.Cutscene;
    }

    public void AddMinutes(int amount)
    {
        currentMinute += amount;

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

            OnHourChanged?.Invoke(currentHour);
        }

        int totalMinutes = (currentDay * 24 * 60) + (currentHour * 60) + currentMinute;
        OnMinuteChanged?.Invoke(totalMinutes);
    }

    private void SleepToNextMorning()
    {
        int minutesLeftToday = (24 * 60) - (currentHour * 60 + currentMinute);
        int minutesToNextMorning = minutesLeftToday + 360;

        Debug.Log($"玩家睡觉，时间快进 {minutesToNextMorning} 分钟，直接到第 {currentDay + 1} 天早上 6:00");
        AddMinutes(minutesToNextMorning);
    }
}
