/*****************************************************************************
// File Name :         GameManager.cs
// Author :            Tyler Hayes
// Creation Date :     January 23, 2024
//
// Brief Description : Handles high scores and the pause menu

*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int enemiesDefeated;

    /// <summary>
    /// initializes the high score
    /// </summary>
    private void Awake()
    {
        //if the player doesnt have high score, makes one
        if (PlayerPrefs.HasKey("HighScore") == false)
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }

        //same for the current score
        if(PlayerPrefs.HasKey("CurrentScore")==false)
        {
            PlayerPrefs.SetInt("CurrentScore", 0);
        }
    }

    /// <summary>
    /// handles score updates when an enemy is defeated
    /// </summary>
    public void EnemyDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", enemiesDefeated);
        }
        
    }

    /// <summary>
    /// quits the game
    /// </summary>
    public void Quit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
