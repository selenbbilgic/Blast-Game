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

    
    public int currentLevel;
    public LevelData currentLevelData;

    public bool isGameEnded;

    public int boxCount = 0;
    public int stoneCount = 0;
    public int vaseCount = 0;
    public int moves;
    private const string lastLevelKey = "last_level";

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

        currentLevel = LoadPlayerProgress();
        if(currentLevel<=10){
            currentLevelData = LevelManager.Instance.GetLevel(currentLevel);
        }
        else{
            currentLevelData = LevelManager.Instance.GetLevel(1);
        }
        

        CalculateGoals();
        moves = currentLevelData.move_count;

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
        FailView.Instance.ShowTheCanvas();
        Start();

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

        // I did not put any restart button at the end of the game since it is not mentioned in the guide. 
        // You can restart the game by swapping the return lines below.

        return PlayerPrefs.GetInt(lastLevelKey, 1);
        //return 1;
    }
}
