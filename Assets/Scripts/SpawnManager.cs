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
    [SerializeField] private float        screenLimitLeftRight = 11f;
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
        { get { return Random.Range(0, powerUpPrefabs.Length);  } }

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
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    //
    // Watchdogs                 ============================================================
    //

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            while (gameManager.GameLive) 
            { 
                SpawnEnemy();
                yield return new WaitForSeconds(Random.Range( spawnEnemyTimeLow
                                                            , spawnEnemyTimeHigh));
            }
            yield return null;
        }
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
