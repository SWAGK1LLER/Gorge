using System.Collections;
using UnityEngine;

public class CheckPointTrigger : MonoBehaviour
{
    [SerializeField] private LevelScript checkpointSystem;
    private bool checkpointUsed = false;
    public AudioClip triggerSound;
    private bool playing = false;
    private AudioSource audioSource;
    private BoxCollider boxCollider;
    private int timeGap = 1;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if(checkpointUsed)
        {
            if(boxCollider.enabled)
            {
                checkpointUsed = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
         if(other.CompareTag("Player") && !checkpointUsed)
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
            checkpointSystem.CurrentCheckpoint = new Vector3(transform.position.x, 0.4f, transform.position.z);
            checkpointSystem.CurrentRotation = transform.eulerAngles;
            checkpointSystem.CurrentPassedCheckpoint = this.gameObject.name;
            this.GetComponent<BoxCollider>().enabled = false;
            checkpointUsed = true;
        }
    }
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(timeGap);
        playing = false;
    }
}
