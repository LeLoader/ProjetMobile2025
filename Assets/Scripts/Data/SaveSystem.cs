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

    public void SaveData(LevelProgressionData datas)
    {
        string json = JsonUtility.ToJson(datas);
        File.WriteAllText(_savePath, json);
        Debug.Log("Game Saved : " + _savePath);
    }

    public void LoadData()
    {
        if (File.Exists(_savePath))
        {
            string json = File.ReadAllText(_savePath);
            _levelData = JsonUtility.FromJson<LevelProgressionData>(json);
        }
        else
        {
            NewData();
        }
        ChangeScene._instance.LoadScene("Guillaume");
    }

    public void NewData()
    {
        string txt = JsonUtility.ToJson(_levelData);
        File.WriteAllText(_savePath, txt);
        Debug.Log("Data Created in : " + _savePath);
    }

    private void OnApplicationQuit()
    {
        SaveData(_levelData);
    }
}
