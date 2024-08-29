using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject   enemyPrefab;
    [SerializeField] private GameObject   enemyContainer;
    [SerializeField] private GameObject   powerUpContainer;
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField] private GameManager  gameManager;

    private void Start() 
    {
        NullCheckOnStartup();

        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerUpRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            while (gameManager.GameLive) 
            { 
                SpawnEnemy();
                yield return new WaitForSeconds(Random.Range(2f, 5f));
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
                yield return new WaitForSeconds(Random.Range(5f, 10f));
            }
            yield return null;
        }
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate( enemyPrefab
                                         , new Vector3(Random.Range(-11f, 11f), 8, 0)
                                         , Quaternion.identity);
        if (newEnemy != null) { newEnemy.transform.parent = enemyContainer.transform; }
    }

    private void ChoosePowerUp()
    {
        GameObject powerUp = Instantiate( powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)]
                                        , new Vector3(Random.Range(-11f, 11f), 8, 0)
                                        , Quaternion.identity);
        if (powerUp != null) { powerUp.transform.parent = powerUpContainer.transform; }
    }

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
}
