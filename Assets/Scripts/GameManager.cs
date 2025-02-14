using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private string _actualScene;
    [SerializeField] private SceneAsset _scene;

    AudioManager _audioManager;
    Slider _musicSlider;
    Slider _soundSlider;

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
        _audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Nouvelle scène chargée : " + scene.name);
        _actualScene = scene.name;

        for (int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
        {
            if (_actualScene == SaveSystem._instance._levelData._level[i]._idLevel)
            {
                _scene = SaveSystem._instance._levelData._level[i]._scene;
            }
        }
        if (_scene != null)
        {
            _audioManager.PlayBackground(_audioManager._backgroundMenu);
        }
        else
        {
            _audioManager.PlayBackground(_audioManager._backgroundGameplay);
        }
        VolumeSettings.Instance.SaveVolume();
        _musicSlider = GameObject.FindGameObjectWithTag("musicSlider").GetComponent<Slider>();
        _soundSlider = GameObject.FindGameObjectWithTag("soundSlider").GetComponent<Slider>();
        VolumeSettings.Instance.musicSlider = _musicSlider;
        VolumeSettings.Instance.soundSlider = _soundSlider;
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

    private void FinishLevel()
    {

    }



    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
