using NaughtyAttributes;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{

    public LevelProgressionData _levelData;
    public float _musicValue;
    public float _soundValue;
    public PlayerStats _playerStats;

    private string _savePath;
    public static SaveSystem _instance { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        _savePath = Path.Combine(Application.persistentDataPath, "saveData.json");
        LoadData();

        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            //scenes *= System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        }
    }

    

#if UNITY_EDITOR

    [SerializeField] string levelsFolderPath = "Assets/Scenes/Levels";
    [Button]
    private void FindAndAddMissingLevelData()
    {
        var assets = AssetDatabase.FindAssets("t:SceneAsset", new[] { levelsFolderPath });
        for (int i = 0; i < assets.Length; i++)
        {
            string guid = assets[i];
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(guid));
            if (!_levelData._level.Exists(level => level._idLevel == scene.name))
            {
                if (i != 0)
                {
                    _levelData._level.Add(new Level(scene.name));
                }
                else
                {
                    _levelData._level.Add(new Level(scene.name, Level.LevelState.Unlock));
                }
            }   
        }
        _levelData._level = _levelData._level.OrderBy(level => int.Parse(level._idLevel.Split(' ')[1])).ToList();
    }

    [Button]
    private void EraseAndGenerateLevelData()
    {
        var assets = AssetDatabase.FindAssets("t:SceneAsset", new[] { levelsFolderPath });
        _levelData._level.Clear();
        for (int i = 0; i < assets.Length; i++)
        {
            string guid = assets[i];
            SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(AssetDatabase.GUIDToAssetPath(guid));
            if (i != 0)
            {
                _levelData._level.Add(new Level(scene.name));
            }
            else
            {
                _levelData._level.Add(new Level(scene.name, Level.LevelState.Unlock));
            }
        }
    }
#endif

    public void SaveData()
    {
        VolumeSettings.Instance.SaveVolume();
        SaveDataWrapper data = new SaveDataWrapper();
        data.levelData = _levelData;
        data.musicValue = _musicValue;
        data.soundValue = _soundValue;
        data.stats = _playerStats;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_savePath, json);
        Debug.Log("Game Saved : " + _savePath);
    }

    public void LoadData()
    {
        Debug.Log("Loading Data: ");
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            SaveDataWrapper data = JsonUtility.FromJson<SaveDataWrapper>(json);

            _levelData = data.levelData;
            _musicValue = data.musicValue;
            _soundValue = data.soundValue;
            _playerStats = data.stats;
            Debug.Log("Success!");
        }
        else
        {
            NewData();
            Debug.Log("No data found, creating a data!");
        }
    }

    public void NewData()
    {
        _musicValue = 0.5f;
        _soundValue = 0.5f;

        SaveData();
        Debug.Log("New data created and saved at: " + _savePath);
    }

    private void OnApplicationQuit()
    {
        Stats.IncrementStat(Stats.STATS.IN_GAME_TIME, (int)Time.realtimeSinceStartup);
        SaveData();
    }
}

[System.Serializable]
public class SaveDataWrapper
{
    public LevelProgressionData levelData;
    public float musicValue;
    public float soundValue;
    public PlayerStats stats;
}
