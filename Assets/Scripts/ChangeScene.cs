using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{

    public event Action OnActualisaton;
    public static ChangeScene _instance { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }



    public void LoadScene(string sceneName)
    {
        OnActualisaton?.Invoke();
        SceneManager.LoadScene(sceneName);
    }



}
