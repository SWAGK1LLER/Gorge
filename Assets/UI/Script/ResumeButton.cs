using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResumeButton : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    public void ResumeGame()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }    
}
