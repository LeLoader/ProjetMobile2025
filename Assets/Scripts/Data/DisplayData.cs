using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DisplayData : MonoBehaviour
{
    [SerializeField] private Text _actualLevel;
    [SerializeField] private Text _numberLevel;
    [SerializeField] private Text _completedLevel;
    List<Level> _levelUnlock = new List<Level>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ChangeDisplay();
    }

    public void ChangeDisplay()
    {
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
        if (_completedLevel != null)
        {
            _completedLevel.text = GameManager.Instance._actualScene + " Completed";
        }
    }
}
