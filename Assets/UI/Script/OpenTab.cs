using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OpenTab : MonoBehaviour
{
    public GameObject NextPanel;
    public GameObject PrevPanel;
    public EventSystem events;
    public GameObject nextButton;

    public void OpenPanel()
    {
        if(NextPanel != null)
        {
            NextPanel.SetActive(true);
            events.SetSelectedGameObject(nextButton);
            PrevPanel.SetActive(false);            
        }
    }
}
