using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    //
    // Game Objects populated in Inspector
    //

    [SerializeField] private GameObject    enemyContainer;
    [SerializeField] private GameObject    powerUpContainer;
    [SerializeField] private GameManager   gameManager;
    [Tooltip("Create a new element for each distinct wave.")]
    [SerializeField] private WaveManager[] waveManager;
    [SerializeField] private int[]         spawnWeight;

    //
    // Properties
    //

    private WaveManager currentWave
        { get { return waveManager[gameManager.CurrentWave - 1]; } }
    private int RandomIndex
        { get { return spawnWeight[Random.Range(0, spawnWeight.Length - 1)]; } }
    private Spawnable CurrentSpawnadObject 
        { get; set; }
    private Transform CurrentSpawnableContainer 
        { get; set; }

    //
    // Data Structures
    //

    public enum SpawnableTypes
    {
          Enemy
        , PowerUp
    };

    public enum SpwanableNames
    {
          Vertical
        , HorizontalRight
        , HorizontalLeft
        , HorizontalZigZagRight
        , HorizontalZagZagLeft
        , Ammo
        , Speed
        , Shield
        , TripleShot
        , Health
        , Spiral
        , NegativeAmmo
    };

    [System.Serializable]
    private struct WaveManager
    {
        [SerializeField, Tooltip("High and Low value in seconds to spawn next game object.  Array size should be 2.  Element 0 is the lower range, Element 1 the upper.  Values are floats.")] 
        private float[]     _spawnRateRange;
        [SerializeField, Tooltip("Total number of enemy ships to spawn, regardless of type.")]
        private int         _numberOfEnemyToSpawn;
        [SerializeField, Tooltip("Create a new element for each object type to spawn in this wave.  This includes both enemy and powerups.")]
        private Spawnable[] _spawnableObjects;
        public WaveManager( int         numberOfEnemyToSpawn
                          , float[]     spawnRateRange
                          , Spawnable[] spawnableObjects
                          )
        {
            _numberOfEnemyToSpawn = numberOfEnemyToSpawn;
            _spawnRateRange       = spawnRateRange;
            _spawnableObjects     = spawnableObjects;
        }
        public float[] SpawnRateRange       { get { return _spawnRateRange;       } set { _spawnRateRange       = value; } }
        public int     NumberOfEnemyToSpawn { get { return _numberOfEnemyToSpawn; } set { _numberOfEnemyToSpawn = value; } }
        public Spawnable[] SpawnableObjects { get { return _spawnableObjects;     } set { _spawnableObjects     = value; } }
    }

    //
    //  ToDo: Implement mvementSpeed
    //
    [System.Serializable]
    private struct Spawnable
    {
        [SerializeField] private GameObject     _prefab;
        [SerializeField] private int            _weight;
        [SerializeField] private float _movementSpeed;
        [SerializeField] private SpwanableNames _name;
        [SerializeField] private SpawnableTypes _type;
        public Spawnable( GameObject prefab
                        , int        weight
                        , SpwanableNames name
                        , SpawnableTypes type
                        , float      movementSpeed
                        )
        {
            _prefab        = prefab;
            _weight        = weight;
            _name          = name;
            _type          = type;
            _movementSpeed = movementSpeed;
        }
        public GameObject     Prefab        { get { return _prefab;        } set { _prefab        = value; } }
        public SpwanableNames Name          { get { return _name;          } set { _name          = value; } }
        public SpawnableTypes Type          { get { return _type;          } set { _type          = value; } }
        public float          MovementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
        public int            Weight        { get { return _weight;        } set { _weight        = value; } }
    }

    //
    // Game Control              ============================================================
    //

    private void NullCheckOnStartup()
    {
        //ToDo:  Add actual error handling rather than just a debug message.
        if (enemyContainer        == null) { Debug.LogError("Enemy Container is NULL"); }
        if (powerUpContainer      == null) { Debug.LogError("PowerUp Container is NULL"); }
        if (gameManager           == null) { Debug.LogError("Game Manager is NULL"); }
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
            SpawnWave();
            return;
        }
    }

    //
    // Wave Managers             ============================================================
    // 

    private void SpawnWave()
    {
        gameManager.CurrentWave++;
        SetSpawnWeight();
        StartCoroutine(SpawnRoutine());
    }

    private void SetSpawnWeight()
    {
        int i = 0;
        spawnWeight = Enumerable.Repeat<int>(0, 0).ToArray();       // There's got to be a better way to reset this array too zero entries
        foreach (Spawnable spawnableObject in currentWave.SpawnableObjects)
        {
            spawnWeight = spawnWeight.Concat(Enumerable.Repeat<int>(i, spawnableObject.Weight).ToArray()).ToArray();
            i++;
        }
    }

    //
    // Watchdogs                 ============================================================
    //

    private IEnumerator WaitforGameStart()
    {
        while (!gameManager.GameLive) { yield return null; }
        gameManager.WaveOver = false;
        SpawnWave();
    }

    //
    //  ToDo:  Once max enemy have spawned, there are no more powerups because the loop ends.
    //         have not yet decided if this is a feature or bug.
    //  ToDo:  Currently no end of game mangement.  Game will error out if you attempt to 
    //         continue beyond last defined wave.  Have not yet decided if this will be
    //         Game Over, or if we will apply a calculation there after to make it infinite.
    //

    private IEnumerator SpawnRoutine()
    {
        gameManager.CurrentEnemyCount += currentWave.NumberOfEnemyToSpawn;
        gameManager.WaveOver           = false;

        int index;
        while (currentWave.NumberOfEnemyToSpawn > 0 && gameManager.GameLive)
        {
            index = RandomIndex;
            CurrentSpawnadObject = currentWave.SpawnableObjects[index];

            if      ( CurrentSpawnadObject.Type == SpawnableTypes.Enemy   )
            {
                CurrentSpawnableContainer = enemyContainer.transform;
                waveManager[gameManager.CurrentWave - 1].NumberOfEnemyToSpawn--;
            }
            else if ( CurrentSpawnadObject.Type == SpawnableTypes.PowerUp )
            {
                CurrentSpawnableContainer = powerUpContainer.transform;
            }

            GameObject newSpawnable = Instantiate(CurrentSpawnadObject.Prefab, CurrentSpawnableContainer);
            newSpawnable.GetComponent<ISpawnable>().MySpeed = CurrentSpawnadObject.MovementSpeed;

            yield return new WaitForSeconds(Random.Range( currentWave.SpawnRateRange[0]
                                                        , currentWave.SpawnRateRange[1]));
        }
    }

}
