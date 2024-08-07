using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

//    private CelebrationView celebrationView;
//    private NavigationManager navigationManager;
    public GameObject welcomeCanvas;
    public TMP_Text levelsTxt;
    private int currentLevel;
    public LevelData currentLevelData;
    private const string lastLevelKey = "last_level";

    public bool isGameEnded;

    public int boxCount = 0;
    public int stoneCount = 0;
    public int vaseCount = 0;
    public int moves;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start(){
        Debug.Log("GameManager Start");

        currentLevel = LoadPlayerProgress();
        currentLevelData = LevelManager.Instance.GetLevel(currentLevel);

        levelsTxt.text = "Level " + currentLevel.ToString();

        CalculateGoals();
        moves = currentLevelData.move_count;
        Debug.Log("seloş move count..." + moves);

        isGameEnded = false;
    } 

    public void UpdateMoves(){
        moves--;

        if(moves == 0){
            CheckWinState();
            if(!isGameEnded){
                LoseGame();
            }
            
        }
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

    public void CheckWinState(){
        if(boxCount+stoneCount+vaseCount == 0){
            isGameEnded = true;
            WinGame();
        }
    }

    void WinGame(){
        
        SaveProgress(currentLevel + 1);

        CelebrationView.Instance.StartCelebration();
    }

    void LoseGame(){
        
        isGameEnded = true;
        Debug.Log("failview is calling.....");
        FailView.Instance.ShowTheCanvas();
        Start();

    }

    void CalculateGoals(){
        boxCount = currentLevelData.grid.Count(b => b=="bo");
        stoneCount = currentLevelData.grid.Count(s => s=="s");
        vaseCount = currentLevelData.grid.Count(v => v=="v");
    }

    public void SaveProgress(int levelNum){
        if(levelNum > 10){
            levelNum = 1;
        }
        PlayerPrefs.SetInt(lastLevelKey, levelNum);
        PlayerPrefs.Save();
    }

    public int LoadPlayerProgress(){
        Debug.Log("loading fiest levels...");
        //return PlayerPrefs.GetInt(lastLevelKey, 1);
        return 1;
    }

}
