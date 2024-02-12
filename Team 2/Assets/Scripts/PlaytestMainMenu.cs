using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class PlaytestMainMenu : MonoBehaviour
{
    [Header("Refrences:")]
    public TMP_Text gridText;
    public TMP_Text moveLassoText;
    [SerializeField] private bool inMain = true;

    /*
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject varsMenu;
    [SerializeField] private GameObject firstButtonMain;
    [SerializeField] private GameObject firstButtonCredits;
    */
    [SerializeField]
    private CanvasInfo[] canvases;
    // Start is called before the first frame update
    void Awake()
    {
    }

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

    public void SetMoveLassoStyle()
    {
        
    }


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


    /*public void SwitchMenus()
    {
        if (inMain)
        {
            inMain = false;
            mainMenu.SetActive(false);
            varsMenu.SetActive(true);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(firstButtonCredits);
        }
        else
        {
            inMain = true;
            mainMenu.SetActive(true);
            varsMenu.SetActive(false);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(firstButtonMain);
        }
    }*/

    private void SwitchCanvas(string newCanvas)
    {
        CanvasInfo canvas = Array.Find(canvases, CanvasInfo => CanvasInfo.canvasName == newCanvas);
        foreach(CanvasInfo c in canvases)
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

    public void OpenCredits()
    {
        SwitchCanvas("Credits");
    }

    public void QuitMenu()
    {
        SwitchCanvas("Quit");
    }

    public void Back()
    {
        SwitchCanvas("MainMenu");
    }

    public void HowToPlay()
    {
        SwitchCanvas("HowToPlay");
    }


    public void Quit()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
