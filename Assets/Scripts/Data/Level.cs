using System;
using UnityEditor;

[Serializable]
public class Level
{
    public enum LevelState
    {
        Blocked,
        Unlock,
        Completed
    }

    //public SceneAsset _scene;
    public string _idLevel;
    public LevelState _state;

}
