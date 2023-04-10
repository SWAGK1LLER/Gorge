using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private List<AudioSource> UIAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < UIAudioSource.Count; ++i)
            UIAudioSource[i].ignoreListenerPause = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
