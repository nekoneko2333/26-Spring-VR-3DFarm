using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("UI 组件")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (fadeCanvasGroup != null)
            {
                fadeCanvasGroup.alpha = 0f;
                // --- 初始化时确保它不拦截点击 ---
                fadeCanvasGroup.blocksRaycasts = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string sceneName, Vector3 spawnPosition)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, spawnPosition));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Vector3 spawnPosition)
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Cutscene);

        // 2. 屏幕变黑
        yield return StartCoroutine(Fade(1f));

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            player.transform.position = spawnPosition;
            if (cc) cc.enabled = true;
        }

        // 5. 屏幕变亮
        yield return StartCoroutine(Fade(0f));

        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }

    public IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        // --- 核心逻辑 ---
        // 如果目标透明度 > 0 (说明要变黑)，则开启拦截，防止玩家在黑屏时乱点
        // 如果目标透明度 == 0 (说明变透明)，则关闭拦截，让玩家可以点到主 UI
        fadeCanvasGroup.blocksRaycasts = (targetAlpha > 0);

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;

        // 安全起见：如果最终是变透明，再次确保关闭拦截
        if (targetAlpha <= 0)
        {
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }
}