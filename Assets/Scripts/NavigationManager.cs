using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance; 
    public void NavigateTo(string _sceneName){
        SceneManager.LoadScene(_sceneName);
    }

    private void Awake(){
        Instance = this;
    }
}
