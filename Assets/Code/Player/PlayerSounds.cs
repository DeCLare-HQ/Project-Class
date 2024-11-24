using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public static PlayerSounds instance;

    [Header("오디오 소스")]
    private AudioSource audioSource;

    [Header("이동")]
    public AudioClip playerJumpSound;

    [Header("공격")]
    public AudioClip playerMeleeAttackSound1;
    public AudioClip playerMeleeAttackSound2;
    public AudioClip playerMeleeAttackSound3;
    public AudioClip playerRangeAttackSound;

    [Header("피격")]
    public AudioClip playerHitSound1;
    public AudioClip playerHitSound2;
    public AudioClip playerHitSound3;
    public AudioClip playerDeathSound;

    [Header("회복")]
    public AudioClip playerHealingSound;
    public AudioClip playerUsePotionSound;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 싱글톤 유지
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 사운드 재생 메서드
    public void PlayDeathSound()
    {
        PlaySound(playerDeathSound);
    }

    public void PlayHealingSound()
    {
        PlaySound(playerHealingSound);
    }

    public void PlayHitSound(int variation = 1)
    {
        switch (variation)
        {
            case 1:
                PlaySound(playerHitSound1);
                break;
            case 2:
                PlaySound(playerHitSound2);
                break;
            case 3:
                PlaySound(playerHitSound3);
                break;
            default:
                Debug.LogWarning("Invalid hit sound variation");
                break;
        }
    }

    public void PlayJumpSound()
    {
        PlaySound(playerJumpSound);
    }

    public void PlayMeleeAttackSound(int variation = 1)
    {
        switch (variation)
        {
            case 1:
                PlaySound(playerMeleeAttackSound1);
                break;
            case 2:
                PlaySound(playerMeleeAttackSound2);
                break;
            case 3:
                PlaySound(playerMeleeAttackSound3);
                break;
            default:
                Debug.LogWarning("Invalid melee attack sound variation");
                break;
        }
    }

    public void PlayRangeAttackSound()
    {
        PlaySound(playerRangeAttackSound);
    }

    public void PlayUsePotionSound()
    {
        PlaySound(playerUsePotionSound);
    }

    // 공통 사운드 재생 메서드
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioClip is not assigned.");
        }
    }
}
