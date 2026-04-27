using UnityEngine;

// 你的农业大管家（单例模式，全局唯一）
public class PlayerFarming : MonoBehaviour
{
    public static PlayerFarming Instance { get; private set; }

    [Header("农业数据库")]
    [Tooltip("把萝卜、南瓜的 CropData 都拖进这里，用作翻译字典")]
    public CropData[] allCropDatabase;

    [Header("当前装备的种子 (自动更新，不要手动填)")]
    public CropData currentEquippedCrop;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ==========================================================
    // 🌟 这是留给 C 同学的专属接口！他按按钮就会调用这里！
    // ==========================================================
    public void EquipItem(ItemData item)
    {
        if (item == null) return;

        currentEquippedCrop = null;

        // 查字典：把 C 同学的 ItemData 翻译成我们的 CropData
        foreach (CropData crop in allCropDatabase)
        {
            if (crop != null && crop.seedItem == item)
            {
                currentEquippedCrop = crop;
                break;
            }
        }

        if (currentEquippedCrop != null)
        {
            Debug.Log($"👨‍🌾【农业管家】成功装备种子：{currentEquippedCrop.cropName}！去点土地吧！");
        }
        else
        {
            Debug.LogWarning("👨‍🌾【农业管家】你装备的不是种子，或者没有在数据库里配置它！");
        }
    }
}