using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    [SerializeField] private GameObject   enemyContainer;
    [SerializeField] private GameObject   powerUpContainer;
    [SerializeField] private GameObject[] powerUpPrefabs;
    [SerializeField] private GameManager  gameManager;
    [SerializeField] private Spawnable[]  spawnableObjects;
    [SerializeField] private int[]        spawnWeight;

    //
    // Data Structures
    //
    //
    //  Add wave controls to this.  eg:
    //    _count
    //    _spawnRateLow/High
    //    _movementSpeed ?
    [System.Serializable]
    private struct Spawnable
    {
        public GameObject _prefab;
        public int        _weight, _waveCount;
        public string     _name, _type;
        public float      _movementSpeed;
//        public float      _spawnRateLow, _spawnRateHigh, _movementSpeed;
        public Spawnable( GameObject prefab
                        , int        weight
                        , string     name
                        , string     type
                        , int        waveCount
 //                       , float      spawnRateLow
 //                       , float      spawnRateHigh
                        , float      movementSpeed
                        )
        {
            _prefab        = prefab;
            _weight        = weight;
            _name          = name;
            _type          = type;
            _waveCount     = waveCount;
 //           _spawnRateLow  = spawnRateLow;
 //           _spawnRateHigh = spawnRateHigh;
            _movementSpeed = movementSpeed;
        }
        public GameObject Prefab        { get { return _prefab;        } set { _prefab        = value; } }
        public string     Name          { get { return _name;          } set { _name          = value; } }
        public string     Type          { get { return _type;          } set { _type          = value; } }
//        public float      SpawnRateLow  { get { return _spawnRateLow;  } set { _spawnRateLow  = value; } }
//        public float      SpawnRateHigh { get { return _spawnRateHigh; } set { _spawnRateHigh = value; } }
        public float      MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
        public int        Weight        { get { return _weight;        } set { _weight        = value; } }
        public int        WaveCount     { get { return _waveCount;     } set { _waveCount     = value; } }
    }
    //
    // Properties
    //
    private int ChoosePowerUpIndex
        { get 
            { 
                int selectIndex = Random.Range(1,56);
            //Debug.Log("Select Index Range: " + selectIndex);
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
        //if (enemyPrefab           == null) { Debug.LogError("Enemy Prefab is NULL"); }
        if (enemyContainer        == null) { Debug.LogError("Enemy Container is NULL"); }
        if (powerUpContainer      == null) { Debug.LogError("PowerUp Container is NULL"); }
        if (gameManager           == null) { Debug.LogError("Game Manager is NULL"); }
        // this does not work if the array size is declared but one or more of the elements is not set.
        if (powerUpPrefabs.Length   == 0)  { Debug.LogError("PowerUp prefabs is EMPTY"); }
        if (spawnableObjects.Length == 0)  { Debug.LogError("Spawnable Objects is EMPTY"); }
    }

    private void Start() 
    {
        NullCheckOnStartup();
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
                SetSpawnWeight();
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
    }

    private void SpawnWave02()
    {
        StartCoroutine(SpawnEnemyRoutine(30, spawnEnemyTimeLow, spawnEnemyTimeHigh));
    }

    private void SpawnWave03()
    {
        StartCoroutine(SpawnEnemyRoutine(30, spawnEnemyTimeLow, spawnEnemyTimeHigh));
    }

    private void SpawnWave04()
    {
        StartCoroutine(SpawnEnemyRoutine(30, spawnEnemyTimeLow, spawnEnemyTimeHigh));
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

    //
    // Need to refactor enemy count if spawning powerups also
    //  If max enemy, no longer spawn enemy  Maybe rebuild SpawnWeight
    //
    private IEnumerator SpawnEnemyRoutine(int enemyCount, float spawnSpeedLow, float spawnSpeedHigh)
    {
        gameManager.CurrentEnemyCount += enemyCount;
        while (enemyCount > 0 && gameManager.GameLive) 
        {
//            SpawnEnemy();
            var index = spawnWeight[Random.Range(0, spawnWeight.Length - 1)];
            GameObject newSpawnable = Instantiate(spawnableObjects[index].Prefab);
            
            if (newSpawnable != null && spawnableObjects[index].Type.ToUpper() == "ENEMY") 
            { 
                newSpawnable.transform.parent = enemyContainer.transform;
                enemyCount--;
            } else if (newSpawnable != null && spawnableObjects[index].Type.ToUpper() == "POWERUP") 
            {
                newSpawnable.transform.parent = powerUpContainer.transform;
            }

            yield return new WaitForSeconds(Random.Range( spawnSpeedLow
                                                        , spawnSpeedHigh));
        }
    }
    private void SetSpawnWeight()
    {
        int i = 0;
        spawnWeight = Enumerable.Repeat<int>(0, 0).ToArray();
        foreach (Spawnable spawnableObject in spawnableObjects)
        {
            spawnWeight = spawnWeight.Concat(Enumerable.Repeat<int>(i, spawnableObject.Weight).ToArray()).ToArray();
            i++;
        }
    }


}
