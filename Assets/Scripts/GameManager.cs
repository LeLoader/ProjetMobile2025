using GooglePlayGames;
using NaughtyAttributes;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Scene]
    public string _actualScene;
    public Level _actualLevel;
    public Level _nextScene;
    [SerializeField] int targetFrameRate = 60;

    private bool setup;

    public enum SCENEPARAMETERS
    {
        LAST_LEVEL,
        CURRENT_LEVEL,
        NEXT_LEVEL,
        MENU_SCENE,
    }
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
            if (!setup)
                Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        AudioManager.Instance?.PlayBackground(AudioManager.Instance?._backgroundMenu);
    }
    public void ManualConnect()
    {
        //AchivementManager.ManualConnect();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (setup)
        {
            _actualScene = scene.name;
            if (scene.name == "--MENU--")
            {
                AudioManager.Instance?.PlayBackground(AudioManager.Instance?._backgroundMenu);
                _canvaReglage.SetActive(true);
            }
        }
    }

    public void ShowAchievement()
    {
        AchivementManager.ShowAchievement();
    }

    public void ChangeScene(SCENEPARAMETERS scene, string level)
    {
        if (scene == SCENEPARAMETERS.LAST_LEVEL)
        {
            _actualScene = SaveSystem._instance._lastLevelUnlocked;
            SceneManager.LoadScene(_actualScene);
            return;
        }
        else if (scene == SCENEPARAMETERS.CURRENT_LEVEL)
        {
            SceneManager.LoadScene(level);
            return;
        }
        else if (scene == SCENEPARAMETERS.NEXT_LEVEL)
        {
            _actualScene = _nextScene._idLevel;
            GetLevel();
            SceneManager.LoadScene(_actualScene);
            return;
        }
        else if (scene == SCENEPARAMETERS.MENU_SCENE)
        {
            SceneManager.LoadScene("--MENU--");
            return;
        }

        //Level currentLevel = GetCurrentLevel();

        //if (currentLevel == null && _actualScene != "--MENU--")
        //{
        //    Debug.Log("marche pas");
        //    return;
        //}
        //else if (sceneName == "thisScene")
        //{
        //    SceneManager.LoadScene(currentLevel._idLevel);
        //    return;
        //}
        //else if (sceneName == "nextLevel")
        //{
        //    Level nextLevel = GetNextLevel(currentLevel);
        //    if (int.Parse(currentLevel._idLevel.Split(' ')[1]) == 1)
        //    {
        //        AchivementManager.UnlockAchivement(AchivementManager.FirstTry);
        //    }

        //    if (nextLevel != null)
        //    {
        //        SceneManager.LoadScene(nextLevel._idLevel);
        //        return;
        //    }
        //}
        //else if (sceneName == "lastLevel")
        //{
        //    string lastUnlockedLevel = GetLastUnlockedLevel();

        //    if (lastUnlockedLevel != null)
        //    {
        //        SceneManager.LoadScene(lastUnlockedLevel);
        //        AudioManager.Instance?.PlayBackground(AudioManager.Instance?._backgroundGameplay);
        //        return;
        //    }
        //}
    }

    private void GetLevel()
    {
        for (int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
        {
            if (_actualScene == SaveSystem._instance._levelData._level[i]._idLevel)
            {
                if (i == SaveSystem._instance._levelData._level.Count - 1)
                {
                    _nextScene = null;

                }
                _actualLevel = SaveSystem._instance._levelData._level[i];
                return;
            }
        }
    }



    public void FinishLevel(GameObject nextlevel)
    {
        GetLevel();
        if (_nextScene == null)
        {
            _actualLevel._state = Level.LevelState.Completed;
            nextlevel.SetActive(false);
            
        }

        else if (_actualLevel._state == Level.LevelState.Unlock && _nextScene._state == Level.LevelState.Blocked)
        {

            _actualLevel._state = Level.LevelState.Completed;
            _nextScene._state = Level.LevelState.Unlock;
            SaveSystem._instance._lastLevelUnlocked = _nextScene._idLevel;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
