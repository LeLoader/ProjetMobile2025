using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class VerifyCompleted : MonoBehaviour
{

    [SerializeField] private GameObject _complete;
    [SerializeField] private GameObject _blocked;
    [SerializeField] private GameObject _prefabLevel;
    [SerializeField] private GameObject _pageOne;
    [SerializeField] private GameObject _pageTwo;
    [SerializeField] private GameObject _pageThree;
    [SerializeField] private List<GameObject> _levelBoutton;

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
        ButtonCreate();
        Verify();
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
            _levelBoutton.Add(levelObject);
        }
    }

    public void Verify()
    {
        for( int i = 0; i < _levelBoutton.Count; i ++)
        {
            GameObject Etoile;
            GameObject Cadena;

            if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Blocked)
            {
                Cadena = Instantiate(_blocked, _levelBoutton[i].transform); 
            }
            else if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Completed)
            {
                Etoile = Instantiate(_complete, _levelBoutton[i].transform);
            }
        }


        //for(int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
        //{
        //    UnityEngine.UI.Button bouton = _goLevel[i]?.GetComponentInChildren<UnityEngine.UI.Button>();

            //    if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Blocked)
            //    {
            //        if(!_goBlocked[i].activeInHierarchy)
            //        {
            //            _goBlocked[i].SetActive(true);
            //            bouton.gameObject.SetActive(false);
            //        }
            //    }
            //    else if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Completed)
            //    {
            //        if (!_goCompleted[i].activeInHierarchy)
            //        {
            //            _goCompleted[i].SetActive(true);
            //            bouton.gameObject.SetActive(true);
            //        }
            //    }
            //    else if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Unlock)
            //    {
            //        bouton.gameObject.SetActive(true);
            //    }
            //}
    }
}
