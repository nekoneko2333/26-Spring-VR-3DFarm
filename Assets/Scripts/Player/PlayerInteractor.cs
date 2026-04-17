using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("交互设置")]
    [Tooltip("射线发射的起点，通常拖拽主角的 Camera 或主角自己的 Transform")]
    public Transform rayOrigin;
    public float interactRange = 3f; // 射线长度（主角手有多长）

    [Tooltip("非常重要！只检测这个图层的物体，避免射线打到自己或空气墙")]
    public LayerMask interactableLayer;

    // 缓存当前正在看的目标
    private IInteractable currentTarget;

    void Update()
    {
        // 第一步：每帧检测前方是否有东西
        CheckForInteractable();

        // 第二步：如果按下了 E 键，并且当前有目标，就触发目标自己的交互逻辑
        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        // 从指定起点，向正前方发射射线
        Debug.DrawRay(rayOrigin.position, rayOrigin.forward * interactRange, Color.red);
        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        // 如果射线打中了属于 Interactable Layer 的物体
        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            // 尝试获取该物体上是否继承了 IInteractable 接口的脚本
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                // 如果看到了新东西
                if (currentTarget != interactable)
                {
                    currentTarget = interactable;
                    // 【通知UI显示】这里暂时用 Debug 代替，后续让负责 UI 的同学来接管
                    Debug.Log("UI显示提示：" + currentTarget.GetInteractPrompt());
                }
            }
        }
        else
        {
            // 如果射线没打中任何东西，或者打中的东西不能交互
            if (currentTarget != null)
            {
                currentTarget = null;
                // 【通知UI隐藏】
                Debug.Log("移开视线，隐藏 UI 提示");
            }
        }
    }
}