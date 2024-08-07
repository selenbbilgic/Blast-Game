using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FailView : MonoBehaviour
{
    public static FailView Instance;
    public GameObject FailCanvas;

    public AudioClip failureSound;
    private AudioSource audioSource;

    public bool shouldNavigateToMain = false;
    public bool shouldTryAgain = false;

    private CanvasGroup canvasGroup;
    public float fadeDuration = 1.0f; 

    private void Awake(){
        Instance = this;
        canvasGroup = FailCanvas.GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ShowTheCanvas(){
        if (failureSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(failureSound); 
        }
        StartCoroutine(FadeInCanvas());
    }

    private IEnumerator FadeInCanvas(){
        FailCanvas.SetActive(true);
        canvasGroup.alpha = 0;

        float elapsedTime = 0;
        while (elapsedTime < fadeDuration){
            canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    public void HideTheCanvas(){
        FailCanvas.SetActive(false);
    }

    public void OnTryAgainButtonClicked()
    {
        HideTheCanvas();
        NavigationManager.Instance.NavigateTo(SceneManager.GetActiveScene().name);
    }

    public void OnCloseButtonClicked()
    {
        HideTheCanvas();
        NavigationManager.Instance.NavigateTo("MainScene");
    }
}

