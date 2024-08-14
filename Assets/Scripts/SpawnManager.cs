using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private bool       keepSpawning = true;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject tripleShotPUPrefab;
    [SerializeField] private GameObject enemyContainer;
    [SerializeField] private GameObject powerUpContainer;

    private void Start() 
    {
        //ToDo:  Add actual error handling rather than just a debug message.
        if (enemyPrefab        == null) { Debug.LogError("Enemy Prefab is NULL");    }
        if (enemyContainer     == null) { Debug.LogError("Enemy Container is NULL"); }
        if (tripleShotPUPrefab == null) { Debug.LogError("TripleShot PowerUp prefab is NULL"); }
        if (powerUpContainer   == null) { Debug.LogError("PowerUp Container is NULL"); }

        StartCoroutine(SpawnRoutine());
        StartCoroutine(SpawnPowerUp());
    }

    public void PlayerDied() 
            { keepSpawning = false; }

    private IEnumerator SpawnRoutine()
    {
        while (keepSpawning)
        {
            GameObject newEnemy = Instantiate( enemyPrefab
                                             , new Vector3(Random.Range(-11f, 11f), 8, 0)
                                             , Quaternion.identity);
            if (newEnemy != null) { newEnemy.transform.parent = enemyContainer.transform; }
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    private IEnumerator SpawnPowerUp()
    {
        while (keepSpawning)
        {
            GameObject powerUp = Instantiate( tripleShotPUPrefab
                                            , new Vector3(Random.Range(-11f, 11f), 8, 0)
                                            , Quaternion.identity);
            if (powerUp != null) { powerUp.transform.parent = powerUpContainer.transform; }
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }
}
