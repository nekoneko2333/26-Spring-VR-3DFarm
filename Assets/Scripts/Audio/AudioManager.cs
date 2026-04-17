using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音轨设置")]
    public AudioSource bgmSource; // 专门放背景音乐
    public AudioSource sfxSource; // 专门放短促音效

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景时不要销毁它
        }
        else Destroy(gameObject);
    }

    // 播放背景音乐（比如切换到商店场景时调用）
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return;
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    // 播放单次音效（比如你挥锄头、买东西时调用）
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}