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
    [SerializeField] private GameObject _prefabPage;
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
        GameObject levelPanel = Instantiate(_prefabPage, this.transform);
        GameObject levelObject;
        for (int i = 0; i < SaveSystem._instance._levelData._level.Count; i ++)
        {
            if(i % 10 != 0 || i == 0)
            {
                levelObject = Instantiate(_prefabLevel, levelPanel.transform);
            }
            else if(i % 10 == 0)
            {
                levelPanel = Instantiate(_prefabPage, this.transform);
                levelObject = Instantiate(_prefabLevel, levelPanel.transform);
            }
            else if(i % 20 != 0)
            {
                levelObject = Instantiate(_prefabLevel, levelPanel.transform);
            }
            else if (i % 20 == 0)
            {
                levelPanel = Instantiate(_prefabLevel, this.transform);
                levelObject = Instantiate(_prefabLevel, levelPanel.transform);
            }
            else
            {
                levelObject = Instantiate(_prefabLevel, levelPanel.transform);
            }
            ButtonManager boutton = levelObject?.GetComponent<ButtonManager>();
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
