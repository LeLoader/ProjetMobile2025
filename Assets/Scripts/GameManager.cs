using GooglePlayGames;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string _actualScene;
    //[SerializeField] private SceneAsset _scene;

    public GameObject _canvaReglage;

    private void Awake()
    {
        //AchivementManager.AutomaticConnect();
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
        //AchivementManager.ManualConnect();

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _actualScene = scene.name;
        if (scene.name == "--MENU--")
        {
            AudioManager.Instance.PlayBackground(AudioManager.Instance._backgroundMenu);
            _canvaReglage.SetActive(true);
            VerifyCompleted.Instance.Verify();
        }
        else
        {
            _canvaReglage.SetActive(false);
        }
    }

    public void ChangeScene(string sceneName)
    {
        Level currentLevel = GetCurrentLevel();

        if (currentLevel == null && _actualScene != "--MENU--")
        {
            Debug.Log("marche pas");
            return;
        }
        else if (sceneName == "thisScene")
        {
            SceneManager.LoadScene(currentLevel._idLevel);
            return;
        }
        else if (sceneName == "nextLevel")
        {
            Level nextLevel = GetNextLevel(currentLevel);

            if (nextLevel != null)
            {
                SceneManager.LoadScene(nextLevel._idLevel);
                return;
            }
        }
        else if (sceneName == "lastLevel")
        {
            string lastUnlockedLevel = GetLastUnlockedLevel();

            if (lastUnlockedLevel != null)
            {
                SceneManager.LoadScene(lastUnlockedLevel);
                AudioManager.Instance.PlayBackground(AudioManager.Instance._backgroundGameplay);
                return;
            }
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private Level GetCurrentLevel()
    {
        foreach (var level in SaveSystem._instance._levelData._level)
        {
            if (_actualScene == level._idLevel)
            {
                return level;
            }
        }
        return null;
    }

    private Level GetNextLevel(Level currentLevel)
    {
        int currentIndex = SaveSystem._instance._levelData._level.IndexOf(currentLevel);

        if (currentIndex >= 0 && currentIndex + 1 < SaveSystem._instance._levelData._level.Count)
        {
            return SaveSystem._instance._levelData._level[currentIndex + 1];
        }
        return null;
    }

    private string GetLastUnlockedLevel()
    {
        return SaveSystem._instance._lastLevelUnlocked;
    }


    public void FinishLevel()
    {
        Level currentLevel = GetCurrentLevel();
        Level nextLevel = GetNextLevel(currentLevel);

        if (currentLevel._state == Level.LevelState.Unlock && (nextLevel._state == Level.LevelState.Blocked || nextLevel._state == Level.LevelState.Unlock))
        {
            currentLevel._state = Level.LevelState.Completed;
            nextLevel._state = Level.LevelState.Unlock;
            SaveSystem._instance._lastLevelUnlocked = nextLevel._idLevel;
        }
    }

    //public void Pause()
    //{
    //    Time.timeScale = 0;
    //}

    //public void Play()
    //{
    //    Time.timeScale = 1;
    //}

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
