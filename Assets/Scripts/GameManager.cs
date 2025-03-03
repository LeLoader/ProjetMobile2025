using GooglePlayGames;
using NaughtyAttributes;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Scene]
    public string _actualScene;
    [SerializeField] int targetFrameRate = 60;
    //[SerializeField] private SceneAsset _scene;
    private bool setup;

    public GameObject _canvaReglage;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        Stats.IncrementStat(Stats.STATS.APPLICATION_STARTED_COUNT);
        AchivementManager.AutomaticConnect();
        if (Instance == null)
        {
            Instance = this;
            setup = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(!setup)
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
        if(setup)
        {
            _actualScene = scene.name;
            if (scene.name == "--MENU--")
            {
                AudioManager.Instance.PlayBackground(AudioManager.Instance._backgroundMenu);
                _canvaReglage.SetActive(true);
            }
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

}
