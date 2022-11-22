using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[Serializable]
public class DataController : MonoBehaviour
{
    static GameObject _container;

    static GameObject Container
    {
        get
        {
            return _container;
        }
    }
    static DataController _instance;

    public static DataController instance
    {
        get
        {
            if(!_instance)
            {
                _container = new GameObject();
                _container.name = "DataController";
                _instance = _container.AddComponent(typeof(DataController)) as DataController;
            }
            return _instance;
        }
    }
    public string GameDataFileName = "CardWorldSave.json";
    public GameData _gameData;

    public GameData gameData
    {
        get
        {
            if(_gameData == null)
            {
                LoadData();
                SaveData();
            }
            return _gameData;
        }
    }
    public void LoadData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;
        if (File.Exists(filePath))
        {
            Debug.Log("Load Complete");
            string FromJsonData = File.ReadAllText(filePath);
            _gameData = JsonUtility.FromJson<GameData>(FromJsonData);
        }
        else
        {
            Debug.Log("Create New File");
            _gameData = new GameData();
        }
    }
    public void SaveData()
    {
        string ToJsonData = JsonUtility.ToJson(gameData);
        string filePath = Application.persistentDataPath + GameDataFileName;
        File.WriteAllText(filePath, ToJsonData);
        Debug.Log("Save Complete");
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}

