using UnityEngine;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [Header("目标设置")]
    public string targetSceneName;    // 目标场景的名字（需在 Build Settings 中添加）
    public Transform spawnPoint;      // 在目标场景中预设的出生点物体

    [Header("交互提示")]
    public string interactionPrompt = "进入房子";

    public void Interact()
    {
        if (spawnPoint == null)
        {
            Debug.LogWarning($"{gameObject.name} 未设置 spawnPoint！");
            return;
        }

        if (SceneTransitionManager.Instance != null)
        {
            // 调用管理器的转场逻辑
            SceneTransitionManager.Instance.TransitionToScene(targetSceneName, spawnPoint.position);
        }
    }

    public string GetInteractPrompt()
    {
        return interactionPrompt;
    }
}