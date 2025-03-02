using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class VerifyCompleted : MonoBehaviour
{

    [SerializeField] private List<GameObject> _goLevel = new List<GameObject>();
    [SerializeField] private List<GameObject> _goBlocked = new List<GameObject>();
    [SerializeField] private List<GameObject> _goCompleted = new List<GameObject>();
    [SerializeField] private GameObject _prefabLevel;
    [SerializeField] private GameObject _pageOne;
    [SerializeField] private GameObject _pageTwo;
    [SerializeField] private GameObject _pageThree;

    public static VerifyCompleted Instance;

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

    private void Start()
    {
        //Verify();
        ButtonCreate();
    }

    public void ButtonCreate()
    {
        for( int i = 0; i < SaveSystem._instance._levelData._level.Count; i ++)
        {
            GameObject levelObject;

            if(i < 10)
            {
                levelObject = Instantiate(_prefabLevel, _pageOne.transform);
            }
            else if(i < 20)
            {
                levelObject = Instantiate(_prefabLevel, _pageTwo.transform);
            }
            else
            {
                levelObject = Instantiate(_prefabLevel, _pageThree.transform);
            }
            ButtonManager boutton = levelObject.GetComponent<ButtonManager>();
            Text nombreLevel = levelObject.GetComponentInChildren<Text>();
            boutton._nameNextScene = "Level " + (i + 1); ;
            nombreLevel.text = "Level\n" + (i + 1); ;
        }
    }

    public void Verify()
    {
        for(int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
        {
            UnityEngine.UI.Button bouton = _goLevel[i]?.GetComponentInChildren<UnityEngine.UI.Button>();

            if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Blocked)
            {
                if(!_goBlocked[i].activeInHierarchy)
                {
                    _goBlocked[i].SetActive(true);
                    bouton.gameObject.SetActive(false);
                }
            }
            else if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Completed)
            {
                if (!_goCompleted[i].activeInHierarchy)
                {
                    _goCompleted[i].SetActive(true);
                    bouton.gameObject.SetActive(true);
                }
            }
            else if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Unlock)
            {
                bouton.gameObject.SetActive(true);
            }
        }
    }
}
