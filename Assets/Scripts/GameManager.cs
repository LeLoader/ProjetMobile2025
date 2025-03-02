using GooglePlayGames;
using NaughtyAttributes;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string _actualScene;
    [SerializeField] int targetFrameRate = 60;
    //[SerializeField] private SceneAsset _scene;

    public GameObject _canvaReglage;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        Stats.IncrementStat(Stats.STATS.APPLICATION_STARTED_COUNT);

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

    private void Start()
    {
        AudioManager.Instance.PlayBackground(AudioManager.Instance._backgroundMenu);
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
        if (sceneName == "--MENU--")
        {
            SceneManager.LoadScene(sceneName);
            _canvaReglage.SetActive(true);
        }

        else
        {
            for (int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
            {
                if (sceneName == "lastLevel")
                {
                    if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Unlock)
                    {
                        SceneManager.LoadScene(SaveSystem._instance._levelData._level[i]._idLevel);
                        AudioManager.Instance.PlayBackground(AudioManager.Instance._backgroundGameplay);
                        _canvaReglage.SetActive(false);
                        return;
                    }
                }
                else if (sceneName == "thisScene" && _actualScene == SaveSystem._instance._levelData._level[i]._idLevel)
                {
                    SceneManager.LoadScene(SaveSystem._instance._levelData._level[i]._idLevel);
                    _canvaReglage.SetActive(false);

                    Stats.IncrementStat(Stats.STATS.RESTART_COUNT);
                    return;
                }
                else if (sceneName == "nextLevel")
                {
                    if (_actualScene == SaveSystem._instance._levelData._level[i]._idLevel &&
                        SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Completed &&
                        (SaveSystem._instance._levelData._level[i + 1]._state == Level.LevelState.Unlock || SaveSystem._instance._levelData._level[i + 1]._state == Level.LevelState.Completed))
                    {
                        SceneManager.LoadScene(SaveSystem._instance._levelData._level[i + 1]._idLevel);
                        AchivementManager.UnlockAchivement(AchivementManager.FirstTry);
                        _canvaReglage.SetActive(false);
                        return;
                    }
                }
            }
            SceneManager.LoadScene(sceneName);
            AudioManager.Instance.PlayBackground(AudioManager.Instance._backgroundMenu);
            _canvaReglage.SetActive(false);
        }
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

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Play()
    {
        Time.timeScale = 1;
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
    //        Debug.Log("Sliders trouv�s et assign�s !");
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
    //        Debug.LogError("Les sliders ne sont pas trouv�s dans la sc�ne !");
    //    }
    //}
}
