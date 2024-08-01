using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public class LevelManager{

private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LevelManager();
                _instance.Initialize();
            }
            return _instance;
        }
    }

    public string levelsDirectory = "Levels";
    private LevelData[] levels;

    private LevelManager() { }

    public void Initialize(){
        LoadLevels();
    }

    public void LoadLevels()
    {
        Debug.Log("loading all levels...");
        string path = Path.Combine(Application.streamingAssetsPath, levelsDirectory);
        string[] files = Directory.GetFiles(path, "*.json");

        levels = files.Select(file => JsonUtility.FromJson<LevelData>(File.ReadAllText(file))).ToArray();
        Debug.Log("loaded all levels...");
    }

    public LevelData GetLevel(int levelNumber)
    {
        return levels.FirstOrDefault(level => level.level_number == levelNumber);
    }

}


public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public string[] grid;

}