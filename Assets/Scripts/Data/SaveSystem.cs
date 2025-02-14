using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{

    public LevelProgressionData _levelData;
    public float _musicValue;
    public float _soundValue;

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
    }

    public void SaveData()
    {
        VolumeSettings.Instance.SaveVolume();
        SaveDataWrapper data = new SaveDataWrapper();
        data.levelData = _levelData;
        data.musicValue = _musicValue;
        data.soundValue = _soundValue;
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(_savePath, json);
        Debug.Log("Game Saved : " + _savePath);
    }

    public void LoadData()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            SaveDataWrapper data = JsonUtility.FromJson<SaveDataWrapper>(json);

            _levelData = data.levelData;
            _musicValue = data.musicValue;
            _soundValue = data.soundValue;
        }
        else
        {
            NewData();
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
        SaveData();
    }
}

[System.Serializable]
public class SaveDataWrapper
{
    public LevelProgressionData levelData;
    public float musicValue;
    public float soundValue;
}
