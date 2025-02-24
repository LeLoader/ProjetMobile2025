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
        StartCoroutine(DelayedBuildScene());
    }

    public void SetMusicVolume(float volume)
    {
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
    }

    public void SetSoundVolume(float volume)
    {
        myMixer.SetFloat("Sound", Mathf.Log10(volume) * 20);
    }

    public void LoadVolume()
    {
        soundSlider.value = SaveSystem._instance._soundValue;
        musicSlider.value = SaveSystem._instance._musicValue;
        soundSlider.onValueChanged.AddListener(SetSoundVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        SetSoundVolume(soundSlider.value);
        SetMusicVolume(musicSlider.value);
        Debug.Log("listener ajoutes VS");
    }

    private System.Collections.IEnumerator DelayedBuildScene()
    {
        yield return new WaitForSeconds(0.3f);
        LoadVolume();
    }

    public void SaveVolume()
    {
        SaveSystem._instance._musicValue = musicSlider.value;
        SaveSystem._instance._soundValue = soundSlider.value;
    }

}
