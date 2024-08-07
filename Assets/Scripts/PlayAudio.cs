using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audioSource;

    public void Play()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
}
