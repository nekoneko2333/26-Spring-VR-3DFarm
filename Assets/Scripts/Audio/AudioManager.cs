/* ==============================================================================
 * 【全局音频管理器】 AudioManager.cs
 * 负责人：同学 D
 * * 核心功能：
 * 1. 监听 TimeManager 的小时广播，自动进行背景音乐(BGM)的日夜切换。
 * 2. 提供全组统一的音效(SFX)播放接口，支持多音效完美重叠播放。
 * * ------------------------------------------------------------------------------
 * ?? 【场景配置说明】 (给负责搭建场景的同学看)：
 * 1. 在 Hierarchy 场景面板中新建一个空物体，命名为 "AudioManager"。
 * 2. 给它挂上这个 AudioManager.cs 脚本。
 * 3. 给这个空物体添加 2 个 [Audio Source] 组件：
 * - 第 1 个 Audio Source：勾选 [Play On Awake] 和 [Loop]，拖给脚本面板的 Bgm Source。
 * - 第 2 个 Audio Source：取消勾选 [Play On Awake] 和 [Loop]，拖给脚本面板的 Sfx Source。
 * 4. 把白天和黑夜的两首音乐(.mp3/.wav)拖入 Day BGM 和 Night BGM 槽位。
 * * ------------------------------------------------------------------------------
 * ? 【代码调用说明】 (A、B、C同学必看)：
 * ?? 警告：请不要在你们自己的模块里新建 AudioSource，统一调用这个接口！
 * * 用法只需两步：
 * 第一步：在你的脚本里声明一个公开变量，并在 Unity 面板里把声音文件拖给它。
 * public AudioClip mySound; 
 * * 第二步：在你需要发声的地方（比如挥锄头、买东西、按按钮），加上这句代码：
 * AudioManager.Instance.PlaySFX(mySound);
 * ============================================================================== */

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // 单例模式，方便全组无缝调用
    public static AudioManager Instance { get; private set; }

    [Header("音频播放器配置 (请按头部注释拖入对应组件)")]
    public AudioSource bgmSource; // 循环播放的BGM
    public AudioSource sfxSource; // 单次播放的短促音效

    [Header("背景音乐资源")]
    public AudioClip dayBGM;   // 白天音乐 (06:00 - 18:00)
    public AudioClip nightBGM; // 夜晚音乐 (18:00 - 06:00)

    private void Awake()
    {
        // 确保场景中只有一个 AudioManager 单例
        if (Instance == null) 
        {
            Instance = this;
            // 因为我们是一次性无存档游戏，如果跨场景不想断音乐，可以解开下面这行：
            // DontDestroyOnLoad(gameObject); 
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 游戏启动时，确保 TimeManager 存在并注册时间监听
        if (TimeManager.Instance != null)
        {
            // 订阅同学D自己的“小时广播”，每过一小时检查一次是否需要切歌
            TimeManager.Instance.OnHourChanged += CheckAndSwitchBGM;
            
            // 刚进入游戏时，手动初始化一次背景音乐
            CheckAndSwitchBGM(TimeManager.Instance.currentHour); 
        }
        else
        {
            Debug.LogError("AudioManager 找不到 TimeManager，请确保场景中有 TimeManager！");
        }
    }

    private void OnDestroy()
    {
        // 脚本销毁或切场景时必须取消订阅，防止内存泄漏
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourChanged -= CheckAndSwitchBGM;
        }
    }

    /// <summary>
    /// 根据当前时间自动切换背景音乐
    /// </summary>
    private void CheckAndSwitchBGM(int currentHour)
    {
        // 核心逻辑：6点到17点用白天，其余用黑夜音乐
        AudioClip targetClip = (currentHour >= 6 && currentHour < 18) ? dayBGM : nightBGM;
        
        // 如果当前播放的音乐和目标音乐不同，才进行切换（防止每小时音乐都被强制重置）
        if (bgmSource.clip != targetClip)
        {
            bgmSource.clip = targetClip;
            bgmSource.Play(); 
        }
    }

    /// <summary>
    /// 全组统一使用的音效播放接口
    /// 无论调用多少次，声音都会重叠播放，不会互相掐断
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            // PlayOneShot 是关键：它允许同一个 AudioSource 同时播放多个短音效
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager 收到一个空的音效播放请求，检查你是否忘了在面板赋值！");
        }
    }
}