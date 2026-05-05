using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("交互设置")]
    [Tooltip("射线发射的起点，第一人称必须拖拽主角头里的 Main Camera")]
    public Transform rayOrigin;
    public float interactRange = 4f; // 稍微加长了一点点

    [Tooltip("非常重要！只检测这个图层的物体，避免射线打到自己或空气墙")]
    public LayerMask interactableLayer;

    // 缓存当前正在看的目标
    private IInteractable currentTarget;
    // 缓存当前正在看的土地（专门用来高亮）
    private SoilTile currentSoil;

    void Update()
    {
        CheckForInteractable();

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

        if (Physics.Raycast(ray, out hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                if (currentTarget != interactable)
                {
                    // 1. 如果之前看了别的地，先把它的高亮关掉
                    if (currentSoil != null) currentSoil.SetHighlight(false);

                    // 2. 锁定新目标
                    currentTarget = interactable;
                    currentSoil = hit.collider.GetComponent<SoilTile>();

                    // 3. 打开新地块的高亮
                    if (currentSoil != null) currentSoil.SetHighlight(true);

                    Debug.Log("UI显示提示：" + currentTarget.GetInteractPrompt());
                }
            }
        }
        else
        {
            // 如果射线没打中任何东西，或者视线移开了
            if (currentTarget != null)
            {
                // 关掉高亮
                if (currentSoil != null) currentSoil.SetHighlight(false);

                currentTarget = null;
                currentSoil = null;
                Debug.Log("移开视线，隐藏 UI 提示");
            }
        }
    }
}