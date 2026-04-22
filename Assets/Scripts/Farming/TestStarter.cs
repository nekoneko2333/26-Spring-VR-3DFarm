using UnityEngine;

public class TestStarter : MonoBehaviour
{
    [Header("你想在游戏开局拿到的种子")]
    public ItemData testSeed;

    [Header("发几个？")]
    public int seedAmount = 10;

    void Start()
    {
        // 游戏一开始，自动呼叫背包管家，给你发新手大礼包！
        if (InventoryManager.Instance != null && testSeed != null)
        {
            InventoryManager.Instance.AddItem(testSeed, seedAmount);
            Debug.Log($"【测试大礼包】已为您强行发放了 {seedAmount} 个 {testSeed.itemName}，祝您种田愉快！");
        }
        else
        {
            Debug.LogWarning("发礼包失败：找不到背包管理器，或者你没配置种子！");
        }
    }
}