using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class UnlockAllLevels : MonoBehaviour
{
    [SerializeField] Sprite unlockedImage;

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
            gameObject.GetComponentInChildren<Button>().interactable = true;
            gameObject.GetComponentInChildren<Image>().sprite = unlockedImage;
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
                if (!gameObject.transform.GetChild(i).GetComponent<Text>() && !gameObject.transform.GetChild(i).GetComponent<Button>())
                {
                    Destroy(gameObject.transform.GetChild(i).gameObject);
                }
            }
        }

        int currentPage = VerifyCompleted.Instance._currentPage;
        VerifyCompleted.Instance.Verify();

        for (int i = 0; i < VerifyCompleted.Instance._pageBoutton.Count; i++)
        {
            GameObject page = VerifyCompleted.Instance._pageBoutton[i];
            if (i == currentPage)
            {
                page.SetActive(true);
            }
            else
            {
                page.SetActive(false);
            }
        }

        VerifyCompleted.Instance._currentPage = currentPage;
        VerifyCompleted.Instance.Boutton();
    }
}