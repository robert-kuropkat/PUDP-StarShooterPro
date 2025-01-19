using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    //
    // Boundaries and timers
    //
    [SerializeField] private float        spawnEnemyTimeLow    = 2f;
    [SerializeField] private float        spawnEnemyTimeHigh   = 5f;
    [SerializeField] private float        spawnPowerUpTimeLow  = 5f;
    [SerializeField] private float        spawnPowerUpTimeHigh = 10f;
    [SerializeField] private float        screenLimitLeftRight = 9f;
    [SerializeField] private float        screenLimitTopBottom = 8f;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private GameObject   enemyPrefab;
    [SerializeField] private GameObject   enemyContainer;
    [SerializeField] private GameObject   powerUpContainer;
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField] private GameManager  gameManager;
    //
    // Properties
    //
    private int ChoosePowerUpIndex
        { get 
            { 
                int selectIndex = Random.Range(1,56);
            Debug.Log("Select Index Range: " + selectIndex);
            //
            // Note: This is dependendent on the order these power ups are placed 
            //       in the array in the inspector which is wonky.
            //
            if      (selectIndex > 0  && selectIndex < 10) { return 0; }  // Triple Shot   approx: 17%
            else if (selectIndex > 9  && selectIndex < 20) { return 1; }  // Speed         approx: 17%
            else if (selectIndex > 19 && selectIndex < 30) { return 2; }  // Shield        approx: 17%
            else if (selectIndex > 29 && selectIndex < 40) { return 3; }  // Ammo          approx: 17%
            else if (selectIndex > 39 && selectIndex < 50) { return 4; }  // Health        approx: 17%
            else if (selectIndex > 49 && selectIndex < 55) { return 6; }  // Negative Ammo approx: 09%
            else if (selectIndex > 54 && selectIndex < 57) { return 5; }  // Spiral        approx: 05%
            return 0;
            } 
        }

    //
    // Game Control              ============================================================
    //

    private void NullCheckOnStartup()
    {
        //ToDo:  Add actual error handling rather than just a debug message.
        if (enemyPrefab           == null) { Debug.LogError("Enemy Prefab is NULL"); }
        if (enemyContainer        == null) { Debug.LogError("Enemy Container is NULL"); }
        if (powerUpContainer      == null) { Debug.LogError("PowerUp Container is NULL"); }
        if (gameManager           == null) { Debug.LogError("Game Manager is NULL"); }
        // this does not work if the array size is declared but one or more of the elements is not set.
        if (powerUpPrefabs.Length == 0) { Debug.LogError("PowerUp prefabs is EMPTY"); }
    }

    private void Start() 
    {
        NullCheckOnStartup();
        //SpawnNewWave();
        StartCoroutine(WaitforGameStart());
    }

    private void Update()
    {
        if (  Input.GetKeyDown(KeyCode.R)           // Restart game
           && gameManager.WaveOver)
        {
            gameManager.WaveOver = false;
            gameManager.CurrentWave++;
            SpawnNewWave();
            return;
        }
    }
    public void SpawnNewWave()
    {
        
        switch (gameManager.CurrentWave)
        {
            case 1:
                SpawnWave01();
                break;
            case 2:
                SpawnWave01();
                break;
            default:
                SpawnWave01();
                break;
        }
    }

    //
    // Wave Managers             ============================================================
    // 
    //  Variables need:
    //      - Enemy Count
    //      - Spawn Speed Range (Low/High)

    private void SpawnWave01()
    {
        StartCoroutine(SpawnEnemyRoutine(5, spawnEnemyTimeLow, spawnEnemyTimeHigh));
        //StartCoroutine(SpawnSideEnemyRoutine(2, spawnEnemyTimeLow, spawnEnemyTimeHigh));
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private void SpawnWave02()
    {
        StartCoroutine(SpawnEnemyRoutine(30, spawnEnemyTimeLow, spawnEnemyTimeHigh));
        //StartCoroutine(SpawnSideEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private void SpawnWave03()
    {
        StartCoroutine(SpawnEnemyRoutine(30, spawnEnemyTimeLow, spawnEnemyTimeHigh));
        //StartCoroutine(SpawnSideEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private void SpawnWave04()
    {
        StartCoroutine(SpawnEnemyRoutine(30, spawnEnemyTimeLow, spawnEnemyTimeHigh));
        //StartCoroutine(SpawnSideEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    //
    // Watchdogs                 ============================================================
    //

    //
    // BUG:  These need to end.  Otherwise the methods above will start new ones
    //       leaving these running.
    //       When Enemy count goes to zero it needs to exit.
    //       The problem is we are starting this with the Start routine as well as update.
    //       The while(true) is waiting for real game start (destroiy asteroid).
    //

    private IEnumerator WaitforGameStart()
    {
        while (!gameManager.GameLive) { yield return null; }
        gameManager.WaveOver = false;
        SpawnNewWave();
    }

    private IEnumerator SpawnSideEnemyRoutine(int enemyCount, float spawnSpeedLow, float spawnSpeedHigh)
    {
        gameManager.CurrentEnemyCount += enemyCount;
        while (true)
        {
            while (enemyCount > 0 && gameManager.GameLive)
            {
                SpawnSideEnemy();
                enemyCount--;
                yield return new WaitForSeconds(Random.Range( spawnSpeedLow
                                                            , spawnSpeedHigh));
            }
            yield return null;
        }
    }
    private IEnumerator SpawnEnemyRoutine(int enemyCount, float spawnSpeedLow, float spawnSpeedHigh)
    {
        gameManager.CurrentEnemyCount += enemyCount;
        while (enemyCount > 0 && gameManager.GameLive) 
        {
            SpawnEnemy();
            enemyCount--;
            //Debug.Log("Enemy Count: " + enemyCount);
            yield return new WaitForSeconds(Random.Range( spawnSpeedLow
                                                        , spawnSpeedHigh));
        }
        //Debug.Log("Closing Spawn Enemy Routine");
    }

    private IEnumerator SpawnPowerUpRoutine()
    {
        while (true)
        {
            while (gameManager.GameLive)
            {
                ChoosePowerUp();
                yield return new WaitForSeconds(Random.Range( spawnPowerUpTimeLow
                                                            , spawnPowerUpTimeHigh));
            }
            yield return null;
        }
    }

    //
    // Helper Methods            ============================================================
    //

    private void SpawnSideEnemy()
    {
        GameObject newEnemy = Instantiate(enemyPrefab
                                         , new Vector3( -(screenLimitLeftRight + 2)
                                                      , Random.Range( -screenLimitTopBottom
                                                                    ,  screenLimitTopBottom), 0)
                                         , Quaternion.identity);
        newEnemy.transform.GetComponent<Enemy>().SpawnSide = "ANGLE";
        if (newEnemy != null) { newEnemy.transform.parent = enemyContainer.transform; }
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate( enemyPrefab
                                         , new Vector3(Random.Range(-screenLimitLeftRight
                                                                   , screenLimitLeftRight)
                                                                   , screenLimitTopBottom, 0)
                                         , Quaternion.identity);
        if (newEnemy != null) { newEnemy.transform.parent = enemyContainer.transform; }
    }

    private void ChoosePowerUp()
    {
        GameObject powerUp = Instantiate( powerUpPrefabs[ChoosePowerUpIndex]
                                        , new Vector3(Random.Range(-screenLimitLeftRight
                                                                  , screenLimitLeftRight)
                                                                  , screenLimitTopBottom, 0)
                                        , Quaternion.identity);
        if (powerUp != null) { powerUp.transform.parent = powerUpContainer.transform; }
    }

}
