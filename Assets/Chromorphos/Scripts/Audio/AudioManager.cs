using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{

    [Header("-----Audio Source -----")]
    [SerializeField] AudioSource m_Music;
    [SerializeField] AudioSource m_Sound;

    [Header("-----MUSIC BACKGROUND -----")]
    public AudioClip _backgroundMenu;
    public AudioClip _backgroundGameplay;
    public AudioClip _pauseSFX;
    public AudioClip _BangerMenu;
    public AudioClip _BangerGameplay;

    
    [Header("-----PLAYER SFX -----")]
    public AudioClip _JumpSFX;
    public AudioClip _BouncySFX1;
    public AudioClip _BouncySFX2;
    public AudioClip _SeringuePlantée;
    public AudioClip _mistakeWord1;
    public AudioClip _mistakeWord2;
    public AudioClip _takeWord;

    public static AudioManager Instance;

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

    public void PlayBackground(AudioClip clip)
    {
        m_Music.clip = clip;
        m_Music.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        m_Sound.PlayOneShot(clip);
    }

}
