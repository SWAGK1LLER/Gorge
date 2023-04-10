using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinningHandler : MonoBehaviour
{
    private LevelChanger lvlChanger;
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("LV") != null)
            lvlChanger = GameObject.FindGameObjectWithTag("LV").GetComponent<LevelChanger>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (lvlChanger != null)
                lvlChanger.FadeInToNextLevel(0);
        }
        if(Input.GetButtonDown("Fire1"))
        {
            if (lvlChanger != null)
                lvlChanger.FadeInToNextLevel(0);
        }
    }
}
