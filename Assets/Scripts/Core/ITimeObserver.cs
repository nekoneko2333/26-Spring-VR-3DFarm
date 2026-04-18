/* ==============================================================================
 * 【时间监听接口】 ITimeObserver.cs
 * 负责人：同学 D
 * 作用：强制规范全组的时间调用。农场里所有需要随时间生长、产出的物体（作物、动物），
 * 必须继承这个接口，严禁自己写 Update 计时！
 * * 💻 【用法示例】 (同学 A、B 直接抄这段)：
 * * public class Crop : MonoBehaviour, ITimeObserver
 * {
 * void Start() {
 * // 1. 启动时订阅广播 (你想听哪个级别的时间就订阅哪个)
 * TimeManager.Instance.OnMinuteChanged += OnMinuteChanged;
 * }
 * * void OnDestroy() {
 * // 2. 销毁时必须取消订阅，防止报错
 * if (TimeManager.Instance != null)
 * TimeManager.Instance.OnMinuteChanged -= OnMinuteChanged;
 * }
 * * // 3. 实现接口方法，写你的生长逻辑
 * public void OnMinuteChanged(int totalMinutes) {
 * // 作物长大的代码写在这里...
 * }
 * * public void OnHourChanged(int currentHour) { }
 * public void OnDayPassed(int currentDay) { }
 * }
 * ============================================================================== */

using UnityEngine;

public interface ITimeObserver
{
    /// <summary>
    /// 每游戏分钟触发一次 (适合：作物累计生长时长、动物产出倒计时)
    /// </summary>
    /// <param name="totalMinutes">游戏开始到现在的总分钟数</param>
    void OnMinuteChanged(int totalMinutes);

    /// <summary>
    /// 每游戏小时触发一次 (适合：NPC行为树切换、状态刷新)
    /// </summary>
    /// <param name="currentHour">当前是几点 (0-23)</param>
    void OnHourChanged(int currentHour);

    /// <summary>
    /// 跨天时触发 (适合：出货箱统一结算、土地水分蒸发干涸)
    /// </summary>
    /// <param name="currentDay">当前是第几天</param>
    void OnDayPassed(int currentDay);
}