using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseTab : MonoBehaviour
{
    public GameObject NextPanel;
    public GameObject PrevPanel;
    public EventSystem events;
    public GameObject nextButton;

    public void ClosePanel()
    {
        if (PrevPanel != null)
        {
            NextPanel.SetActive(true);
            events.SetSelectedGameObject(nextButton);
            PrevPanel.SetActive(false);
        }
    }
}

