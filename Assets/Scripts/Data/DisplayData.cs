using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayData : MonoBehaviour
{
    [SerializeField] private Text _actualLevel;

    List<Level> _levelUnlock = new List<Level>();



    public void ChangeDisplay()
    {
        foreach(Level _level in SaveSystem._instance._levelData._level)
        {
            if(_level._state == Level.LevelState.Unlock)
            {
                _levelUnlock.Add(_level);
            }
        }
        int level = _levelUnlock.Count;
        _actualLevel.text = "Actual Level : " + level;
    }

}
