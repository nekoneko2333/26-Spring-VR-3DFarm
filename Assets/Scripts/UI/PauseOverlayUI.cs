using UnityEngine;

public class PauseOverlayUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject blockWhileActive;
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    private bool isPaused;
    private float previousTimeScale = 1f;

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (!isPaused && blockWhileActive != null && blockWhileActive.activeInHierarchy)
            {
                return;
            }

            SetPaused(!isPaused);
        }
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    private void SetPaused(bool paused)
    {
        if (isPaused == paused) return;

        isPaused = paused;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }

        if (isPaused)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = previousTimeScale;
        }
    }

    private void OnDestroy()
    {
        if (isPaused)
        {
            Time.timeScale = previousTimeScale;
        }
    }
}
