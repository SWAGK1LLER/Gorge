using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check : MonoBehaviour
{
    public AudioClip triggerSound;
    bool playing = false; 
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playing)
        {
            if (triggerSound != null)
            {
                playing = true;
                StartCoroutine(Timer());
                audioSource.PlayOneShot(triggerSound, 0.7F);
                
            }
        }
        
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        playing = false;
    }
}
