/*****************************************************************************
// File Name :         EnemyBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     February 10, 2024
//
// Brief Description : Handles all UI menu functionality for non-title scenes

*****************************************************************************/
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class GameMenuController : MonoBehaviour
{
    [SerializeField]
    private CanvasInfo[] canvases;

    [HideInInspector]
    public bool isPaused;

    /// <summary>
    /// Sets the active canvas based on a provided name
    /// </summary>
    /// <param name="newCanvas">The canvas to set active</param>
    private void SwitchCanvas(string newCanvas)
    {
        //Finds the associated canvas and sets all canvases to inactive
        CanvasInfo canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == newCanvas);
        foreach (CanvasInfo c in canvases)
        {
            c.Canvas.SetActive(false);
        }

        //Sets the current canvas and its first selected
        if (canvas != null)
        {
            canvas.Canvas.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(canvas.firstSelected);

        }
    }

    /// <summary>
    /// Opens the quit menu
    /// </summary>
    public void QuitMenu()
    {
        SwitchCanvas("Quit");
    }

    /// <summary>
    /// Resets the in-game menu back to pause
    /// </summary>
    public void Back()
    {
        SwitchCanvas("PauseMenu");
    }

    /// <summary>
    /// Handles pausing or unpausing the game
    /// </summary>
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

    /// <summary>
    /// Resumes the game 
    /// </summary>
    private void Resume()
    {
        isPaused = false;
        foreach (CanvasInfo c in canvases)
        {
            c.Canvas.SetActive(false);
        }
    }

    /// <summary>
    /// Loads the main scene
    /// </summary>
    public void Quit()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    /// <summary>
    /// Exits the pause options on the end screen
    /// </summary>
    public void ResumeEnd()
    {
        SwitchCanvas("End");
    }
}
