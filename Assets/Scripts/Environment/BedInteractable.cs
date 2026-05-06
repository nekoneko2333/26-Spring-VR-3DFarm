using UnityEngine;
using System.Collections;
using TMPro; // 如果你使用的是 TextMeshPro，请加上这行

public class BedInteractable : MonoBehaviour, IInteractable
{
    [Header("UI 引用")]
    public GameObject dayTextObject; // 拖入刚才创建的 DayDisplayText 物体

    [Header("计时设置")]
    public float textDisplayDuration = 2.0f; // 文字显示多久

    public void Interact()
    {
        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        // 1. 锁定玩家
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Cutscene);

        // 2. 屏幕渐变为全黑
        if (SceneTransitionManager.Instance != null)
            yield return SceneTransitionManager.Instance.StartCoroutine("Fade", 1f);

        // ---------------- 开始黑屏阶段 ----------------

        // 3. 执行时间跳转逻辑（此时玩家看不见）
        EventManager.TriggerPlayerSleep();

        // 4. 显示“到达第二天”的文字
        if (dayTextObject != null)
        {
            // 如果需要动态改变天数，可以在这里获取 TimeManager 的数据
            // int nextDay = TimeManager.Instance.currentDay; 
            // dayTextObject.GetComponent<TMP_Text>().text = "第 " + nextDay + " 天";

            dayTextObject.SetActive(true); // 显示文字
            Debug.Log("屏幕中心显示：到达第二天");
        }

        // 5. 让玩家盯着黑屏上的字看一会儿
        yield return new WaitForSeconds(textDisplayDuration);

        // 6. 隐藏文字，准备亮起
        if (dayTextObject != null)
            dayTextObject.SetActive(false);

        // ---------------- 结束黑屏阶段 ----------------

        // 7. 屏幕恢复亮度
        if (SceneTransitionManager.Instance != null)
            yield return SceneTransitionManager.Instance.StartCoroutine("Fade", 0f);

        // 8. 恢复玩家操作
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);

        Debug.Log("☀️ 醒来，新的一天开始！");
    }

    public string GetInteractPrompt() => "休息并结算 (跳至次日 06:00)";
}