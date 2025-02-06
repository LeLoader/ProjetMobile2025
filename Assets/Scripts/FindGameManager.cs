using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FindGameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameManager;
    [SerializeField] private Button _button;

    private void Start()
    {
        FindGame();
    }

    public void FindGame()
    {
        _gameManager = GameObject.Find("GameManager");
    }

    public void ButtonClick()
    {
        ChangeScene _changeScene = _gameManager.GetComponentInChildren<ChangeScene>();
        _changeScene.LoadScene(" ");
    }

}
