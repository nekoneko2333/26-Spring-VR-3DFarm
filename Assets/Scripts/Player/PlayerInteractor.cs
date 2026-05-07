using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("交互设置")]
    public Transform rayOrigin;
    public float interactRange = 3f;
    public float checkRadius = 0.4f; // 增加判定体积

    [Tooltip("只检测选中的图层，请确保床和 Console 在此图层内")]
    public LayerMask interactableLayer;

    private IInteractable currentTarget;

    void Start()
    {
        // 自动防御：如果未分配 rayOrigin，尝试绑定主摄像机
        if (rayOrigin == null && Camera.main != null)
        {
            rayOrigin = Camera.main.transform;
        }
    }

    void Update()
    {
        // 状态锁：睡觉或剧情中停止交互检测
        if (GameManager.Instance != null && GameManager.Instance.currentState != GameManager.GameState.Playing)
        {
            ResetTarget();
            return;
        }

        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null)
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        if (rayOrigin == null) return;

        Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
        RaycastHit hit;

        // 使用 SphereCast 代替普通 Raycast 以获得更好的交互手感[cite: 2]
        if (Physics.SphereCast(ray, checkRadius, out hit, interactRange, interactableLayer))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                if (currentTarget != interactable)
                {
                    currentTarget = interactable;
                    Debug.Log("提示：" + currentTarget.GetInteractPrompt());
                }
                return;
            }
        }
        ResetTarget();
    }

    private void ResetTarget()
    {
        if (currentTarget != null)
        {
            currentTarget = null;
            Debug.Log("移开视线，隐藏 UI 提示");
        }
    }
}