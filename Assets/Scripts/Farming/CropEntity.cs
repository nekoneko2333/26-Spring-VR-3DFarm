using UnityEngine;
using System.Collections;


public class CropEntity : MonoBehaviour, ITimeObserver
{
    public CropData cropData;
    private int currentStageIndex = 0;
    private GameObject currentModel;

    private int minutesGrown = 0;
    private bool isWatered = false;
    private bool isDead = false;

    public void Initialize(CropData data)
    {
        cropData = data;
        currentStageIndex = 0;
        minutesGrown = 0;
        isDead = false;
        UpdateVisuals();

        if (TimeManager.Instance != null)
            TimeManager.Instance.OnMinuteChanged += OnMinuteChanged;
    }

    public void SetWatered(bool status)
    {
        if (isDead) return;
        isWatered = status;
    }

    public void Die()
    {
        isDead = true;
        isWatered = false;
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnMinuteChanged -= OnMinuteChanged;

        if (currentModel != null)
        {
            Renderer r = currentModel.GetComponentInChildren<Renderer>();
            if (r != null) r.material.color = Color.gray;
            currentModel.transform.localScale = new Vector3(1, 0.2f, 1);
        }
    }

    public void OnMinuteChanged(int totalMinutes)
    {
        if (IsMature() || !isWatered || isDead) return;

        minutesGrown++;
        if (minutesGrown >= cropData.growTimePerStage)
        {
            minutesGrown = 0;
            Grow();
        }
    }

    public void Grow()
    {
        if (currentStageIndex < cropData.growthStages.Length - 1)
        {
            currentStageIndex++;
            UpdateVisuals();
            // 新增：每次生长播放Q弹动画
            if (currentModel != null)
            {
                StartCoroutine(GrowthBounceEffect(currentModel.transform));
            }
        }
    }

    // 增加第一人称视觉反馈：生长时有果冻弹跳效果
    private IEnumerator GrowthBounceEffect(Transform targetTransform)
    {
        Vector3 originalScale = Vector3.one;
        float timer = 0f;
        float duration = 0.5f;

        while (timer < duration)
        {
            // 【新增安全检测】如果模型在中途被收割或销毁了，立刻终止动画！
            if (targetTransform == null) yield break;

            timer += Time.deltaTime;
            float scale = 1f + Mathf.Sin(timer / duration * Mathf.PI) * 0.4f;
            targetTransform.localScale = originalScale * scale;

            yield return null; // 等待下一帧
        }

        // 【新增安全检测】最后恢复大小时也检查一下
        if (targetTransform != null)
        {
            targetTransform.localScale = originalScale;
        }
    }

    public bool IsMature()
    {
        if (cropData == null) return false;
        return currentStageIndex >= cropData.growthStages.Length - 1;
    }

    private void UpdateVisuals()
    {
        if (currentModel != null) Destroy(currentModel);
        if (currentStageIndex < cropData.growthStages.Length)
        {
            GameObject prefabToSpawn = cropData.growthStages[currentStageIndex];
            if (prefabToSpawn != null)
            {
                currentModel = Instantiate(prefabToSpawn, transform.position, Quaternion.identity, transform);
                currentModel.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
            TimeManager.Instance.OnMinuteChanged -= OnMinuteChanged;
    }

    public void OnHourChanged(int currentHour) { }
    public void OnDayPassed(int currentDay) { }

}