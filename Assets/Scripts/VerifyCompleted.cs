using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VerifyCompleted : MonoBehaviour
{

    [SerializeField] private List<GameObject> _goLevel = new List<GameObject>();
    [SerializeField] private List<GameObject> _goBlocked = new List<GameObject>();
    [SerializeField] private List<GameObject> _goCompleted = new List<GameObject>();

    private void Start()
    {
        Verify();
    }

    public void UpdateImage()
    {
        foreach (GameObject go in _goBlocked)
        {
            
        }
    }

    public void Verify()
    {
        for(int i = 0; i < SaveSystem._instance._levelData._level.Count; i++)
        {
            if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Blocked)
            {
                _goBlocked.Add(_goLevel[i]);
            }
            else if (SaveSystem._instance._levelData._level[i]._state == Level.LevelState.Completed)
            {
                _goCompleted.Add(_goLevel[i]);
            }
        }
    }
}
