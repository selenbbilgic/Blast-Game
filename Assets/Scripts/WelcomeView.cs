using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WelcomeView : MonoBehaviour
{
    public GameObject welcomeCanvas;

    public TMP_Text levelsTxt;

    // Start is called before the first frame update
    void Start()
    {

        if(GameManager.Instance.currentLevel > 10){
            levelsTxt.text = "Finished!";
        }
        else{
            levelsTxt.text = "Level " + GameManager.Instance.currentLevel.ToString();
        }
    }

    public void onStartButtonTapped(){
        if(GameManager.Instance.currentLevel <= 10){
            NavigationManager.Instance.NavigateTo("LevelScene");
        }
    }



}
