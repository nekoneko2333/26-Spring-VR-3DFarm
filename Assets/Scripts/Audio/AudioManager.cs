// Author: mcf
// Global BGM and SFX manager.

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音频播放器配置")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("背景音乐资源")]
    public AudioClip dayBGM;
    public AudioClip nightBGM;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourChanged += CheckAndSwitchBGM;
            CheckAndSwitchBGM(TimeManager.Instance.currentHour);
        }
        else
        {
            Debug.LogError("AudioManager 找不到 TimeManager，请确保场景中有 TimeManager！");
        }
    }

    private void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourChanged -= CheckAndSwitchBGM;
        }
    }

    private void CheckAndSwitchBGM(int currentHour)
    {
        AudioClip targetClip = (currentHour >= 6 && currentHour < 18) ? dayBGM : nightBGM;

        if (bgmSource.clip != targetClip)
        {
            bgmSource.clip = targetClip;
            bgmSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager 收到一个空的音效播放请求，检查你是否忘了在面板赋值！");
        }
    }
}
