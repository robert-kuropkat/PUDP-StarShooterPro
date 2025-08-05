using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //
    // Properties
    //

    [SerializeField] private bool gameOver = false;
    public bool GameOver
    {
        get { return gameOver; }
        set 
        { 
            gameOver = value;
            gameLive = !value;
        }
    }

    [SerializeField] private bool gameLive = false;
    public bool GameLive
    {
        get { return gameLive;  }
        set { gameLive = value; }
    }

    [SerializeField] private bool waveOver = true;
    public bool WaveOver
    {
        get { return waveOver; }
        set { 
              if (CurrentWave > 5) { GameOver = true;  } 
              else                 { waveOver = value; }
            }
    }

    [SerializeField] private int currentWave = 0;
    public int CurrentWave
    {
        get { return currentWave; }
        set { currentWave = value; }
    }

    [SerializeField] private static int currentEnemyCount = 0;
    public static int CurrentEnemyCount
    {
        get { return currentEnemyCount;  }
        set { currentEnemyCount = value; }
    }

    //
    // Game Control         ============================================================
    //

    private void Update()
    {
        if (WaveOver && GameLive) { return; }

        if (  Input.GetKeyDown(KeyCode.R)           // Restart game/wave
           && gameOver )
           { SceneManager.LoadScene("Game"); }
        
        if (  Input.GetKeyDown(KeyCode.Escape) )    // Exit game
           {  Application.Quit(); }
        
        if (  currentEnemyCount == 0
           && gameLive )
             { WaveOver = true; }
        else { WaveOver = false; }

    }

}
