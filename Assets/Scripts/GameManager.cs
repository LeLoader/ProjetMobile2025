using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private SceneAsset _actualScene;

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
}
