using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class VerifyCompleted : MonoBehaviour
{

    [Header("prefabs")]
    [SerializeField] private GameObject _complete;
    [SerializeField] private GameObject _blocked;
    [SerializeField] private GameObject _prefabLevel;
    [SerializeField] private GameObject _prefabPage;

    [Header("useful")]
    [SerializeField] private List<GameObject> _levelBoutton;
    [SerializeField] private List<GameObject> _pageBoutton;
    [SerializeField] private Button _next;
    [SerializeField] private Button _prev;

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
        _pageBoutton.Add(levelPanel);
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
            _pageBoutton.Add(levelPanel);
            levelPanel.SetActive(false);
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
            _pageBoutton[0].SetActive(true);
            if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Blocked)
            {
                Cadena = Instantiate(_blocked, _levelBoutton[i].transform); 
            }
            else if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Completed)
            {
                Etoile = Instantiate(_complete, _levelBoutton[i].transform);
            }
        }
    }

    public void NextPage()
    {
        for (int i = 0; i < _pageBoutton.Count; i++)
        {
            if (_pageBoutton[i].activeInHierarchy)
            {
                _next.onClick.AddListener(NextPage);
            }
        }
    }

    public void PreviousPage()
    {

    }
}
