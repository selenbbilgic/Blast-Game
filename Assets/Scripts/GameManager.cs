using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject welcomeCanvas;
    public TMP_Text levelsTxt;
    private int currentLevel;
    public LevelData currentLevelData;
    private const string lastLevelKey = "last_level";

    private void Awake(){
        Instance = this;
        
    }

    void Start(){
        Debug.Log("GameManager Start");

        currentLevel = LoadPlayerProgress();
        currentLevelData = LevelManager.Instance.GetLevel(currentLevel);

        levelsTxt.text = "Level " + currentLevel.ToString();
    } 

    public void SaveProgress(int levelNum){
        PlayerPrefs.SetInt(lastLevelKey, levelNum);
        PlayerPrefs.Save();
    }

    public int LoadPlayerProgress(){
        Debug.Log("loading fiest levels...");
        return PlayerPrefs.GetInt(lastLevelKey, 4);
    }

}
