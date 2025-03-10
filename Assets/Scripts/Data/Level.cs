using NaughtyAttributes;
using System;

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
    [Scene]
    public string _idLevel;
    public LevelState _state;

    public Level(string id, LevelState initialState = LevelState.Blocked)
    {
        _idLevel = id;
        _state = initialState;
    }
}
