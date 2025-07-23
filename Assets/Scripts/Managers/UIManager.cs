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
    [SerializeField] private TMP_Text    ammoText;
    [SerializeField] private TMP_Text    tripleShotText;
    [SerializeField] private TMP_Text    torpedoText;
    [SerializeField] private TMP_Text    spiralText;
    [SerializeField] private TMP_Text    gameOverText;
    [SerializeField] private TMP_Text    restartText;
    [SerializeField] private Image       livesImage;
    [SerializeField] private Image       spiralShotImage;
    [SerializeField] private Sprite[]    livesSprites;
    [SerializeField] private Sprite[]    spiralSprites;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Slider      thrusterGuage;
    [SerializeField] private Color       thrusterGuageColor = Color.cyan;
    [SerializeField] private Color       defaultTextColor   = new Color(147,171,241);

    //
    // Game Control             ============================================================
    //

    private void NullCheckOnStartup()
    {
        if (scoreText        == null) { Debug.LogError("Score Text is NULL"); }
        if (ammoText         == null) { Debug.LogError("Ammo Text is NULL"); }
        if (torpedoText      == null) { Debug.LogError("Torpedo Text is NULL"); }
        if (spiralText       == null) { Debug.LogError("Spiral Text is NULL"); }
        if (gameOverText     == null) { Debug.LogError("Game Over Text is NULL"); }
        if (restartText      == null) { Debug.LogError("Game Restart Text is NULL"); }
        if (livesImage       == null) { Debug.LogError("Lives Image is NULL"); }
        if (spiralShotImage  == null) { Debug.LogError("Spiral Shot Image is NULL"); }
        if (gameManager      == null) { Debug.LogError("Game Manager is NULL"); }
        // Sprite Array check
    }

    private void Start()
    {
        NullCheckOnStartup();
        InitializeGame();
        StartCoroutine(WatchForGameOver());
        StartCoroutine(WatchForWaveOver());
        //spiralLaserImage.enabled = false;
    }

    //
    // Public Methods           ============================================================
    //

    public void NewScore(int score)         { scoreText.text      = $"{score}"; }
    public void AmmoCount(int count)        { ammoText.text       = $"{count}"; }
    public void TripleShotCount(int count)  { tripleShotText.text = $"{count}"; }
    public void TorpedoCount(int count)     { torpedoText.text    = $"{count}"; }
    public void SpiralLaserCount(int count) { spiralText.text     = $"{count}"; }

    public void CurrentLives (int lives) 
    { 
        if ( lives >= 0 && lives <  livesSprites.Length ) 
           { livesImage.sprite = livesSprites[lives]; }
    }

    public void ArmLaser()          { ammoText.color       = Color.green; }
    public void ArmTripleShot()     { tripleShotText.color = Color.green; }
    public void ArmTorpedo()        { torpedoText.color    = Color.green; }
    public void ArmSpiralLaser()    { spiralText.color     = Color.green; spiralShotImage.sprite = spiralSprites[1]; }
    public void DisArmLaser()       { ammoText.color       = defaultTextColor; }
    public void DisArmTripleShot()  { tripleShotText.color = defaultTextColor; }
    public void DisArmTorpedo()     { torpedoText.color    = defaultTextColor; }
    public void DisArmSpiralLaser() 
    {
        spiralText.color        = defaultTextColor;
        spiralShotImage.sprite = spiralSprites[0];
        spiralShotImage.gameObject.SetActive(true);
    }

    public void UpdateThrusterGuage(int currentSpeed)
    {
        switch (currentSpeed)
        {
            case 0:
                thrusterGuageColor = Color.cyan;
                break;
            case 25:
                thrusterGuageColor = Color.green;
                break;
            case 50:
                thrusterGuageColor = Color.yellow;
                break;
            case 75:
                thrusterGuageColor = new Color(1.0f, 0.64f, 0.0f);
                break;
            case 100:
                thrusterGuageColor = Color.red;
                break;
            default:
                thrusterGuageColor = Color.cyan;
                break;
        }
        thrusterGuage.gameObject.transform.Find("Fill Area").Find("Fill").GetComponent<Image>().color = thrusterGuageColor;
        thrusterGuage.value = currentSpeed;
    }

    public void EnableSpiralLaser()
    {
        spiralShotImage.sprite = spiralSprites[0];
        spiralShotImage.gameObject.SetActive(true); 
    }

    //public void ArmSpiralLaser()
    //{
    //    spiralLaserImage.sprite = spiralSprites[1];
    //}

    public void DisableSpiralLaser() { DisArmSpiralLaser(); }

    public void StartNewWave()
    {
        StopCoroutine(WatchForWaveOver());
    }

    //
    //  Helper Methods
    //

    private void InitializeGame()
    {
        AmmoCount(Laser.Count);
        TripleShotCount(TripleShot.Count);
        TorpedoCount(Torpedo.Count);
        SpiralLaserCount(SpiralShot.Count);
        DisArmTripleShot();
        DisArmTorpedo();
        DisArmSpiralLaser();
        ArmLaser();
        gameOverText.gameObject.SetActive(false);
        scoreText.text = "0";
    }

    //
    // Watchdogs                 ============================================================
    //

    IEnumerator WatchForGameOver()
    {
        while (!gameManager.GameOver) { yield return null; }
        StartCoroutine(DisplayGameOver2());
    }

    IEnumerator WatchForWaveOver()
    {
            while (!gameManager.WaveOver) { yield return null; }
            StartCoroutine(DisplayWaveOver());
    }


    IEnumerator DisplayGameOver2()
    {
        gameOverText.gameObject.SetActive(true);
        restartText.text = "Press the 'R' key to restart the game";
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

    IEnumerator DisplayWaveOver()
    {
        bool flash;
        gameOverText.text = "WAVE OVER";
        restartText.text  = "Press the 'R' key to start the next level";
        gameOverText.gameObject.SetActive(true);
        restartText.gameObject.SetActive(true);
        while (gameManager.WaveOver)
        {
            yield return new WaitForSeconds(0.5f);
            flash = gameOverText.gameObject.activeSelf ? false : true;
            gameOverText.gameObject.SetActive(flash);
        }
        gameOverText.gameObject.SetActive(false);
        restartText.gameObject.SetActive(false);
        StartCoroutine(WatchForWaveOver());
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
