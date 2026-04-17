using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [Tooltip("把你的主角拖到这里！")]
    public Transform target;

    [Header("视角设置")]
    [Tooltip("摄像机相对于主角的位置偏移量（X, Y, Z）")]
    // 典型的斜俯视角参数：Y轴拉高，Z轴往后退
    public Vector3 offset = new Vector3(0f, 10f, -8f);

    [Tooltip("摄像机跟上的速度，越大越生硬，越小越有电影感")]
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        // 如果没有目标，就什么都不做
        if (target == null) return;

        // 1. 计算摄像机应该在的理想位置
        Vector3 desiredPosition = target.position + offset;

        // 2. 使用 Lerp 平滑移动过去（自带镜头的丝滑跟随感）
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 3. 永远死死盯着主角看
        transform.LookAt(target);
    }
}