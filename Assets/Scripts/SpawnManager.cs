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
                int selectIndex = Random.Range(1,51);
            //
            // Note: This is dependendent on the order these power ups are placed 
            //       in the array in the inspector which is wonky.
            //
            if      (selectIndex > 0  && selectIndex < 10) { return 0; }  // Triple Shot approx: 19%
            else if (selectIndex > 9  && selectIndex < 20) { return 1; }  // Speed       approx: 19%
            else if (selectIndex > 19 && selectIndex < 30) { return 2; }  // Shield      approx: 19%
            else if (selectIndex > 29 && selectIndex < 40) { return 3; }  // Ammo        approx: 19%
            else if (selectIndex > 39 && selectIndex < 50) { return 4; }  // Health      approx: 19%
            else if (selectIndex > 49 && selectIndex < 52) { return 5; }  // Spiral      approx: 4%
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
