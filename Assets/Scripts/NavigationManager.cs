using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationManager : MonoBehaviour
{
    public void NavigateTo(string _sceneName){
        SceneManager.LoadScene(_sceneName);
    }
}
