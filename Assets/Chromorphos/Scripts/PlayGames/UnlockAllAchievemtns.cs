using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class UnlockAllLevels : MonoBehaviour
{
    [Button]
    public void UnlockAllLevelsMethod()
    {
        foreach (Level level in SaveSystem._instance._levelData._level)
        {
            if (level._state == Level.LevelState.Blocked)
            {
                level._state = Level.LevelState.Unlock;
            }
        }

        foreach (GameObject gameObject in VerifyCompleted.Instance._levelBoutton)
        {
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                if (!gameObject.transform.GetChild(i).GetComponent<Text>() && !gameObject.transform.GetChild(i).GetComponent<Button>())
                {
                    Destroy(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }

        VerifyCompleted.Instance.Verify();
    }
}
