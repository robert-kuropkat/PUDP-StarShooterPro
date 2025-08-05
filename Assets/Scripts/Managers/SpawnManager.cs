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
    [SerializeField] private EnemyMinion   enemyMinionLeft;
    [SerializeField] private EnemyMinion   enemyMinionRight;
    [SerializeField] private Boss          enemyBoss;

    //
    // Properties
    //

    private WaveManager currentWave                { get { return waveManager[gameManager.CurrentWave - 1]; } }
    private int         RandomIndex                { get { return spawnWeight[Random.Range(0, spawnWeight.Length - 1)]; } }
    private Spawnable   CurrentSpawnadObject       { get; set; }
    private Transform   CurrentSpawnableContainer  { get; set; }

    //
    // Game Control              ============================================================
    //

    private void NullCheckOnStartup()
    {
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

    private void SpawnBossWave()
    {
        GameManager.CurrentEnemyCount += 1;
        gameManager.WaveOver = false;
        enemyBoss.gameObject.SetActive(true);
        enemyBoss.ChangeStateWithDelay(1f, BossState.Enter);
        StartCoroutine(SpawnMinions());
    }

    //
    // Wave Managers             ============================================================
    // 

    private void SpawnWave()
    {
        gameManager.CurrentWave++;
        if (gameManager.CurrentWave > 4) { SpawnBossWave(); return; }
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

    private IEnumerator SpawnRoutine()
    {
        GameManager.CurrentEnemyCount += currentWave.NumberOfEnemyToSpawn;
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

            if (  CurrentSpawnadObject.Type == SpawnableTypes.Enemy
               && Random.Range(0f,100f)     <= currentWave.ShieldedEnemyPercentage 
               )
               { newSpawnable.GetComponent<Enemy>().ActivateShield(); }
            if (CurrentSpawnadObject.Type == SpawnableTypes.Enemy
               && Random.Range(0f, 100f) <= currentWave.AggressiveEnemyPercentage
               )
               { newSpawnable.GetComponent<Enemy>().ActivateAgression(); }

            yield return new WaitForSeconds(Random.Range( currentWave.SpawnRateRange[0]
                                                        , currentWave.SpawnRateRange[1]));
        }
    }

    private IEnumerator SpawnMinions()
    {
        yield return new WaitForSeconds(5);
        while (!gameManager.GameOver)
            {
                for (int i=0;i<5;i++)
                {
                    GameManager.CurrentEnemyCount++;
                    GameManager.CurrentEnemyCount++;
                    Instantiate(enemyMinionLeft , enemyContainer.transform);
                    Instantiate(enemyMinionRight, enemyContainer.transform);
                    yield return new WaitForSeconds(.5f);
                }
            yield return new WaitForSeconds(20);
        }
    }

}
