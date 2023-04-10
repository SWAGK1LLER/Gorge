using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    [SerializeField] private Animator fadeAnimator;
    private bool changingScene = false;
    private int lvlToLoad;

    public bool ChangingScene { get => changingScene; set => changingScene = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FadeInToNextLevel(int lvlIdx)
    {
        ChangingScene = true;
        lvlToLoad = lvlIdx;
        fadeAnimator.SetBool("FadeOut", true);
    }

    public void OnFadeComplete()
    {
        ChangingScene = false;
        SceneManager.LoadScene(lvlToLoad, LoadSceneMode.Single);
    }

    public void FadeOutInNextLevel()
    {
        fadeAnimator.SetBool("FadeOut", false);        
    }
}
