using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameMenuController : MonoBehaviour
{
    [SerializeField]
    private CanvasInfo[] canvases;

    [HideInInspector]
    public bool isPaused;

    private void SwitchCanvas(string newCanvas)
    {
        CanvasInfo canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == newCanvas);
        foreach (CanvasInfo c in canvases)
        {
            c.Canvas.SetActive(false);
        }
        if (canvas != null)
        {
            canvas.Canvas.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(canvas.firstSelected);

        }
        else
        {
            print("No matching canvas found");
        }
    }


    public void QuitMenu()
    {
        SwitchCanvas("Quit");
    }

    public void Back()
    {
        SwitchCanvas("PauseMenu");
    }

    public void Pause()
    {
        if(!isPaused)
        {
            SwitchCanvas("PauseMenu");
            isPaused = true;
        }
        else
        {
            isPaused = false;
            Resume();
        }
    }

    public void Resume()
    {
        isPaused = false;
        foreach (CanvasInfo c in canvases)
        {
            c.Canvas.SetActive(false);
        }
    }



    public void Quit()
    {
        Application.Quit();
    }
}
