using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelebrationView : MonoBehaviour
{
    public static CelebrationView Instance;

    public GameObject celebrationPanel;
    public GameObject CelebrationStar;
    public float moveDuration = 1.0f; 
    public float celebrationDuration = 3.0f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private ParticleSystem celebrationParticles;

    private void Awake(){
        Instance = this;
    }
    public void StartCelebration(){
        celebrationPanel.SetActive(true);

        RectTransform starRectTransform = CelebrationStar.GetComponent<RectTransform>();

        startPosition = starRectTransform.anchoredPosition;
        endPosition = startPosition + new Vector3(0, 1100, 0);

        celebrationParticles = CelebrationStar.GetComponentInChildren<ParticleSystem>();

        StartCoroutine(CelebrationSequence(starRectTransform));
    }

    IEnumerator CelebrationSequence(RectTransform starRectTransform){
        float elapsedTime = 0f;

        while(elapsedTime < moveDuration){
            starRectTransform.anchoredPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;

        }

        starRectTransform.anchoredPosition = endPosition;
        celebrationParticles.Play();
        yield return new WaitForSeconds(celebrationDuration); 

        NavigationManager.Instance.NavigateTo("MainScene");
    }
}
