using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class EnemyIntroInstruction : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource sound;
    [SerializeField] private LevelScript checkpointSystem;
    [SerializeField] private GameObject enemyToLookAt;
    [SerializeField] private GameObject enemyBody;
    [SerializeField] private Transform enemyTarget;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private PlayerManager playerController;
    private bool triggered = false;
    private bool enemyActive = false;
    private bool enemyGone = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyToLookAt.GetComponentInChildren<SnakeControllerTest>().enabled = false;
        enemyToLookAt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyGone)
        {
            if (Vector3.Distance(enemyToLookAt.transform.position, enemyTarget.position) < 2)
            {
                Destroy(enemyBody);
                enemyActive = false;
                enemyGone = true;
            }
        }
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.5f);
        if (!triggered)
        {            
            triggered = true;
            enemyActive = true;
            enemyToLookAt.SetActive(true);
            enemyToLookAt.GetComponentInChildren<SnakeControllerTest>().enabled = true;
            agent.SetDestination(enemyTarget.position);
            sound.Play();
            playerController.objects.controller.Fire(enemyToLookAt.transform.position, 10, 10, 0, 3);
            StartCoroutine(FireAgain());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Timer());
    }

    private IEnumerator FireAgain()
    {
        while (enemyActive)
        {
            yield return new WaitForSeconds(sound.clip.length);
            if (enemyToLookAt != null)
            {
                playerController.objects.controller.Fire(enemyToLookAt.transform.position, 10, 10, 0, 3);
                sound.Play();
            }
        }
    }
}
