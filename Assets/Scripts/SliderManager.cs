using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private VolumeSettings _volumeSettings;

    public static SliderManager Instance;

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

    void Start()
    {
        _audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        _volumeSettings = _audioManager.GetComponentInChildren<VolumeSettings>();
    }
}