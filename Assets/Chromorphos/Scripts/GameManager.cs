using GooglePlayGames;
using NaughtyAttributes;
using NUnit.Framework;
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

    [SerializeField] histoire history;

    [SerializeField, Tag, Tooltip("All objects with one of those tags will be disable during scene transition")] string[] tagToDisable;

    private bool setup;

    public Porte porte;

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
        Debug.Log("new instance of GM");
        if (Instance == null)
        {
            Debug.Log("no existing instance");
            Instance = this;
            setup = true;
        }
        else
        {
            Debug.Log("already existing instance");
            if (!setup)
                Destroy(gameObject);
        }
        Debug.Log("end of new instance of GM");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        AudioManager.Instance?.PlayBackground(AudioManager.Instance?._BangerMenu);
        porte = GameObject.FindGameObjectWithTag("Porte")?.GetComponent<Porte>();

    }
    public void ManualConnect()
    {
        AchivementManager.ManualConnect();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (setup)
        {
            _actualScene = scene.name;
            porte = GameObject.FindGameObjectWithTag("Porte")?.GetComponent<Porte>();
            if (scene.name == "--MENU--")
            {
                AudioManager.Instance?.PlayBackground(AudioManager.Instance?._BangerMenu);
                _canvaReglage.SetActive(true);
            }
            else
            {
                AudioManager.Instance?.PlayBackground(AudioManager.Instance?._BangerGameplay);
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
            _canvaReglage.SetActive(false);
            if (_actualScene == "Level 1" && !SaveSystem._instance._sawIntro)
            {
                history.Image.SetActive(true);
                history.LaunchHistory();
            }
            else
            {
                SceneManager.LoadScene(_actualScene);
            }
            return;
        }
        else if (scene == SCENEPARAMETERS.CURRENT_LEVEL)
        {
            if (level == "--MENU--")
            {
                _canvaReglage.SetActive(true);
                SceneManager.LoadScene(_actualScene);
                Stats.IncrementStat(Stats.STATS.RESTART_COUNT);
            }
            else
            {
                _canvaReglage.SetActive(false);
                if (level == "Level 1" && !SaveSystem._instance._sawIntro)
                {
                    history.Image.SetActive(true);
                    history.LaunchHistory();
                }
                else
                {
                    SceneManager.LoadScene(level);
                }
            }
            return;
        }
        else if (scene == SCENEPARAMETERS.NEXT_LEVEL)
        {
            porte = GameObject.FindGameObjectWithTag("Porte")?.GetComponent<Porte>();
            _actualScene = _nextScene._idLevel;
            GetLevel();
            _canvaReglage.SetActive(false);
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("WordObject"));
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("UI"));
            foreach (string tag in tagToDisable)
            {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag))
                {
                    go.SetActive(false);
                }
            }
            porte.asyncOperation.allowSceneActivation = true;
            porte.Paper.SetTrigger("Start");
            return;
        }
        else if (scene == SCENEPARAMETERS.MENU_SCENE)
        {
            SceneManager.LoadScene("--MENU--");
            return;
        }
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
                else
                {
                    _nextScene = SaveSystem._instance._levelData._level[i + 1];
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
            if (_actualLevel._idLevel == "Level 1" && _nextScene._state == Level.LevelState.Blocked)
            {
                AchivementManager.UnlockAchievement(AchivementManager.FirstTry);
            }
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
