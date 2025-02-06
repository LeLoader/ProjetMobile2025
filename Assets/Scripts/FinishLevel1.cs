using UnityEngine;

public class FinishLevel1 : MonoBehaviour
{
    


    public void FinishLevel()
    {
        SaveSystem._instance._levelData._level[0]._state = Level.LevelState.Completed;
        SaveSystem._instance._levelData._level[1]._state = Level.LevelState.Unlock;
    }
}
