using UnityEngine;
using System.Collections.Generic;

public enum SoilState
{
    Barren,    // 荒地
    Tilled,    // 耕地（干）
    Planted,   // 已播种（干）
    PlantedWet, // 已播种（湿）
    Dead       // 植物死亡
}

[RequireComponent(typeof(AudioSource))] // 自动添加音效组件
public class SoilTile : MonoBehaviour, IInteractable, ITimeObserver
{
    public SoilState currentState = SoilState.Barren;

    [Header("视觉材质")]
    public Material barrenMat;
    public Material tilledMat;
    public Material plantedMat;
    public Material plantedWetMat;
    public Material deadMat;

    [Header("基础预制体")]
    public GameObject cropEntityPrefab;
    private MeshRenderer meshRenderer;
    private CropEntity currentCropInstance;
    public GameObject highlightBox;

    [Header("音效反馈")]
    private AudioSource audioSource;
    public AudioClip hoeSound;      // 锄地音效
    public AudioClip plantSound;    // 播种音效
    public AudioClip waterSound;    // 浇水音效
    public AudioClip harvestSound;  // 收获音效
    public AudioClip errorSound;    // 错误/拒绝音效

    [Header("视觉特效")]
    public GameObject hoeVFX;       // 飞溅的泥土
    public GameObject plantVFX;     // 闪烁的种子光芒
    public GameObject waterVFX;     // 水花四溅
    public GameObject harvestVFX;   // 爆金币/大丰收的光效

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        UpdateVisuals();
        if (TimeManager.Instance != null) TimeManager.Instance.OnDayPassed += OnDayPassed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) CheckBackpack();
        if (Input.GetKeyDown(KeyCode.G) && currentCropInstance != null) currentCropInstance.Grow();
    }

    // ==========================================
    // 核心动作：统统只按 E 键，严格按顺序执行
    // 顺序：荒地(锄地) -> 耕地(播种) -> 播种(浇水) -> 浇水后(等待成熟后收获)
    // ==========================================
    public void Interact()
    {
        switch (currentState)
        {
            case SoilState.Barren:
                // 步骤 1：锄地
                currentState = SoilState.Tilled;
                PlayFeedback(hoeSound, hoeVFX);
                Debug.Log("🟩【操作成功】你翻松了土地。下一步：按 E 播种。");
                break;

            case SoilState.Tilled:
                // 步骤 2：播种
                if (PlantCrop())
                {
                    PlayFeedback(plantSound, plantVFX);
                }
                else
                {
                    PlayFeedback(errorSound, null);
                }
                break;

            case SoilState.Planted:
                // 步骤 3：浇水
                currentState = SoilState.PlantedWet;
                if (currentCropInstance != null) currentCropInstance.SetWatered(true);
                PlayFeedback(waterSound, waterVFX);
                Debug.Log("💧【操作成功】你给种子浇了水！它开始生长了！等待成熟按 E 收获。");
                break;

            case SoilState.PlantedWet:
                // 步骤 4：检查并收获
                if (currentCropInstance != null)
                {
                    if (currentCropInstance.IsMature())
                    {
                        Harvest(); // 熟了，收割！
                        PlayFeedback(harvestSound, harvestVFX);
                    }
                    else
                    {
                        PlayFeedback(errorSound, null);
                        Debug.Log("⏳【提示】植物还没熟，耐心等它长大！(可按G催熟)");
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


    // 播放音效和特效的统一方法
    private void PlayFeedback(AudioClip clip, GameObject vfxPrefab)
    {
        if (clip != null) audioSource.PlayOneShot(clip);
        if (vfxPrefab != null)
        {
            // 在土地上方偏高一点的位置生成特效，更适合第一人称观看
            Vector3 spawnPos = transform.position + new Vector3(0, 0.5f, 0);
            GameObject vfx = Instantiate(vfxPrefab, spawnPos, Quaternion.identity);
            Destroy(vfx, 2f); // 2秒后自动销毁粒子
        }
    }

    // 内部逻辑：播种与收割
    private bool PlantCrop()
    {
        CropData seedToPlant = null;
        if (PlayerFarming.Instance != null) seedToPlant = PlayerFarming.Instance.currentEquippedCrop;

        if (seedToPlant == null || seedToPlant.seedItem == null)
        {
            Debug.LogWarning("❌手里没拿种子！");
            return false;
        }

        if (InventoryManager.Instance.RemoveItem(seedToPlant.seedItem, 1) == false)
        {
            Debug.Log("❌背包里种子用光了！");
            return false;
        }

        currentState = SoilState.Planted; // 严格设定为未浇水状态

        GameObject newCropObj = Instantiate(cropEntityPrefab, transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity);
        currentCropInstance = newCropObj.GetComponent<CropEntity>();
        currentCropInstance.Initialize(seedToPlant);
        currentCropInstance.SetWatered(false); // 强制未浇水

        Debug.Log($"🌱【播种成功】种下了 {seedToPlant.cropName}。请立刻按 E 浇水！");
        return true;
    }

    private void Harvest()
    {
        if (currentCropInstance.cropData.harvestItem != null)
        {
            InventoryManager.Instance.AddItem(currentCropInstance.cropData.harvestItem, 1);
            Debug.Log($"🌾【收割成功】获得 [{currentCropInstance.cropData.harvestItem.itemName}]！");
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


    // 查账工具：调用 C 同学的背包字典
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
        UpdateVisuals();
    }

    public void OnMinuteChanged(int totalMinutes) { }
    public void OnHourChanged(int currentHour) { }
    public string GetInteractPrompt() => "按 E 交互";

    private void UpdateVisuals()
    {
        if (meshRenderer == null) return;
        Material target = barrenMat;
        switch (currentState) { case SoilState.Tilled: target = tilledMat; break; case SoilState.Planted: target = plantedMat; break; case SoilState.PlantedWet: target = plantedWetMat; break; case SoilState.Dead: target = deadMat; break; }
        meshRenderer.material = target;
    }
    public void SetHighlight(bool isOn)
    {
        if (highlightBox != null)
        {
            highlightBox.SetActive(isOn);
        }
    }

}