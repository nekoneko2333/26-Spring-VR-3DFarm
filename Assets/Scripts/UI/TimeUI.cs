using UnityEngine;
using TMPro;

public class TimeUI : MonoBehaviour
{
    [Header("UI 引用")]
    public RectTransform rotatingIcon; // 你的四个圆圈交叉图
    public TextMeshProUGUI hourText;   // 你的时间文本 (TextMeshPro)

    private void Start()
    {
        if (TimeManager.Instance == null) return;

        // 初始化显示
        RefreshUI();

        TimeManager.Instance.OnMinuteChanged += HandleTimeUpdate;
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged -= HandleTimeUpdate;
        }
    }

    private void HandleTimeUpdate(int totalMinutes)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        int hour = TimeManager.Instance.currentHour;
        int minute = TimeManager.Instance.currentMinute;

        // 1. 更新文本显示：格式为 "数字:数字" (例如 06:00)
        if (hourText != null)
        {
            hourText.text = string.Format("{0:D2}:{1:D2}", hour, minute);
        }

        // 2. 更新持续旋转逻辑 (修正为逆时针旋转)
        if (rotatingIcon != null)
        {
            // 计算当前总分钟数
            int totalMinutesThisDay = hour * 60 + minute;
            
            // 计算距离早上 6:00 的偏移量
            int minutesSince6AM = (totalMinutesThisDay - 360 + 1440) % 1440;

            float targetAngle = minutesSince6AM * 0.25f;

            rotatingIcon.localRotation = Quaternion.Euler(0, 0, targetAngle);
        }
    }
}