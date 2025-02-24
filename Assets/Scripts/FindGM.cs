using UnityEngine;
using UnityEngine.UI;

public class FindGM : MonoBehaviour
{
    public AudioManager _audioManager;
    public VolumeSettings _volumeSettings;
    public Slider _musicSlider;
    public Slider _soundSlider;


    private void Start()
    {
        FindGameManager();
    }

    public void FindGameManager()
    {
        _audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("AudioManager introuvable !");
        }

        _volumeSettings = _audioManager.GetComponentInChildren<VolumeSettings>();
        if (_volumeSettings == null)
        {
            Debug.LogError("VolumeSettings introuvable sur AudioManager !");
        }

        _musicSlider = GameObject.FindGameObjectWithTag("musicSlider")?.GetComponent<Slider>();
        _soundSlider = GameObject.FindGameObjectWithTag("soundSlider")?.GetComponent<Slider>();

        _volumeSettings.musicSlider = _musicSlider;
        _volumeSettings.soundSlider = _soundSlider;


        if (_musicSlider != null && _soundSlider != null)
        {
            Debug.Log("Sliders trouvés et assignés !");
            _musicSlider.onValueChanged.RemoveAllListeners();
            _soundSlider.onValueChanged.RemoveAllListeners();

            _musicSlider.onValueChanged.AddListener(_volumeSettings.SetMusicVolume);
            _soundSlider.onValueChanged.AddListener(_volumeSettings.SetSoundVolume);
            Debug.Log("listener ajoutes GM");


            _musicSlider.value = _volumeSettings.musicSlider.value;
            _soundSlider.value = _volumeSettings.soundSlider.value;
        }
        else
        {
            Debug.LogError("Les sliders ne sont pas trouvés dans la scène !");
        }
    }




}
