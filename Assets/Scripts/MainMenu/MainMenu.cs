using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //
    // Game Control         ============================================================
    //

    private void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Escape) ) 
           { Application.Quit(); }
    }

    //
    // Game Start Methods    ============================================================
    //

    public void LoadGame()
        { SceneManager.LoadScene("Game"); }
}
