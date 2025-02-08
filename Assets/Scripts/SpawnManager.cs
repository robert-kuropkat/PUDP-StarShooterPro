using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    //
    // Boundaries and timers
    //
    //
    // Game Objects populated in Inspector
    //
    //  Wave Manager probalby needs to be an array...
    //
    [SerializeField] private GameObject enemyContainer;
    [SerializeField] private GameObject powerUpContainer;
    //[SerializeField] private GameObject[]  powerUpPrefabs;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private WaveManager[] currentWave;
    [SerializeField] private Spawnable[] spawnableObjects;
    [SerializeField] private Spawnable[] spawnableObjects_2;
    [SerializeField] private Spawnable[] spawnableObjects_3;
    [SerializeField] private Spawnable[] spawnableObjects_4;
    [SerializeField] private int[] spawnWeight;
    [SerializeField] private object[] waveSpawnables;

    //
    //  Possibly create an array we can loop through
    //  waveSpawnables[] = [spawnableObjects, spawnableObjects_2, spawnableObjects_3, spawnableObjects_4]
    //  That we can loop throuhg
    //  Then possibly, loop again applying some math to the values to make it endless
    //

    //
    // Data Structures
    //

    [System.Serializable]
    public struct WaveManager
    {
        public float[] _spawnRateRange;
        public int     _numberOfEnemyToSpawn;
        public WaveManager ( int     numberOfEnemyToSpawn
                           , float[] spawnRateRange
                           )
        {
            _numberOfEnemyToSpawn = numberOfEnemyToSpawn;
            _spawnRateRange       = spawnRateRange;
            }
        public float[] SpawnRateRange       { get { return _spawnRateRange;       } set { _spawnRateRange       = value; } }
        public int     NumberOfEnemyToSpawn { get { return _numberOfEnemyToSpawn; } set { _numberOfEnemyToSpawn = value; } }
    }

    //
    //  ToDo: Implement mvementSpeed
    //
    [System.Serializable]
    private struct Spawnable
    {
        public GameObject _prefab;
        public int        _weight;
        public string     _name, _type;
        public float      _movementSpeed;
        public Spawnable( GameObject prefab
                        , int        weight
                        , string     name
                        , string     type
                        , float      movementSpeed
                        )
        {
            _prefab        = prefab;
            _weight        = weight;
            _name          = name;
            _type          = type;
            _movementSpeed = movementSpeed;
        }
        public GameObject Prefab        { get { return _prefab;        } set { _prefab        = value; } }
        public string     Name          { get { return _name;          } set { _name          = value; } }
        public string     Type          { get { return _type;          } set { _type          = value; } }
        public float      MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
        public int        Weight        { get { return _weight;        } set { _weight        = value; } }
    }

    //
    // Properties
    //

    //
    // Game Control              ============================================================
    //

    private void NullCheckOnStartup()
    {
        //ToDo:  Add actual error handling rather than just a debug message.
        if (enemyContainer        == null) { Debug.LogError("Enemy Container is NULL"); }
        if (powerUpContainer      == null) { Debug.LogError("PowerUp Container is NULL"); }
        if (gameManager           == null) { Debug.LogError("Game Manager is NULL"); }
        // this does not work if the array size is declared but one or more of the elements is not set.
        //if (powerUpPrefabs.Length   == 0)  { Debug.LogError("PowerUp prefabs is EMPTY"); }
        if (spawnableObjects.Length == 0)  { Debug.LogError("Spawnable Objects is EMPTY"); }
    }

    private void Start() 
    {
        NullCheckOnStartup();
        StartCoroutine(WaitforGameStart());
        waveSpawnables[0] = spawnableObjects;
        //waveSpawnables[1] = spawnableObjects_2;
        //waveSpawnables[2] = spawnableObjects_3;
        //waveSpawnables[3] = spawnableObjects_4;
    }

    private void Update()
    {
        if (  Input.GetKeyDown(KeyCode.R)           // Restart game
           && gameManager.WaveOver)
        {
            SpawnWave();
            return;
        }
    }

    //
    // Wave Managers             ============================================================
    // 

    private void SpawnWave()
    {
        SetSpawnWeight();
        gameManager.CurrentWave++;
        StartCoroutine(SpawnRoutine());
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
        SpawnWave();
    }

    //
    // Need to refactor enemy count if spawning powerups also
    //  If max enemy, no longer spawn enemy  Maybe rebuild SpawnWeight
    //
    //  ToDo:  Once max enemy have spawned, there are no more powerups because the loop ends.
    //
    private IEnumerator SpawnRoutine()
    {
        gameManager.CurrentEnemyCount += currentWave[gameManager.CurrentWave -1].NumberOfEnemyToSpawn;
        gameManager.WaveOver = false;
        while (currentWave[gameManager.CurrentWave - 1].NumberOfEnemyToSpawn > 0 && gameManager.GameLive) 
        {
            var index = spawnWeight[Random.Range(0, spawnWeight.Length - 1)];
            GameObject newSpawnable = Instantiate(spawnableObjects[index].Prefab);
            newSpawnable.GetComponent<ISpawnable>().MySpeed = spawnableObjects[index].MovementSpeed;

            if (newSpawnable != null && spawnableObjects[index].Type.ToUpper() == "ENEMY") 
            { 
                newSpawnable.transform.parent = enemyContainer.transform;
                currentWave[gameManager.CurrentWave - 1].NumberOfEnemyToSpawn--;
            } else if (newSpawnable != null && spawnableObjects[index].Type.ToUpper() == "POWERUP") 
            {
                newSpawnable.transform.parent = powerUpContainer.transform;
            }

            yield return new WaitForSeconds(Random.Range( currentWave[gameManager.CurrentWave - 1].SpawnRateRange[0]
                                                        , currentWave[gameManager.CurrentWave - 1].SpawnRateRange[1]));
        }
    }
    private void SetSpawnWeight()
    {
        int i = 0;
        spawnWeight = Enumerable.Repeat<int>(0, 0).ToArray();
        //foreach (Spawnable spawnableObject in spawnableObjects)
        Spawnable nextWave = Spawnable[] waveSpawnables[0];
        foreach (Spawnable spawnableObject in )
        {
            spawnWeight = spawnWeight.Concat(Enumerable.Repeat<int>(i, spawnableObject.Weight).ToArray()).ToArray();
            i++;
        }
    }


}
