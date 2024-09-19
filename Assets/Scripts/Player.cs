using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //
    // Speed, Boundaries and Timers
    //
    [SerializeField] private float        mySpeed           = 3.5f;
    [SerializeField] private int          speedUp           = 0;
    [SerializeField] private float        fireRate          = .15f;
    [SerializeField] private float        tripleShotTimer   = 5f;
    [SerializeField] private float        speedUpTimer      = 5f;
    [SerializeField] private float        leftRightBoundary = 11.2f;
    [SerializeField] private float        topBoundary       = 0;
    [SerializeField] private float        bottomBoundary    = -4;
    [SerializeField] private float        ThrusterIncrease  = 1.1f;
    [SerializeField] private Vector3      laserOffest       = new Vector3(0, 1.006f, 0);
    //
    // Game Counters
    //
    [SerializeField] private int          playerLives       = 3;
    [SerializeField] private int          playerScore       = 0;
    [SerializeField] private int          ammoCount         = 16;
    //
    // Flags
    //
    [SerializeField] private bool         laserCanFire      = true;
    [SerializeField] private bool         tripleShot        = false;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private UIManager    uiManager;
    [SerializeField] private GameManager  gameManager;
    [SerializeField] private GameObject   laserPrefab;
    [SerializeField] private GameObject   tripleShotPrefab;
    [SerializeField] private Shields      shieldAnim;
    [SerializeField] private Animator     myExplosion;
    [SerializeField] private GameObject[] fireEngineAnims;
    //
    // Properties
    //
    private bool PlayerHasFired  
        { get { return Input.GetKeyDown(KeyCode.Space); } }

    private int ChooseEngine  // The engine fire animations are stored in an array
    {                         // This property randomizes which one is chosen first
        get
        {
            return playerLives == 2
                 ? Random.Range(0, 2)                       // choose random engine on first hit
                 : fireEngineAnims[0].activeSelf ? 1 : 0; ; // choose other (inactive) engine on second
        }
    }

    private float Thrusters
    {
        get
        {
            if (Input.GetKey(KeyCode.LeftShift)) { return ThrusterIncrease; }
            return 1;
        }
    }

    //
    // Game Control             ============================================================
    //

    private void NullCheckOnStartup()
    {
        //ToDo:  Add actual error handling rather than just a debug message.
        if (spawnManager     == null) { Debug.LogError("The Spawn Manager is NULL."); }
        if (laserPrefab      == null) { Debug.LogError("The Laser Prefab is NULL."); }
        if (tripleShotPrefab == null) { Debug.LogError("The TripleShot PowerUp Prefab is NULL"); }
        if (shieldAnim       == null) { Debug.LogError("The Shield Animation is NULL"); }
        if (uiManager        == null) { Debug.LogError("The UI Manager is NULL"); }
        if (gameManager      == null) { Debug.LogError("Game Manager is NULL"); }
    }

    private void Start()
    {
        NullCheckOnStartup();
        uiManager.AmmoCount(ammoCount);
        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        MovePlayer();
        CheckBoundaries();

        if (  PlayerHasFired  // check order.  For fast fail.  Again, a useful optimization!!!
           && laserCanFire 
           ) { FireLaser(); }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Enemy":
                TakeDamage();
                break;
            case "Enemy Laser":
                if (other.transform.parent.GetComponent<EnemyFire>().HasHit) { return; }
                other.transform.parent.GetComponent<EnemyFire>().HasHit = true;
                TakeDamage();
                break;
            case "TripleShotPU":
                if (!tripleShot) { StartCoroutine(PowerUpTripleShot()); }
                break;
            case "SpeedPU":
                if (speedUp == 0) { StartCoroutine(PowerUpSpeed()); }
                break;
            case "ShieldPU":
                if (!shieldAnim.IsActive) { shieldAnim.IsActive = true; }
                break;
            default:
                break;
        }
    }

    //
    // Watchdogs            ============================================================
    //

    private IEnumerator LaserCoolDown()
    {
        yield return new WaitForSeconds(fireRate);
        laserCanFire = true;
    }

    private IEnumerator PowerUpTripleShot()
    {
        tripleShot = true;
        yield return new WaitForSeconds(tripleShotTimer);
        tripleShot = false;
    }

    private IEnumerator PowerUpSpeed()
    {
        speedUp = 3;  // ToDo: This value should be moved up so it can be changed in the inspector
        yield return new WaitForSeconds(speedUpTimer);
        speedUp = 0;
    }

    //
    // Helper Methods           ============================================================
    //

    //
    // Movement Methods
    //

    private void MovePlayer()
    {
        transform.Translate(new Vector3( Input.GetAxis("Horizontal")
                                       , Input.GetAxis("Vertical")
                                       , 0
                                       ) * Time.deltaTime * (mySpeed+speedUp) * Thrusters);
    }

    private void CheckBoundaries()
    {
        transform.position = new Vector3( CheckLeftRight(transform.position.x)
                                        , CheckTopBottom(transform.position.y)
                                        , 0);
    }

    private float CheckLeftRight(float x) 
        { return Mathf.Abs(x) < leftRightBoundary ? x : -x;  }

    private float CheckTopBottom(float y) 
        { return Mathf.Clamp(y, bottomBoundary, topBoundary);  }

    //
    // Damage Methods
    //

    private void FireLaser()
    {
        if (playerLives < 1) { return; }
        if (ammoCount   < 1) { return; }
        ammoCount--;
        uiManager.AmmoCount(ammoCount);

        Instantiate((tripleShot) ? tripleShotPrefab : laserPrefab
                   , transform.position + laserOffest
                   , Quaternion.identity);
        laserCanFire = false;
        StartCoroutine(LaserCoolDown());
    }

    public void EnemyDestroyed()
    {
        playerScore++;
        uiManager.NewScore(playerScore);
    }

    private void TakeDamage()
    {
        /*
        if (shieldAnim.IsActive)
        {
            //shieldAnim.TakesHit();
            return;
        }
        */

        playerLives--;
        uiManager.CurrentLives(playerLives);            // report current lives count to dashboard
        if (playerLives < 1) { DeathScene(); return; }
        fireEngineAnims[ChooseEngine].SetActive(true);  // damage animation
    }

    private void DeathScene()
    {
        gameManager.GameOver = true;
        foreach (Transform child in transform) { Destroy(child.gameObject); }      // Destroy thrusters, wing damage, etc. child objects
        Collider2D  myCollider = GetComponent<Collider2D>();                       // Disable collider to prevent "false" collisions
        Renderer   myRenderer  = GetComponent<Renderer>();
        if (myCollider == null) { Debug.LogError("Player Collider2D is NULL"); }
                           else { myCollider.enabled = false; }
        if (myRenderer == null) { Debug.LogError("Player Renderer is NULL"); }
                           else { myRenderer.enabled = false; }
        Instantiate(myExplosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 3f);
    }
}
