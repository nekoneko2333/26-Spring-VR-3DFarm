// Author: mcf
// Updates sun rotation, light color, and ambient color from TimeManager.

using UnityEngine;

public class LightingController : MonoBehaviour
{
    [Header("光照引用")]
    public Light directionalLight;

    [Header("环境配置")]
    public Gradient directionalColor;
    public Gradient ambientColor;
    public AnimationCurve lightIntensity;

    private void Start()
    {
        if (directionalLight == null) directionalLight = GetComponent<Light>();

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnMinuteChanged += UpdateLighting;
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
        float hour = TimeManager.Instance.currentHour;
        float minute = TimeManager.Instance.currentMinute;
        float timePercent = (hour + (minute / 60f)) / 24f;

        float sunRotation = Mathf.Lerp(-90, 270, timePercent);
        directionalLight.transform.rotation = Quaternion.Euler(sunRotation, -30f, 0);

        directionalLight.color = directionalColor.Evaluate(timePercent);
        directionalLight.intensity = lightIntensity.Evaluate(timePercent);
        RenderSettings.ambientLight = ambientColor.Evaluate(timePercent);
    }
}
