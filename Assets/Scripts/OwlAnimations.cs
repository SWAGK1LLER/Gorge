using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlAnimations : MonoBehaviour
{
    public Animator owl;

    void Start()
    {
        owl = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(FlyTowards());
    }

    IEnumerator FlyTowards()
    {
        yield return new WaitForSeconds(0.7f);
        owl.SetTrigger("FlyTowards");
    }
}
