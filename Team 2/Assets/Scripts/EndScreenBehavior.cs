/*****************************************************************************
// File Name :         EndScreenBehavior.cs
// Author :            Cade R. Naylor
// Creation Date :     January 28, 2024
//
// Brief Description : Displays the player's score

*****************************************************************************/
using TMPro;
using UnityEngine;

public class EndScreenBehavior : MonoBehaviour
{
    [SerializeField]
    private TMP_Text highScore;
    [SerializeField]
    private TMP_Text recentScore;

    void Start()
    {
        highScore.text = "Your High Score:\n" + PlayerPrefs.GetInt("HighScore");
        recentScore.text = "Your Recent Score:\n" + GameManager.enemiesDefeated;
    }
}
