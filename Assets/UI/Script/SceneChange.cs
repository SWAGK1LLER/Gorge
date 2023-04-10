using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    private LevelChanger lvlChanger;

    // Start is called before the first frame update
    void Start()
    {
        lvlChanger = GameObject.FindGameObjectWithTag("LV").GetComponent<LevelChanger>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeScene(int sceneIdx)
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        lvlChanger.FadeInToNextLevel(sceneIdx);
    }
}
