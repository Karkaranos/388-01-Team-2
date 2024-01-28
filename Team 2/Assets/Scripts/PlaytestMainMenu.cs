using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaytestMainMenu : MonoBehaviour
{
    [Header("Refrences:")]
    public TMP_Text gridText;
    public TMP_Text moveLassoText;
    private bool inMain = true;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject varsMenu;
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


    public void SwitchMenus()
    {
        if (inMain)
        {
            inMain = false;
            mainMenu.SetActive(false);
            varsMenu.SetActive(true);
        }
        else
        {
            inMain = true;
            mainMenu.SetActive(true);
            varsMenu.SetActive(false);
        }
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
