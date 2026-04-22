using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI goldText;

    private void Start()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("场景中未找到 InventoryManager！请确保你已经把它挂载到了某个物体上。");
            return;
        }

        // 初始化显示
        UpdateGoldDisplay(InventoryManager.Instance.currentGold);

        // 订阅广播
        InventoryManager.Instance.OnGoldChanged += UpdateGoldDisplay;
    }

    private void OnDestroy()
    {
        // 销毁时取消订阅
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnGoldChanged -= UpdateGoldDisplay;
        }
    }

    private void UpdateGoldDisplay(int amount)
    {
        if (goldText != null)
        {
            // "N0" 会自动加千位符逗号，比如 1500 会显示为 1,500
            goldText.text = amount.ToString("N0");
        }
    }
}