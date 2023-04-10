using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMessage : MonoBehaviour
{
    [SerializeField] private float animationTime;
    private GameMaster gameMaster;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("GM") != null)
            gameMaster = GameObject.FindGameObjectWithTag("GM").GetComponent<GameMaster>();
        StartCoroutine(ShowDeathMessage());        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ShowDeathMessage()
    {
        if (gameMaster != null)
        {
            yield return new WaitForSeconds(animationTime);
            gameMaster.RespawnFromDeath();
        }
    }
}
