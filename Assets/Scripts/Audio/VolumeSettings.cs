using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundSlider;

    public static VolumeSettings Instance;

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
        LoadVolume();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume()
    {
        float volume = soundSlider.value;
        myMixer.SetFloat("Sound", Mathf.Log10(volume) * 20);
    }

    private void LoadVolume()
    {
        soundSlider.value = SaveSystem._instance._soundValue;
        musicSlider.value = SaveSystem._instance._musicValue;
        SetSoundVolume();
        SetMusicVolume();
    }

    public void SaveVolume()
    {
        SaveSystem._instance._musicValue = musicSlider.value;
        SaveSystem._instance._soundValue = soundSlider.value;
    }

}
