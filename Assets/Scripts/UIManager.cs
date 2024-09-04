using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    //
    // Timers
    //
    [SerializeField] private float       gameOverDelay = 0.4f;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private TMP_Text    scoreText;
    [SerializeField] private TMP_Text    gameOverText;
    [SerializeField] private TMP_Text    restartText;
    [SerializeField] private Image       livesImage;
    [SerializeField] private Sprite[]    livesSprites;
    [SerializeField] private GameManager gameManager;

    //
    // Game Control             ============================================================
    //

    private void NullCheckOnStartup()
    {
        if (scoreText    == null) { Debug.LogError("Score Text is NULL"); }
        if (gameOverText == null) { Debug.LogError("Game Over Text is NULL"); }
        if (restartText  == null) { Debug.LogError("Game Restart Text is NULL"); }
        if (livesImage   == null) { Debug.LogError("Lives Image is NULL"); }
        if (gameManager  == null) { Debug.LogError("Game Manager is NULL"); }
        // Sprite Array check
    }

    private void Start()
    {
        NullCheckOnStartup();
        InitializeGame();
        StartCoroutine(WatchForGameOver());
    }

    //
    // Public Methods           ============================================================
    //

    public void NewScore(int score)      
        { scoreText.text = "Score: " + score; }

    public void CurrentLives (int lives) 
        { if (  lives >= 0
             && lives <  livesSprites.Length ) { livesImage.sprite = livesSprites[lives]; }}

    //
    //  Helper Methods
    //

    private void InitializeGame()
    {
        gameOverText.gameObject.SetActive(false);
        scoreText.text = "Score: 0";
    }

    //
    // Watchdogs                 ============================================================
    //

    IEnumerator WatchForGameOver()
    {
        while (!gameManager.GameOver) { yield return null; }
        StartCoroutine(DisplayGameOver2());
    }

    IEnumerator DisplayGameOver2()
    {
        gameOverText.gameObject.SetActive(true);
        while (gameManager.GameOver)
        {
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "G";
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "GA";
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "GAM";
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "GAME";
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "GAME O";
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "GAME OV";
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "GAME OVE";
            yield return new WaitForSeconds(gameOverDelay);
            gameOverText.text = "GAME OVER";
            restartText.gameObject.SetActive(true);
            yield return new WaitForSeconds(gameOverDelay);
        }
    }

    //
    // Unused                ============================================================
    //

    IEnumerator DisplayGameOver()
    {
        bool flash;
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        while (gameManager.GameOver)
        {
            yield return new WaitForSeconds(0.5f);
            flash = gameOverText.gameObject.activeSelf ? false : true;
            gameOverText.gameObject.SetActive(flash);
        }
    }

    IEnumerator DisplayGameOver3()
    {
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        float delay = .05f;
        while (gameManager.GameOver)
        {
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GR";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAER";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAMVER";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAMEOVER";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(.3f);
        }
    }

}
