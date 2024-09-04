using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //
    // Properties
    //

    private bool gameOver = false;
    public bool GameOver
    {
        get { return gameOver; }
        set 
        { 
            gameOver = value;
            gameLive = !value;
        }
    }

    private bool gameLive = false;
    public bool GameLive
    {
        get { return gameLive;  }
        set { gameLive = value; }
    }

    //
    // Game Control         ============================================================
    //

    private void Update()
    {
        if (  Input.GetKeyDown(KeyCode.R)           // Restart game
           && gameOver ) 
           {  SceneManager.LoadScene("Game"); }
        if (  Input.GetKeyDown(KeyCode.Escape) )    // Exit game
           {  Application.Quit(); }
    }

}
