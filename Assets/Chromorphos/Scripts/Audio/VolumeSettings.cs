using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] public Slider musicSlider;
    [SerializeField] public Slider soundSlider;

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
        float volume = ConvertSliderToVolume(musicSlider.value);
        myMixer.SetFloat("Music", volume);
    }

    public void SetSoundVolume()
    {
        float volume = ConvertSliderToVolume(soundSlider.value);
        myMixer.SetFloat("Sound", volume);
    }

    private float ConvertSliderToVolume(float sliderValue)
    {
        if (sliderValue == 0)
            return -80f;

        return Mathf.Log10(sliderValue / 10f) * 20f;
    }

    public void LoadVolume()
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
