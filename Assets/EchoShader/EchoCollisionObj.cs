using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoCollisionObj : MonoBehaviour
{
    [SerializeField] private ChangeShaderProperty controller;
    [SerializeField] private float echoTravelSpeed;
    [SerializeField] private float rangeEcho;
    [SerializeField] private int echoPriority;
    [SerializeField] private int nbEchoProduced;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        controller.Fire(collision.GetContact(0).point, rangeEcho, echoTravelSpeed, echoPriority, nbEchoProduced);
    }
}
