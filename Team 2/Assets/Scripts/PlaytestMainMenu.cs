/*****************************************************************************
// File Name :         PlaytestMainMenu.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Handles the buttons on the main menu

*****************************************************************************/

using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlaytestMainMenu : MonoBehaviour
{
    //holds refrences to the texts
    [Header("Refrences:")]
    public TMP_Text gridText;
    public TMP_Text moveLassoText;
    [SerializeField] private bool inMain = true;

    [SerializeField]
    private CanvasInfo[] canvases;

    /// <summary>
    /// allows the designers to set the style of generation
    /// </summary>
    public void SetGridStyle()
    {
        if (RoomGenerator.GridStyle)
        {
            RoomGenerator.GridStyle = false;
        }
        else 
        {
            RoomGenerator.GridStyle = true;
        }
    }

    /// <summary>
    /// allows the player to change if the player can move while lassoing
    /// </summary>
    /// <param name="toggle"></param>
    public void LassoMovement(bool toggle)
    {
        if (toggle)
        {
            PlayerBehavior.canMoveWhileLassoing = false;
        }
        else
        {
            PlayerBehavior.canMoveWhileLassoing = true;
        }
    }

    /// <summary>
    /// also allows the player to set the style of generation
    /// </summary>
    /// <param name="i"></param>
    public void DropdownTest(int i)
    {
        Debug.Log(i);
        switch (i)
        {
            case 0:
                RoomGenerator.GridStyle = true;
                break;
            case 1:
                
                RoomGenerator.GridStyle = false;
                break;
            default:
                
                break;
        }
    }

    /// <summary>
    /// Sets the active canvas based on a provided name
    /// </summary>
    /// <param name="newCanvas">The canvas to set active</param>
    private void SwitchCanvas(string newCanvas)
    {
        //Finds the associated canvas and sets all canvases to inactive
        CanvasInfo canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == newCanvas);
        foreach(CanvasInfo c in canvases)
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
    /// Opens the credit canvas
    /// </summary>
    public void OpenCredits()
    {
        SwitchCanvas("Credits");
    }

    /// <summary>
    /// Opens the quit canvas
    /// </summary>
    public void QuitMenu()
    {
        SwitchCanvas("Quit");
    }

    /// <summary>
    /// Closes any non-menu canvases
    /// </summary>
    public void Back()
    {
        SwitchCanvas("MainMenu");
    }

    /// <summary>
    /// Opens the how to play canvas
    /// </summary>
    public void HowToPlay()
    {
        SwitchCanvas("HowToPlay");
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Loads the game scene
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
