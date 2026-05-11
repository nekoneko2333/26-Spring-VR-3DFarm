using UnityEngine;

public class PressAnyKeyStartUI : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private bool pauseWhileVisible = true;

    private float previousTimeScale = 1f;
    private bool isShowing;

    private void Start()
    {
        if (startPanel == null)
        {
            startPanel = gameObject;
        }

        startPanel.SetActive(true);
        isShowing = true;

        if (pauseWhileVisible)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }
    }

    public void EnterGame()
    {
        if (!isShowing) return;

        isShowing = false;

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        if (pauseWhileVisible)
        {
            Time.timeScale = previousTimeScale;
        }

        enabled = false;
    }

    private void OnDestroy()
    {
        if (isShowing && pauseWhileVisible)
        {
            Time.timeScale = previousTimeScale;
        }
    }
}
