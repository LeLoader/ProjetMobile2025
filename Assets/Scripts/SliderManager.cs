using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private VolumeSettings _volumeSettings;

    void Start()
    {

        _audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        _volumeSettings = _audioManager.GetComponent<VolumeSettings>();

        if (_volumeSettings != null)
        {
            _musicSlider.onValueChanged.AddListener(_volumeSettings.SetMusicVolume);
            _soundSlider.onValueChanged.AddListener(_volumeSettings.SetSoundVolume);
            Debug.Log("listener ajoutes SM");
        }
        else
        {
            Debug.LogError("Script persistant introuvable !");
        }
    }
}