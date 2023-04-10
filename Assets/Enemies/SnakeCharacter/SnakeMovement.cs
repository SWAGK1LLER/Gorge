using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private List<Transform> bodyParts;
    [SerializeField] private GameObject body;
    [SerializeField] private float minDistFromBodyParts;
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private int nbBodySize;

    private Transform currentBodyPart;
    private Transform prevBodyPart;
    private float dist;

    [SerializeField] private List<Transform> routes;
    private bool readyToMouvOnPath;

    public bool ReadyToMouvOnPath { get => readyToMouvOnPath; set => readyToMouvOnPath = value; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        for (int i = 1; i < bodyParts.Count; ++i)
        {
            currentBodyPart = bodyParts[i];
            prevBodyPart = bodyParts[i - 1];

            dist = Vector3.Distance(prevBodyPart.position, currentBodyPart.position);

            Vector3 newPos = prevBodyPart.position;

            newPos.y = bodyParts[0].position.y;

            float time = Time.deltaTime * dist / minDistFromBodyParts * speed;
            if (time > 0.5f)
                time = 0.5f;

            currentBodyPart.position = Vector3.Slerp(currentBodyPart.position, newPos, time);
            currentBodyPart.rotation = Quaternion.Slerp(currentBodyPart.rotation, prevBodyPart.rotation, time);
        }
    }
}
