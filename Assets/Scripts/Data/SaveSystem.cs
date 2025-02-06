using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{

    //dictionnaire à récupérer pour savoir quels niveaux sont finit.
    //Par exemple Level1, 1 /  Level2, 0
    //Le level 1 est finit et le level 2 n'est pas finit 

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
