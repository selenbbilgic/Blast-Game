using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelebrationView : MonoBehaviour
{
    public static CelebrationView Instance;

    public GameObject celebrationPanel;
    public GameObject CelebrationStar;
    public float moveDuration = 1.0f; 
    public float celebrationDuration = 2.0f;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private ParticleSystem celebrationParticles;

    private void Awake(){
        Instance = this;
    }
    public void StartCelebration(){
        celebrationPanel.SetActive(true);

        startPosition = CelebrationStar.transform.position;
        endPosition = startPosition + new Vector3(0, 11, 0);

        celebrationParticles = CelebrationStar.GetComponentInChildren<ParticleSystem>();

        StartCoroutine(CelebrationSequence());
    }

    IEnumerator CelebrationSequence(){
        float elapsedTime = 0;

        while(elapsedTime < moveDuration){
            CelebrationStar.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime/moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        CelebrationStar.transform.position = endPosition;
        celebrationParticles.Play();
        yield return new WaitForSeconds(celebrationDuration); 

        //NavigationManager.Instance.NavigateTo("MainScene");
    }
}
