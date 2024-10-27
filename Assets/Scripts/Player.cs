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
    [SerializeField] private float        speedUpTimer      = 5f;
    [SerializeField] private float        leftRightBoundary = 11.2f;
    [SerializeField] private float        topBoundary       = 0;
    [SerializeField] private float        bottomBoundary    = -4;
    [SerializeField] private float        ThrusterIncrease  = 1.1f;
    //
    // Game Counters
    //
    [SerializeField] private int          playerLives       = 3;
    [SerializeField] private int          playerScore       = 0;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private UIManager    uiManager;
    [SerializeField] private GameManager  gameManager;
    [SerializeField] private Shields      shieldAnim;
    [SerializeField] private Animator     myExplosion;
    [SerializeField] private Weapons      myWeapons;
    [SerializeField] private GameObject[] fireEngineAnims;
    //
    // Properties
    //

    private bool PlayerHasEnabled360Laser
    {
        get
        {
            if (  Input.GetKeyDown(KeyCode.LeftAlt )
               || Input.GetKeyDown(KeyCode.RightAlt))
            {
                return true;
            } else { return false; }
        }
    }
    private bool PlayerHasFired  
        { get {
                if (  Input.GetKeyDown(KeyCode.Space) 
                   && playerLives > 0) { return true; } 
                return false;
              } 
        }

    private int ChooseGoodEngine  // The engine fire animations are stored in an array
    {                             // This property randomizes which one is chosen first
        get
        {
            return playerLives == 2
                 ? Random.Range(0, 2)                       // choose random engine on first hit
                 : fireEngineAnims[0].activeSelf ? 1 : 0; ; // choose other (inactive) engine on second
        }
    }

    private int ChooseDamagedEngine  // The engine fire animations are stored in an array
    {                             // This property randomizes which one is chosen first
        get
        {
            return playerLives == 2
                 ? Random.Range(0, 2)                       // choose random engine on first hit
                 : !fireEngineAnims[0].activeSelf ? 1 : 0; ; // choose other (inactive) engine on second
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
        if (shieldAnim       == null) { Debug.LogError("The Shield Animation is NULL"); }
        if (uiManager        == null) { Debug.LogError("The UI Manager is NULL"); }
        if (gameManager      == null) { Debug.LogError("Game Manager is NULL"); }
    }

    private void Start()
    {
        NullCheckOnStartup();
        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        MovePlayer();
        CheckBoundaries();
        // If collected and enabled myWeapons.FireWeapon()
        if (PlayerHasFired) { myWeapons.FireWeapon(); }
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
                if (!myWeapons.TripleShotEnabled) { myWeapons.TripleShotEnabled = true; }
                break;
            case "SpeedPU":
                if (speedUp == 0) { StartCoroutine(PowerUpSpeed()); }
                break;
            case "ShieldPU":
                if (!shieldAnim.IsActive) { shieldAnim.IsActive = true; }
                break;
            case "AmmoPU":
                myWeapons.CollectAmmo();
                break;
            case "HealthPU":
                if (playerLives < 3) { CollectHealth();  }
                break;
            default:
                break;
        }
    }

    //
    // Watchdogs            ============================================================
    //

    private IEnumerator PowerUpSpeed()
    {
        speedUp = 3;  // ToDo: This value should be moved up so it can be changed in the inspector
        yield return new WaitForSeconds(speedUpTimer);
        speedUp = 0;
    }

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
    // Damage/Health Methods
    //

    public void EnemyDestroyed()
    {
        playerScore++;
        uiManager.NewScore(playerScore);
    }

    private void CollectHealth()
    {
        playerLives++;
        uiManager.CurrentLives(playerLives);
        fireEngineAnims[ChooseDamagedEngine].SetActive(false);  // remove damage animation
    }

    private void TakeDamage()
    {
        playerLives--;
        uiManager.CurrentLives(playerLives);            // report current lives count to dashboard
        if (playerLives < 1) { DeathScene(); return; }
        fireEngineAnims[ChooseGoodEngine].SetActive(true);  // damage animation - Need to reverse the ChooseEngine logic.  New getter?
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
