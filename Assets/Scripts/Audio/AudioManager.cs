using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{

    [Header("-----Audio Source -----")]
    [SerializeField] AudioSource m_Music;
    [SerializeField] AudioSource m_Sound;

    [Header("-----Audio Clip -----")]
    public AudioClip _backgroundMenu;
    public AudioClip _backgroundGameplay;

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
