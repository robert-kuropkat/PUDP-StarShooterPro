using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //
    // Speed, Boundaries and Timers
    //
    [SerializeField] private float        mySpeed           = 3.5f;
    [SerializeField] private float        currentSpeed      = 0;
    [SerializeField] private int          speedUp           = 1;
    [SerializeField] private float        speedUpTimer      = 5f;
    [SerializeField] private float        leftRightBoundary = 11.2f;
    [SerializeField] private float        topBoundary       = 5;
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
    [SerializeField] private MainCamera   mainCamera;
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
            
            //if (!myWeapons.LaserEnabled)          { return false; }
            if (!Laser.Enabled)                   { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha1)) 
            {
                //myWeapons.DisarmWeapons();
                Debug.Log("Laser Armed"); return true;  
            }
            return false;
        }
    }

    private  bool PlayerHasArmedTripleShot
    {
        get
        {
            
            //if (!myWeapons.TripelShotEnabled)        { return false; }
            if (!TripleShot.Enabled)                 { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha2)) 
            {
                //myWeapons.DisarmWeapons();
                Debug.Log("TripleShot Armed"); return true;  
            }
            return false;
        }
    }

    private bool PlayerHasArmedTorpedo
    {
        get
        {

            //if (!myWeapons.TorpedoEnabled)        { return false; }
            if (!Torpedo.Enabled) { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                //myWeapons.DisarmWeapons();
                Debug.Log("Torpedo Armed"); return true;
            }
            return false;
        }
    }

    private bool PlayerHasArmedSpiralShot
    {
        get
        {
            
            //if (!myWeapons.Laser360Enabled)       { return false; }
            if (!SpiralShot.Enabled)              { return false; }
            if (Input.GetKeyDown(KeyCode.Alpha4)) 
            {
                //myWeapons.DisarmWeapons();
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
            return 0;
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

        Laser.Enabled = true;
        //TripleShot.Enabled = true;
        //Torpedo.Enabled = true;
        //SpiralShot.Enabled = true;
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            //
            //  Use new Enum here...
            //
            case "Enemy":
                TakeDamage();
                break;
            case "Enemy Laser":
                if (other.transform.parent.GetComponent<EnemyFire>().HasHit) { return; }  /// probable when I put it into a container.
                other.transform.parent.GetComponent<EnemyFire>().HasHit = true;
                TakeDamage();
                break;
            case "SpeedPU":
                if (speedUp == 1) { StartCoroutine(PowerUpSpeed()); }
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
                //if (!myWeapons.TripleShotEnabled) { myWeapons.TripleShotEnabled = true; }
                myWeapons.CollectTripelShot();
                break;
            case "TorpedoPU":
                myWeapons.CollectTorpedo();
                break;
            case "SpiralPU":
                //myWeapons.Laser360Enabled = true;
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
        speedUp = 3;  // ToDo: This value should be moved up so it can be changed in the inspector
        yield return new WaitForSeconds(speedUpTimer);
        speedUp = 1;
    }

    //
    // Movement Methods
    //

    private void UpdateThrusterUI()
    {
        int relativeSpeed = 0;
        if      (Mathf.Approximately(currentSpeed, 0))                                      { relativeSpeed = 0;  }
        else if (Mathf.Approximately(currentSpeed, mySpeed))                                { relativeSpeed = 25; }
        else if (Mathf.Approximately(currentSpeed, mySpeed + ThrusterIncrease))             { relativeSpeed = 50; }
        else if (Mathf.Approximately(currentSpeed, mySpeed * speedUp))                      { relativeSpeed = 75; }
        else if (Mathf.Approximately(currentSpeed, (mySpeed + ThrusterIncrease) * speedUp)) { relativeSpeed = 100; }
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
        //
        //  I think this is causing a race condition when two enemy are destroyed
        //  at the same-ish time.  They each pull the same current value so only 
        //  one actually deducts.
        //
        //  Discovered the Current Enemy Count manages to go to -1 which it should
        //  not do...
        //
        //  Move to a method call instead and keep it internal to GameManager
        //
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
