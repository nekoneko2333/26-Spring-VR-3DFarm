using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("UI 组件")]
    public CanvasGroup fadeCanvasGroup; // 拖入一个带有 CanvasGroup 的全屏黑色 UI
    public float fadeDuration = 1f;     // 渐变时间

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 核心：切场景不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 外部调用的主方法：传入场景名字，以及玩家到达新场景后的坐标
    public void TransitionToScene(string sceneName, Vector3 spawnPosition)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, spawnPosition));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Vector3 spawnPosition)
    {
        // 1. 锁死玩家操作，进入过场状态
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Cutscene);

        // 2. 屏幕变黑 (Fade Out)
        yield return StartCoroutine(Fade(1f));

        // 3. 异步加载场景
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }

        // 4. 场景加载完，把玩家瞬移到目标位置
        // 注意：这里需要你的玩家对象带有 "Player" 标签，或者通过单例访问
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // 暂时禁用控制器以防瞬移冲突
            CharacterController cc = player.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            player.transform.position = spawnPosition;

            if (cc) cc.enabled = true;
        }

        // 5. 屏幕变亮 (Fade In)
        yield return StartCoroutine(Fade(0f));

        // 6. 恢复游戏状态
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;
        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }
    }
}