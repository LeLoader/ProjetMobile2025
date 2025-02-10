using UnityEditor;
using UnityEngine;

public class FinishLevel1 : MonoBehaviour
{
    public static FinishLevel1 Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void FinishLevel()
    {
        SaveSystem._instance._levelData._level[0]._state = Level.LevelState.Completed;
        SaveSystem._instance._levelData._level[1]._state = Level.LevelState.Unlock;
    }

    public void FinishLevelTwo()
    {
        SaveSystem._instance._levelData._level[1]._state = Level.LevelState.Completed;
        SaveSystem._instance._levelData._level[2]._state = Level.LevelState.Unlock;
    }


}
