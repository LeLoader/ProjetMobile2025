using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private string _actualScene;
    [SerializeField] private SceneAsset _scene;

    AudioManager _audioManager;

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
            //jouer la musique de background 
        }
        else
        {
            //jouer la musique de gameplay
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
            for (int i = 0; i<0; i++)
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
