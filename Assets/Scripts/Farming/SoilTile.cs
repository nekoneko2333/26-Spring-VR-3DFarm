using UnityEngine;

// 注意这里：除了继承 MonoBehaviour，还要加上 , IInteractable
public class SoilTile : MonoBehaviour, IInteractable
{
    private bool isPlanted = false; // 这块地种东西了吗？

    // 必须实现接口规定的 Interact() 方法
    public void Interact()
    {
        if (!isPlanted)
        {
            // 同学 B 在这里写种地的逻辑
            Debug.Log("种下了一颗白菜！播放一个绿色的粒子特效！");
            isPlanted = true;

            // 可以通知同学 A：扣除玩家一点体力
            // 可以通知同学 C：背包里的白菜种子 -1
        }
        else
        {
            Debug.Log("这块地已经有东西了，不能再种了。");
        }
    }

    // 必须实现接口规定的 GetInteractPrompt() 方法
    public string GetInteractPrompt()
    {
        if (!isPlanted)
            return "按 E 播种";
        else
            return "已种植";
    }
}