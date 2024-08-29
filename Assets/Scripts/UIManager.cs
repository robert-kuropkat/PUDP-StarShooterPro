using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text    scoreText;
    [SerializeField] private TMP_Text    gameOverText;
    [SerializeField] private TMP_Text    restartText;
    [SerializeField] private Image       livesImage;
    [SerializeField] private Sprite[]    livesSprites;
    [SerializeField] private GameManager gameManager;

    private Text testText;  // ToDo: ??? Remove ???

    void Start()
    {
        if (gameManager == null) { Debug.LogError("Game Manager is NULL"); }

        gameOverText.gameObject.SetActive(false);
        scoreText.text = "Score: 0";
        StartCoroutine(WatchForGameOver());
    }

    public void NewScore(int score)      
        { scoreText.text = "Score: " + score; }

    public void CurrentLives (int lives) 
        { livesImage.sprite = livesSprites[lives]; }

    IEnumerator WatchForGameOver()
    {
        while (!gameManager.GameOver) { yield return null; }
        StartCoroutine(DisplayGameOver2());
    }
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

    IEnumerator DisplayGameOver2()
    {
        gameOverText.gameObject.SetActive(true);
        float delay = .4f;
        while (gameManager.GameOver)
        {
            yield return new WaitForSeconds(delay);
            gameOverText.text = "G";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GA";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAM";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAME";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAME O";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAME OV";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAME OVE";
            yield return new WaitForSeconds(delay);
            gameOverText.text = "GAME OVER";
            restartText.gameObject.SetActive(true);
            yield return new WaitForSeconds(.3f);
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
