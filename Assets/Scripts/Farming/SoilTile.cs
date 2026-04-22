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
    public CropData selectedSeed;

    public GameObject cropEntityPrefab;
    private MeshRenderer meshRenderer;
    private CropEntity currentCropInstance;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
        if (TimeManager.Instance != null) TimeManager.Instance.OnDayPassed += OnDayPassed;
    }

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

        if (selectedSeed.seedItem == null)
        {
            Debug.LogWarning("未配置种子ItemData！请联系同学C填入数据！");
            return;
        }

        // 呼叫同学 C 的背包系统：扣除种子
        if (InventoryManager.Instance.RemoveItem(selectedSeed.seedItem, 1) == false)
        {
            Debug.Log("背包里的种子不够了！无法种植！");
            return;
        }

        bool wasWet = (currentState == SoilState.TilledWet);
        currentState = wasWet ? SoilState.PlantedWet : SoilState.Planted;

        // ========== 核心修复：防止被压扁 ==========
        // 去掉了原代码最后的 `transform` 参数。让植物独立存在于场景中，不再做土地的儿子。
        // 这样土地就算压得再扁，也不会影响植物的形状了！
        GameObject newCropObj = Instantiate(cropEntityPrefab, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
        newCropObj.transform.localScale = Vector3.one;
        // =======================================

        currentCropInstance = newCropObj.GetComponent<CropEntity>();
        currentCropInstance.Initialize(selectedSeed);
        currentCropInstance.SetWatered(wasWet);

        Debug.Log($"种下了：{selectedSeed.cropName}，已从背包扣除 1 个种子");
    }

    private void Harvest()
    {
        if (currentCropInstance.cropData.harvestItem != null)
        {
            InventoryManager.Instance.AddItem(currentCropInstance.cropData.harvestItem, 1);
            Debug.Log($"收割了：{currentCropInstance.cropData.cropName}！已存入背包！");
        }
        else
        {
            Debug.LogWarning("未配置果实ItemData！收割了但没拿到东西！");
        }

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

    // （已经删除了那个又大又糊的 OnGUI 方法，现在屏幕上清爽了！）

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