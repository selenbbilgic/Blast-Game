using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject welcomeCanvas;
    public TMP_Text levelsTxt;
    private int currentLevel;
    public LevelData currentLevelData;
    private const string lastLevelKey = "last_level";

    public int boxCount = 0;
    public int stoneCount = 0;
    public int vaseCount = 0;

    public int moves;

    private void Awake(){
        Instance = this;
        
    }

    void Start(){
        Debug.Log("GameManager Start");

        currentLevel = LoadPlayerProgress();
        currentLevelData = LevelManager.Instance.GetLevel(currentLevel);

        levelsTxt.text = "Level " + currentLevel.ToString();

        CalculateGoals();
        moves = currentLevelData.move_count;
    } 

    public void UpdateMoves(){
        moves--;
    }

    public void UpdateItems(Item item){
        if(item is Box){
            boxCount--;
        }
        if(item is Stone){
            stoneCount--;
        }
        if(item is Vase){
            vaseCount--;
        }
    }

    void CalculateGoals(){
        boxCount = currentLevelData.grid.Count(b => b=="bo");
        stoneCount = currentLevelData.grid.Count(s => s=="s");
        vaseCount = currentLevelData.grid.Count(v => v=="v");
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
