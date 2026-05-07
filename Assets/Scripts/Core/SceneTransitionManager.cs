using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("UI 组件")]
    public CanvasGroup fadeCanvasGroup; // 之前创建的包含黑屏和文字的 CanvasGroup
    public float fadeDuration = 1f;     // 渐变时间

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            if (fadeCanvasGroup != null) fadeCanvasGroup.alpha = 0f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 供外部调用：执行切场景转场
    public void TransitionToScene(string sceneName, Vector3 spawnPosition)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, spawnPosition));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Vector3 spawnPosition)
    {
        // 1. 锁定玩家操作
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Cutscene);

        // 2. 屏幕变黑
        yield return StartCoroutine(Fade(1f));

        // 3. 异步加载场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        // 4. 场景加载完，瞬移玩家
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false; // 禁用控制器防止坐标修正失败

            player.transform.position = spawnPosition;

            if (cc) cc.enabled = true;
        }

        // 5. 屏幕变亮
        yield return StartCoroutine(Fade(0f));

        // 6. 恢复游戏状态
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }

    // 改为 public，方便单独调用渐变效果
    public IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null) yield break;

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = targetAlpha;
    }
}