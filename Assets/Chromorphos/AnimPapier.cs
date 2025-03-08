using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimPapier : MonoBehaviour
{
    public void LoveFrancko()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
