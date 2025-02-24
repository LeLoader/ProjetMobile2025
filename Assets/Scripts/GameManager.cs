using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private string _actualScene;
    [SerializeField] private SceneAsset _scene;

    [SerializeField] private VolumeSettings volumeSettings;
    [SerializeField] AudioManager _audioManager;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _soundSlider;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Nouvelle scène chargée : " + scene.name);
        _actualScene = scene.name;

        StartCoroutine(DelayedFindAudioManager());
    }

    private System.Collections.IEnumerator DelayedFindAudioManager()
    {
        yield return new WaitForSeconds(0.2f);

        _audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        if (_audioManager == null)
        {
            Debug.LogError("AudioManager introuvable !");
            yield break;
        }
        
        volumeSettings = _audioManager.GetComponentInChildren<VolumeSettings>();
        if (volumeSettings == null)
        {
            Debug.LogError("VolumeSettings introuvable sur AudioManager !");
            yield break;
        }

        _musicSlider = GameObject.FindGameObjectWithTag("musicSlider")?.GetComponent<Slider>();
        _soundSlider = GameObject.FindGameObjectWithTag("soundSlider")?.GetComponent<Slider>();

        volumeSettings.musicSlider = _musicSlider;
        volumeSettings.soundSlider = _soundSlider;


        if (_musicSlider != null && _soundSlider != null)
        {
            Debug.Log("Sliders trouvés et assignés !");
            _musicSlider.onValueChanged.RemoveAllListeners();
            _soundSlider.onValueChanged.RemoveAllListeners();

            _musicSlider.onValueChanged.AddListener(volumeSettings.SetMusicVolume);
            _soundSlider.onValueChanged.AddListener(volumeSettings.SetSoundVolume);
            Debug.Log("listener ajoutes GM");


            _musicSlider.value = volumeSettings.musicSlider.value;
            _soundSlider.value = volumeSettings.soundSlider.value;
        }
        else
        {
            Debug.LogError("Les sliders ne sont pas trouvés dans la scène !");
        }
    }


    public void ChangeScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            for (int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
            {
                if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Blocked)
                {
                    SceneManager.LoadScene(SaveSystem._instance._levelData._level[i - 1]._idLevel);
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
