using UnityEngine;
using System.Collections.Generic;

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

    void Update()
    {
        // 开发者工具：按 I 键查看背包里到底有什么！
        if (Input.GetKeyDown(KeyCode.I))
        {
            CheckBackpack();
        }

        // 开发者工具：按 G 键催熟（方便测试收割）
        if (Input.GetKeyDown(KeyCode.G) && currentCropInstance != null)
        {
            currentCropInstance.Grow();
            Debug.Log("【神仙水】植物被强制催熟了！");
        }
    }

    // ==========================================
    // 动作 1：按左键 / E 键 (锄地、播种、收割)
    // ==========================================
    public void Interact()
    {
        switch (currentState)
        {
            case SoilState.Barren:
                currentState = SoilState.Tilled;
                Debug.Log("🟩【操作成功】你锄了地。下一步：请继续【左键】播种。");
                break;

            case SoilState.Tilled:
            case SoilState.TilledWet:
                PlantCrop();
                break;

            case SoilState.Planted:
                // 没浇水，绝对不让收割，并骂玩家
                Debug.Log("❌【操作错误】这颗种子快渴死了，它根本没在长！请先按【右键】浇水！");
                break;

            case SoilState.PlantedWet:
                // 浇了水，检查熟了没
                if (currentCropInstance != null)
                {
                    if (currentCropInstance.IsMature())
                    {
                        Harvest(); // 熟了，收割！
                    }
                    else
                    {
                        Debug.Log("⏳【提示】植物正在健康生长中，还没熟，请耐心等待！(测试可按G键催熟)");
                    }
                }
                break;

            case SoilState.Dead:
                ClearSoil();
                Debug.Log("🟫【提示】清理了枯萎的植物，土地重置。");
                break;
        }
        UpdateVisuals();
    }

    // ==========================================
    // 动作 2：按右键 (浇水专用)
    // ==========================================
    public void WaterSoil()
    {
        if (currentState == SoilState.Barren)
        {
            Debug.Log("❌【操作错误】这还是荒地！你浇什么水？请先按【左键】锄地！");
        }
        else if (currentState == SoilState.Tilled)
        {
            currentState = SoilState.TilledWet;
            Debug.Log("💧【操作成功】你提前润湿了土地。下一步：请按【左键】播种。");
        }
        else if (currentState == SoilState.Planted)
        {
            currentState = SoilState.PlantedWet;
            if (currentCropInstance != null) currentCropInstance.SetWatered(true);
            Debug.Log("💧【操作成功】你给植物浇了水！它现在开始生长了！");
        }
        else if (currentState == SoilState.TilledWet || currentState == SoilState.PlantedWet)
        {
            Debug.Log("⚠️【提示】地已经是湿的了，别浪费水啦！");
        }
        UpdateVisuals();
    }

    // ==========================================
    // 内部逻辑：播种与收割
    // ==========================================
    private void PlantCrop()
    {
        if (selectedSeed == null || selectedSeed.seedItem == null) return;

        // 向 C 的背包申请扣除种子
        if (InventoryManager.Instance.RemoveItem(selectedSeed.seedItem, 1) == false)
        {
            Debug.Log("❌【警告】你背包里没有种子了！无法播种！");
            return;
        }

        bool wasWet = (currentState == SoilState.TilledWet);
        currentState = wasWet ? SoilState.PlantedWet : SoilState.Planted;

        GameObject newCropObj = Instantiate(cropEntityPrefab, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
        newCropObj.transform.localScale = Vector3.one;

        currentCropInstance = newCropObj.GetComponent<CropEntity>();
        currentCropInstance.Initialize(selectedSeed);
        currentCropInstance.SetWatered(wasWet);

        string waterTip = wasWet ? "地是湿的，它会立刻开始生长！" : "❌警告：地是干的！请立刻【右键】浇水，否则长不大！";
        Debug.Log($"🌱【播种成功】种下了 {selectedSeed.cropName} (背包扣除1个)。{waterTip}");
    }

    private void Harvest()
    {
        if (currentCropInstance.cropData.harvestItem != null)
        {
            // 给 C 的背包发货
            InventoryManager.Instance.AddItem(currentCropInstance.cropData.harvestItem, 1);
            Debug.Log($"🌾【收割成功】获得了 1 个 [{currentCropInstance.cropData.harvestItem.itemName}]！已存入背包！(按 I 键可查账)");
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

    // ==========================================
    // 查账工具：调用 C 同学的背包字典
    // ==========================================
    private void CheckBackpack()
    {
        if (InventoryManager.Instance == null) return;

        // 获取 C 同学写的字典
        Dictionary<ItemData, int> dict = InventoryManager.Instance.GetInventoryDict();

        Debug.Log("========== 🎒 当前背包清单 ==========");
        if (dict.Count == 0)
        {
            Debug.Log("背包空空如也...");
        }
        else
        {
            foreach (var item in dict)
            {
                Debug.Log($"📦 物品：{item.Key.itemName}  |  数量：{item.Value} 个");
            }
        }
        Debug.Log($"💰 金币：{InventoryManager.Instance.currentGold}");
        Debug.Log("=====================================");
    }

    // 跨天逻辑及其他接口实现...
    public void OnDayPassed(int currentDay)
    {
        if (currentState == SoilState.Planted) { currentState = SoilState.Dead; if (currentCropInstance != null) currentCropInstance.Die(); }
        else if (currentState == SoilState.PlantedWet) { currentState = SoilState.Planted; if (currentCropInstance != null) currentCropInstance.SetWatered(false); }
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
        switch (currentState) { case SoilState.Tilled: target = tilledMat; break; case SoilState.TilledWet: target = tilledWetMat; break; case SoilState.Planted: target = plantedMat; break; case SoilState.PlantedWet: target = plantedWetMat; break; case SoilState.Dead: target = deadMat; break; }
        meshRenderer.material = target;
    }

    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) Interact();
        if (Input.GetMouseButtonDown(1)) WaterSoil();
    }
}