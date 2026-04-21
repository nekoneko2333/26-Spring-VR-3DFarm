using UnityEngine;

// 1. 定义土地状态
public enum SoilState
{
    Barren,    // 荒地
    Tilled,    // 耕地（干）
    TilledWet, // 耕地（湿）
    Planted,   // 已播种（干）
    PlantedWet, // 已播种（湿）
    Dead       // 植物死亡
}

// 2. 实现接口

public class SoilTile : MonoBehaviour, IInteractable, ITimeObserver
{
    public SoilState currentState = SoilState.Barren;

    [Header("视觉材质")]
    public Material barrenMat;
    public Material tilledMat;
    public Material tilledWetMat;
    public Material plantedMat;
    public Material plantedWetMat;
    public Material deadMat;

    [Header("当前选中的种子 (调试用)")]
    public CropData selectedSeed; // 你可以直接从 Inspector 拖入不同的 SO 来换种

    public GameObject cropEntityPrefab;
    private MeshRenderer meshRenderer;
    private CropEntity currentCropInstance; // 引用当前地里的作物

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
        if (TimeManager.Instance != null) TimeManager.Instance.OnDayPassed += OnDayPassed;
    }

    // --- 核心交互逻辑 ---
    public void Interact()
    {
        switch (currentState)
        {
            case SoilState.Barren:
                currentState = SoilState.Tilled;
                break;

            case SoilState.Tilled:
            case SoilState.TilledWet:
                PlantCrop();
                break;

            case SoilState.Planted:
            case SoilState.PlantedWet:
                if (currentCropInstance != null && currentCropInstance.IsMature())
                {
                    Harvest();
                }
                break;

            case SoilState.Dead:
                ClearSoil();
                break;
        }
        UpdateVisuals();
    }

    private void PlantCrop()
    {
        if (selectedSeed == null) { Debug.LogWarning("手里没种子！"); return; }

        bool wasWet = (currentState == SoilState.TilledWet);
        currentState = wasWet ? SoilState.PlantedWet : SoilState.Planted;

        GameObject newCropObj = Instantiate(cropEntityPrefab, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity, transform);
        newCropObj.transform.localScale = Vector3.one;

        currentCropInstance = newCropObj.GetComponent<CropEntity>();
        currentCropInstance.Initialize(selectedSeed);
        currentCropInstance.SetWatered(wasWet);

        Debug.Log($"种下了：{selectedSeed.cropName}");
    }

    private void Harvest()
    {
        Debug.Log($"收割了：{currentCropInstance.cropData.cropName}！金币+10 (占位)");
        Destroy(currentCropInstance.gameObject);
        currentCropInstance = null;
        currentState = SoilState.Barren;
    }

    private void ClearSoil()
    {
        if (currentCropInstance != null) Destroy(currentCropInstance.gameObject);
        currentCropInstance = null;
        currentState = SoilState.Barren;
    }

    public void WaterSoil()
    {
        if (currentState == SoilState.Tilled) currentState = SoilState.TilledWet;
        else if (currentState == SoilState.Planted)
        {
            currentState = SoilState.PlantedWet;
            if (currentCropInstance != null) currentCropInstance.SetWatered(true);
        }
        UpdateVisuals();
    }

    // --- 状态显示 (无需UI系统，直接在屏幕画字) ---
    void OnGUI()
    {
        // 获取土地在屏幕上的位置
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPos.z > 0) // 确保在镜头内
        {
            Rect rect = new Rect(screenPos.x - 50, Screen.height - screenPos.y - 50, 150, 100);
            string info = $"状态: {currentState}\n";
            if (currentCropInstance != null)
            {
                info += $"品种: {currentCropInstance.cropData.cropName}\n";
                info += currentCropInstance.IsMature() ? "<color=green>可收割!</color>" : "生长中...";
            }
            GUI.Label(rect, info);
        }
    }

    // --- 跨天逻辑保持不变 ---
    public void OnDayPassed(int currentDay)
    {
        if (currentState == SoilState.Planted)
        {
            currentState = SoilState.Dead;
            if (currentCropInstance != null) currentCropInstance.Die();
        }
        else if (currentState == SoilState.PlantedWet)
        {
            currentState = SoilState.Planted;
            if (currentCropInstance != null) currentCropInstance.SetWatered(false);
        }
        else if (currentState == SoilState.TilledWet) currentState = SoilState.Tilled;

        UpdateVisuals();
    }

    // 接口兼容
    public void OnMinuteChanged(int totalMinutes) { }
    public void OnHourChanged(int currentHour) { }
    public string GetInteractPrompt() => "交互";

    private void UpdateVisuals()
    {
        if (meshRenderer == null) return;
        Material target = barrenMat;
        switch (currentState)
        {
            case SoilState.Tilled: target = tilledMat; break;
            case SoilState.TilledWet: target = tilledWetMat; break;
            case SoilState.Planted: target = plantedMat; break;
            case SoilState.PlantedWet: target = plantedWetMat; break;
            case SoilState.Dead: target = deadMat; break;
        }
        meshRenderer.material = target;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) Interact();
        if (Input.GetMouseButtonDown(1)) WaterSoil();
    }
}