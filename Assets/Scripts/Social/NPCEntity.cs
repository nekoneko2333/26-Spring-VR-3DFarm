// Author: mcf
// Interactable NPC entry point for dialogue and merchant access.

using UnityEngine;

public class NPCEntity : MonoBehaviour, IInteractable
{
    [Header("NPC 数据 (必须拖入 ScriptableObject)")]
    public NPCData npcData;

    [Header("商人设定")]
    [Tooltip("勾选后，对话选项里会显示商店入口")]
    public bool isMerchant;

    [Header("对话朝向")]
    [Tooltip("对话开始时让 NPC 自动转向玩家")]
    public bool facePlayerOnInteract = true;

    [Tooltip("如果模型正面方向和 Transform.forward 不一致，可在这里补角度")]
    public float faceYawOffset = 0f;

    [Tooltip("转向玩家所需时间")]
    public float faceTurnDuration = 0.25f;

    [Tooltip("对话结束后转回原朝向所需时间")]
    public float returnTurnDuration = 0.25f;

    private Quaternion originalRotation;
    private bool hasStoredRotation;
    private Coroutine rotateRoutine;

    public string GetInteractPrompt()
    {
        if (npcData == null) return "未知 NPC";
        return isMerchant ? $"与 {npcData.npcName} 交易" : $"与 {npcData.npcName} 对话";
    }

    public void Interact()
    {
        if (npcData == null) return;

        if (ShopManager.Instance != null && ShopManager.Instance.shopPanel.activeInHierarchy)
        {
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.isConversing)
        {
            return;
        }

        if (DialogueManager.Instance != null)
        {
            FacePlayer();
            DialogueManager.Instance.StartDialogue(npcData, isMerchant, RestoreFacing);
        }
    }

    private void FacePlayer()
    {
        if (!facePlayerOnInteract) return;

        if (!TryGetPlayerFacingRotation(out Quaternion targetRotation)) return;

        originalRotation = transform.rotation;
        hasStoredRotation = true;
        RotateTo(targetRotation, faceTurnDuration);
    }

    private void RestoreFacing()
    {
        if (!hasStoredRotation) return;

        RotateTo(originalRotation, returnTurnDuration);
        hasStoredRotation = false;
    }

    private bool TryGetPlayerFacingRotation(out Quaternion rotation)
    {
        rotation = transform.rotation;

        Transform target = FindPlayerTarget();
        if (target == null) return false;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f) return false;

        rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0f, faceYawOffset, 0f);
        return true;
    }

    private void RotateTo(Quaternion targetRotation, float duration)
    {
        if (rotateRoutine != null)
        {
            StopCoroutine(rotateRoutine);
        }

        rotateRoutine = StartCoroutine(RotateRoutine(targetRotation, duration));
    }

    private System.Collections.IEnumerator RotateRoutine(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;

        if (duration <= 0f)
        {
            transform.rotation = targetRotation;
            rotateRoutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        rotateRoutine = null;
    }

    private Transform FindPlayerTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) return player.transform;

        if (Camera.main != null) return Camera.main.transform;

        return null;
    }
}
