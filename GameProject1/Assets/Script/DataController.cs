using System.IO;
using System;
using UnityEngine;

public class DataController : MonoBehaviour
{
    private static GameObject _container;
    private static DataController _instance;

    public static DataController instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<DataController>();
                if (_instance != null)
                {
                    _container = _instance.gameObject;
                    DontDestroyOnLoad(_container);
                }
                else
                {
                    _container = new GameObject("DataController");
                    DontDestroyOnLoad(_container);
                    _instance = _container.AddComponent<DataController>();
                }
                _instance.EnsureLoaded();
            }

            return _instance;

        }
    }

    [Header("Save")]
    public string GameDataFileName = "CardWorldSave.json";

    [SerializeField] private GameData _gameData;

    public GameData gameData
    {
        get
        {
            EnsureLoaded();
            return _gameData;
        }
    }

    private string GetFilePath()
    {
        return Path.Combine(Application.persistentDataPath, GameDataFileName);
    }

    private void EnsureLoaded()
    {
        if (_gameData != null)
        {
            _gameData.EnsureRuntimeDefaults();
            return;
        }

        LoadData();

        _gameData.EnsureRuntimeDefaults();

        SaveData();
    }

    public void LoadData()
    {
        string filePath = GetFilePath();

        if (File.Exists(filePath))
        {
            Debug.Log("Load Complete");
            string fromJson = File.ReadAllText(filePath);
            _gameData = JsonUtility.FromJson<GameData>(fromJson);
            if (_gameData == null) _gameData = new GameData();
        }
        else
        {
            Debug.Log("Create New File");
            _gameData = new GameData();
        }
    }
    public void SaveData()
    {
        EnsureLoaded();

        string toJson = JsonUtility.ToJson(gameData);
        string filePath = GetFilePath();

        File.WriteAllText(filePath, toJson);
        Debug.Log("Save Complete");
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _container = gameObject;
        DontDestroyOnLoad(gameObject);

        EnsureLoaded();
    }
}

