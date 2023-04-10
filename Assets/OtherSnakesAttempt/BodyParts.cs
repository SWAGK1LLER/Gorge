using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyParts : MonoBehaviour
{
    [SerializeField] private Transform srcObj;
    [SerializeField] private Transform targetObj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FaceObject(srcObj, targetObj.position);
    }

    private static void FaceObject(Transform srcObj, Vector3 targetPos)
    {
        srcObj.rotation = Quaternion.LookRotation(srcObj.position - targetPos);
    }
}
