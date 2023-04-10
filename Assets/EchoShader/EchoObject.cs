using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoObject : MonoBehaviour
{
    [SerializeField] private ChangeShaderProperty controller;
    [SerializeField] private float timeMinBeforeEcho;
    [SerializeField] private float timeMaxBeforeEcho;
    [SerializeField] private float echoTravelSpeed;
    [SerializeField] private float rangeEcho;
    [SerializeField] private int echoPriority;
    [SerializeField] private int nbEchoProduced;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ShootEcho());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator ShootEcho()
    {
        while (true)
        {
            float rand = Random.Range(timeMinBeforeEcho, timeMaxBeforeEcho);
            yield return new WaitForSeconds(rand);
            controller.Fire(this.transform.position, rangeEcho, echoTravelSpeed, echoPriority, nbEchoProduced);
        }
    }
}
