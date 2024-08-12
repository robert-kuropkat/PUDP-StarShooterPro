using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private bool       keepSpawning = true;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject enemyContainer;

    void Start() 
    {
        if (enemyPrefab    == null) { Debug.LogError("Enemy Prefab is NULL");    }
        if (enemyContainer == null) { Debug.LogError("Enemy Container is NULL"); }

        StartCoroutine(SpawnRoutine()); 
    }

    public void PlayerDied() 
            { keepSpawning = false; }

    IEnumerator SpawnRoutine()
    {
        while (keepSpawning)
        {
            GameObject newEnemy       = Instantiate( enemyPrefab
                                                   , new Vector3(Random.Range(-11f, 11f), 8, 0)
                                                   , Quaternion.identity);
            newEnemy.transform.parent = enemyContainer.transform;
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }
}
