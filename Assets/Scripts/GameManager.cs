using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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

    void Update()
    {
        if (  Input.GetKeyDown(KeyCode.R)
           && gameOver) { SceneManager.LoadScene("Game"); }
    }

}
