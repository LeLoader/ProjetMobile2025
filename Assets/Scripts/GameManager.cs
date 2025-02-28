using GooglePlayGames;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string _actualScene;
    //[SerializeField] private SceneAsset _scene;

    private void Awake()
    {
        AchivementManager.AutomaticConnect();
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

    public void ManualConnect()
    {
        AchivementManager.ManualConnect();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _actualScene = scene.name;
    }

    public void ChangeScene(string sceneName)
    {
        for (int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
        {
            if (sceneName == "lastLevel")
            {
                if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Unlock)
                {
                    SceneManager.LoadScene(SaveSystem._instance._levelData._level[i]._idLevel);
                    return;
                }
            }

            else if (sceneName == "thisScene" && _actualScene == SaveSystem._instance._levelData._level[i]._idLevel)
            {
                SceneManager.LoadScene(SaveSystem._instance._levelData._level[i]._idLevel);
                return;
            }

            else if (sceneName == "nextLevel")
            {
                if (_actualScene == SaveSystem._instance._levelData._level[i]._idLevel &&
                    SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Completed &&
                    (SaveSystem._instance._levelData._level[i + 1]._state == Level.LevelState.Unlock || SaveSystem._instance._levelData._level[i + 1]._state == Level.LevelState.Completed))
                {
                    SceneManager.LoadScene(SaveSystem._instance._levelData._level[i + 1]._idLevel);
<<<<<<< HEAD
                    AchivementManager.UnlockAchivement(AchivementManager.FirstTry);
=======
>>>>>>> origin/LD
                    return;
                }
            }
        }
        SceneManager.LoadScene(sceneName);
    }

    public void FinishLevel()
    {
        for (int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
        {
            if (_actualScene == SaveSystem._instance._levelData._level[i]._idLevel)
            {
                SaveSystem._instance._levelData._level[i]._state = Level.LevelState.Completed;
                if (SaveSystem._instance._levelData._level[i + 1] != null && SaveSystem._instance._levelData._level[i + 1]._state == Level.LevelState.Blocked)
                {
                    SaveSystem._instance._levelData._level[i + 1]._state = Level.LevelState.Unlock;
                }
            }
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    //private System.Collections.IEnumerator DelayedFindAudioManager()
    //{
    //    yield return new WaitForSeconds(0.2f);

    //    _audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    //    if (_audioManager == null)
    //    {
    //        Debug.LogError("AudioManager introuvable !");
    //        yield break;
    //    }

    //    volumeSettings = _audioManager.GetComponentInChildren<VolumeSettings>();
    //    if (volumeSettings == null)
    //    {
    //        Debug.LogError("VolumeSettings introuvable sur AudioManager !");
    //        yield break;
    //    }

    //    _musicSlider = GameObject.FindGameObjectWithTag("musicSlider")?.GetComponent<Slider>();
    //    _soundSlider = GameObject.FindGameObjectWithTag("soundSlider")?.GetComponent<Slider>();

    //    volumeSettings.musicSlider = _musicSlider;
    //    volumeSettings.soundSlider = _soundSlider;


    //    if (_musicSlider != null && _soundSlider != null)
    //    {
    //        Debug.Log("Sliders trouvés et assignés !");
    //        _musicSlider.onValueChanged.RemoveAllListeners();
    //        _soundSlider.onValueChanged.RemoveAllListeners();

    //        //_musicSlider.onValueChanged.AddListener(volumeSettings.SetMusicVolume);
    //        //_soundSlider.onValueChanged.AddListener(volumeSettings.SetSoundVolume);
    //        Debug.Log("listener ajoutes GM");


    //        _musicSlider.value = volumeSettings.musicSlider.value;
    //        _soundSlider.value = volumeSettings.soundSlider.value;
    //    }
    //    else
    //    {
    //        Debug.LogError("Les sliders ne sont pas trouvés dans la scène !");
    //    }
    //}
}
