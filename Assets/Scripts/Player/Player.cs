using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //
    // Speed, Boundaries and Timers
    //
    [SerializeField] private float        mySpeed            = 3.5f;
    [SerializeField] private float        currentSpeed       = 0;
    [SerializeField] private int          speedUp            = 1;
    [SerializeField] private float        speedUpTimer       = 5f;
    [SerializeField] private float        leftRightBoundary  = 11.2f;
    [SerializeField] private float        topBoundary        = 5;
    [SerializeField] private float        bottomBoundary     = -4;
    [SerializeField] private float        ThrusterIncrease   = 1.1f;
    [SerializeField] private bool         thrusterInCooldown = false;
    [SerializeField] private int          thrusterMax        = 100;
    [SerializeField] private int          thrusterCurrent    = 100;
    [SerializeField] private int          thrusterMin        = 50;
    [SerializeField] private int          thrusterChange     = 0;
    [SerializeField] private float        trusterCheckTimer  = 2.0f;
    //
    // Game Counters
    //
    [SerializeField] private int          playerLives       = 3;
    [SerializeField] private int          playerScore       = 0;
    [SerializeField] private bool         inTestMode        = false;
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
    [SerializeField] private MainCamera   mainCamera;
    [SerializeField] private Canvas       helpScreen;
    //
    // Properties
    //

    //
    // 1. Weapon must be enabled befor eit can be armed.  Weapons should be Disabled when inventory is 0
    // 2. If weapon is enabled, it can be armed with the correct keypress and return a value of true.
    // 3. If weapon is disabled, or if the associated key is not being pressed, a value of false is returned.
    //
    // This does require knowlege of the available weapons be magically hidden here in the player, so I don't 
    //   like it much.  Need to think it through...
    //
    private bool PlayerHasArmedLaser
    {
        get
        {
            
            if (!Laser.Enabled)                   { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha1)) 
            {
                Debug.Log("Laser Armed"); return true;  
            }
            return false;
        }
    }

    private  bool PlayerHasArmedTripleShot
    {
        get
        {
            
            if (!TripleShot.Enabled)                 { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha2)) 
            {
                Debug.Log("TripleShot Armed"); return true;  
            }
            return false;
        }
    }

    private bool PlayerHasArmedTorpedo
    {
        get
        {

            if (!Torpedo.Enabled) { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("Torpedo Armed"); return true;
            }
            return false;
        }
    }

    private bool PlayerHasArmedSpiralShot
    {
        get
        {
            
            if (!SpiralShot.Enabled)              { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha4)) 
            {
                Debug.Log("Spiral Armed"); return true; 
            }
            return false;
        }
    }

    private bool PlayerHasFired  
    { 
        get 
        {
            if (  Input.GetKeyDown(KeyCode.Space) 
               && playerLives > 0 ) { return true; } 
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
            if (Input.GetKey(KeyCode.LeftShift) && !thrusterInCooldown) { return ThrusterIncrease; }
            return 0;
        }
    }

    private bool displayHelp = false;
    private bool DisplayHelp
    {
        get 
        {
            
            if (( Input.GetKey(KeyCode.F1) || Input.GetKey(KeyCode.H) )
               && !displayHelp) { Time.timeScale = 0; return true; }
            Time.timeScale = 1;
            return false;
        }
    }

    //
    // Game Control             ============================================================
    //

    private void NullCheckOnStartup()
    {
        if (spawnManager     == null) { Debug.LogError("The Spawn Manager is NULL."); }
        if (shieldAnim       == null) { Debug.LogError("The Shield Animation is NULL"); }
        if (uiManager        == null) { Debug.LogError("The UI Manager is NULL"); }
        if (gameManager      == null) { Debug.LogError("Game Manager is NULL"); }
    }

    private void RestockPlayer() 
    {
        if (playerLives < 3)  { playerLives = 3; }
        if (Laser.Count < 26) { Laser.Count = 26; }
    }

    private void Start()
    {
        NullCheckOnStartup();
        transform.position = new Vector3(0, 0, 0);
        RestockPlayer();
        StartCoroutine(TrusterMonitor());

        Laser.Enabled = true;
    }

    private void Update()
    {
        MovePlayer();
        CheckBoundaries();

        if (PlayerHasArmedLaser)      { myWeapons.ArmLaser(); }
        if (PlayerHasArmedTripleShot) { myWeapons.ArmTripleShot(); }
        if (PlayerHasArmedSpiralShot) { myWeapons.ArmSpiralShot(); }
        if (PlayerHasArmedTorpedo)    { myWeapons.ArmTorpedo(); }
        
        if (PlayerHasFired)           { myWeapons.FireWeapon(); }
        if (DisplayHelp) { helpScreen.gameObject.SetActive(true); } else { helpScreen.gameObject.SetActive(false); }
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
            case "SpeedPU":
                if (speedUp == 1 && !thrusterInCooldown) { StartCoroutine(PowerUpSpeed()); }
                break;
            case "ShieldPU":
                if (!shieldAnim.IsActive) { shieldAnim.IsActive = true; }
                break;
            case "HealthPU":
                if (playerLives < 3) { CollectHealth(); }
                break;
            
            case "AmmoPU":
                myWeapons.CollectAmmo();
                break;
            case "TripleShotPU":
                myWeapons.CollectTripelShot();
                break;
            case "TorpedoPU":
                myWeapons.CollectTorpedo();
                break;
            case "SpiralPU":
                myWeapons.CollectSpiralShot();
                break;
            case "NegativeAmmoPU":
                myWeapons.ReduceAmmo();
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
        speedUp = 3;
        yield return new WaitForSeconds(speedUpTimer);
        speedUp = 1;
    }

    private IEnumerator TrusterMonitor()
    {
        while (true)
        {
            thrusterCurrent += thrusterChange;
            if (thrusterCurrent > thrusterMax) { thrusterCurrent    = thrusterMax; }
            if (thrusterCurrent <= 0)          { thrusterInCooldown = true; }
            if (thrusterCurrent > thrusterMin) { thrusterInCooldown = false; }
            yield return new WaitForSeconds(trusterCheckTimer);
        }
    }

    //
    // Movement Methods
    //

    private void UpdateThrusterUI()
    {
        int relativeSpeed = 0;
        if      (Mathf.Approximately(currentSpeed, 0))                                      { relativeSpeed = 0;   thrusterChange =  10; }
        else if (Mathf.Approximately(currentSpeed, mySpeed))                                { relativeSpeed = 25;  thrusterChange =   5; }
        else if (Mathf.Approximately(currentSpeed, mySpeed + ThrusterIncrease))             { relativeSpeed = 50;  thrusterChange = -15; }
        else if (Mathf.Approximately(currentSpeed, mySpeed * speedUp))                      { relativeSpeed = 75;  thrusterChange = -20; }
        else if (Mathf.Approximately(currentSpeed, (mySpeed + ThrusterIncrease) * speedUp)) { relativeSpeed = 100; thrusterChange = -25; }
        uiManager.UpdateThrusterGuage(relativeSpeed);
    }

    private void MovePlayer()
    {
        currentSpeed = 0;
        if (  Input.GetAxis("Horizontal") != 0
           || Input.GetAxis("Vertical")   != 0 )
           { currentSpeed = (mySpeed + Thrusters) * speedUp; }
        UpdateThrusterUI();
        transform.Translate(new Vector3( Input.GetAxis("Horizontal")
                                       , Input.GetAxis("Vertical")
                                       , 0
                                       ) * Time.deltaTime * currentSpeed );
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
        GameManager.CurrentEnemyCount--;
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
        mainCamera.PlayerDamage();                          // invoke Main camera shake
        if (playerLives == 1 && inTestMode) { return; } 
        playerLives--;
        uiManager.CurrentLives(playerLives);                // report current lives count to dashboard
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
