using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float        mySpeed           = 3.5f;
    [SerializeField] private float        fireRate          = .15f;
    [SerializeField] private float        tripleShotTimer   = 5f;
    [SerializeField] private float        speedUpTimer      = 5f;
    [SerializeField] private float        shieldsUpTimer    = 5f;
    [SerializeField] private float        leftRightBoundary = 11.2f;
    [SerializeField] private float        topBoundary       = 0;
    [SerializeField] private float        bottomBoundary    = -4;
    [SerializeField] private int          playerLives       = 3;
    [SerializeField] private int          speedUp           = 0;
    [SerializeField] private int          playerScore       = 0;
    [SerializeField] private bool         laserCanFire      = true;
    [SerializeField] private bool         tripleShot        = false;
    [SerializeField] private bool         shieldsUp         = false;
    [SerializeField] private Vector3      laserOffest       = new Vector3(0, 1.006f, 0);
    [SerializeField] private GameObject   laserPrefab;
    [SerializeField] private GameObject   tripleShotPrefab;
    [SerializeField] private GameObject   shieldAnim;
    [SerializeField] private GameObject[] fireEngineAnims;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private UIManager    uiManager;
    [SerializeField] private GameManager  gameManager;
    [SerializeField] private Animator     myExplosion;



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

    private void Start()
    {
        //ToDo:  Add actual error handling rather than just a debug message.
        if (spawnManager     == null) { Debug.LogError("The Spawn Manager is NULL."); }
        if (laserPrefab      == null) { Debug.LogError("The Laser Prefab is NULL."); }
        if (tripleShotPrefab == null) { Debug.LogError("The TripleShot PowerUp Prefab is NULL"); }
        if (shieldAnim       == null) { Debug.LogError("The Shield Animation is NULL"); }
        if (uiManager        == null) { Debug.LogError("The UI Manager is NULL"); }
        if (gameManager      == null) { Debug.LogError("Game Manager is NULL"); }

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

    public void EnemyDestroyed ()
    {
        playerScore++;
        UIManager myUI = uiManager.GetComponent<UIManager>();
        if (myUI == null) { Debug.LogError("UI game object is NULL"); }
                     else { myUI.NewScore(playerScore); }
    }

    private void MovePlayer()
    {
        transform.Translate(new Vector3( Input.GetAxis("Horizontal")
                                       , Input.GetAxis("Vertical")
                                       , 0
                                       ) * Time.deltaTime * (mySpeed+speedUp));
    }

    private void CheckBoundaries()
    {
        transform.position = new Vector3( CheckLeftRight(transform.position.x)
                                        , CheckTopBottom(transform.position.y)
                                        , 0);
    }

    private float CheckLeftRight(float x) 
    {
        return Mathf.Abs(x) < leftRightBoundary ? x : -x; 
    }

    private float CheckTopBottom(float y) 
    {
        return Mathf.Clamp(y, bottomBoundary, topBoundary); 
    }

    private void FireLaser()
    {
        if (tripleShot)
        {
            Instantiate( tripleShotPrefab
                       , transform.position + laserOffest
                       , Quaternion.identity);
        } else {
            Instantiate( laserPrefab
                       , transform.position + laserOffest
                       , Quaternion.identity);
        }
        laserCanFire = false;
        StartCoroutine(LaserCoolDown());
    }

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
        speedUp = 3;
        yield return new WaitForSeconds(speedUpTimer);
        speedUp = 0;
    }

    private IEnumerator PowerUpShield()
    {
        shieldsUp = true;
        shieldAnim.SetActive(true);
        yield return new WaitForSeconds(shieldsUpTimer);
        shieldsUp = false;
        shieldAnim.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)        
    { 
        switch (other.tag)
        {
            case "Enemy":
                TakeDamage();
                break;
            case "TripleShotPU":
                if (!tripleShot)  { StartCoroutine(PowerUpTripleShot()); }
                break;
            case "SpeedPU":
                if (speedUp == 0) { StartCoroutine(PowerUpSpeed()); }
                break;
            case "ShieldPU":
                if (!shieldsUp)   { StartCoroutine(PowerUpShield()); }
                break;
            default:
                break;
        }
    }

    private void TakeDamage()
    {
        if (shieldsUp)
        {
            shieldsUp = false;
            shieldAnim.SetActive(false);
            return;
        }

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
