using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisplayData : MonoBehaviour
{
    [SerializeField] private Text _actualLevel;
    [SerializeField] private Text _numberLevel;
    [SerializeField] private Text _completedLevel;
    List<Level> _levelUnlock = new List<Level>();

    

   

    

    IEnumerator WaitForNextLevel()
    {
        yield return new WaitForSeconds(3f);
        CompleteLevel();
    }

    private void Start()
    {
        ChangeDisplay();
        StartCoroutine(WaitForNextLevel());
    }

    public void ChangeDisplay()
    {
        if (SaveSystem._instance == null)
        {
            return;
        }
        foreach(Level _level in SaveSystem._instance._levelData._level)
        {
            if(_level._state == Level.LevelState.Unlock || _level._state == Level.LevelState.Completed)
            {
                _levelUnlock.Add(_level);
            }
        }
        if (_actualLevel != null)
        {
            int level = _levelUnlock.Count;
            _actualLevel.text = "Actual Level : " + level;
        }
        if (_numberLevel != null)
        {
            _numberLevel.text = GameManager.Instance._actualScene;
        }
        
    }

    public void CompleteLevel()
    {
        if (_completedLevel != null)
        {
            _completedLevel.text = GameManager.Instance._actualScene + " Completed";
        }
    }
}
