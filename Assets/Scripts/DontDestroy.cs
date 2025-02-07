using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    void Update()
    {
        DontDestroyOnLoad(gameObject);   
    }
}
