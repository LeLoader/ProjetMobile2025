using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class UnlockAllLevels : MonoBehaviour
{
    [Button]
    public void UnlockAllLevelsMethod()
    {
        foreach (GameObject gameObject in VerifyCompleted.Instance._levelBoutton)
        {
            gameObject.GetComponentInChildren<Button>().interactable = true;
        }
    }
}
