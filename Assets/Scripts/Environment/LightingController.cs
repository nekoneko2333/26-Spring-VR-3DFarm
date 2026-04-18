using UnityEngine;

public class LightingController : MonoBehaviour
{
    [Header("光照引用")]
    public Light directionalLight;

    [Header("环境配置")]
    public Gradient directionalColor; // 在面板里调：日出橘红 -> 正午白 -> 日落深红 -> 夜晚蓝
    public Gradient ambientColor;     // 环境光颜色
    public AnimationCurve lightIntensity; // 光照强度曲线

    private void Start()
    {
        if (directionalLight == null) directionalLight = GetComponent<Light>();
        
        // 注册时间广播监听
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += UpdateLighting;
            // 初始化当前光照
            UpdateLighting(0); 
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged -= UpdateLighting;
        }
    }

    private void UpdateLighting(int totalMinutes)
    {
        // 获取当前时间占一天的百分比 (0.0 到 1.0)
        float hour = TimeManager.Instance.currentHour;
        float minute = TimeManager.Instance.currentMinute;
        float timePercent = (hour + (minute / 60f)) / 24f;

        // 旋转太阳：假设 0.25 (早上6点) 从地平线升起，0.75 (晚上18点) 落下
        float sunRotation = Mathf.Lerp(-90, 270, timePercent);
        directionalLight.transform.rotation = Quaternion.Euler(sunRotation, -30f, 0);

        // 更新颜色和强度
        directionalLight.color = directionalColor.Evaluate(timePercent);
        directionalLight.intensity = lightIntensity.Evaluate(timePercent);
        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
    }
}