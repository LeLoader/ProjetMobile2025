using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{

    public static SliderManager Instance;

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
}