using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int enemiesDefeated;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("HighScore") == false)
        {
            PlayerPrefs.SetInt("HighScore", 0);
        }
    }

    public void enemyDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", enemiesDefeated);
        }
        
    }

    public void Quit()
    {
        Debug.Log("Quitting...");
        Application.Quit();
    }
}
