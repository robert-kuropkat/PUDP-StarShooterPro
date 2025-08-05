using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, ISpawnable
{
    //
    // Template
    //
    abstract public    float   MySpeed { get; set; }
    abstract protected Vector3 SpawnPosition { get; }
    abstract protected void    MoveMe();
    abstract protected void    Update();
    abstract protected void    Teleport();

    //
    // Timers
    //
    [SerializeField] private float laserLowerTimer = 2.0f;
    [SerializeField] private float laserUpperTimer = 5.0f;
    [SerializeField] private float explosionTimer  = 2.4f;

    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private EnemyShields     shieldAnim;
    [SerializeField] private CircleCollider2D proximityCollider;


    //
    // Game Objects non-Serialized objects are populated in code
    //
    [SerializeField] private GameObject laserPrefab;
                     private Player     myPlayer;
                     private Animator   myExplosion_anim;

    //
    // Properties
    //
    protected Vector3 chaseVector = Vector3.zero;
    protected bool ImDead { get; set; } = false;
    protected bool PowerUpDetected { get; set; } = false;

    [SerializeField] protected Boundary ScreenBoundary          = new Boundary( 9.5f, 6.0f);
    [SerializeField] protected Boundary HorizontalSpawnBoundary = new Boundary(11.5f, 5.0f);
    [SerializeField] protected Boundary VerticalSpawnBoundary   = new Boundary( 8.5f, 8.0f);

    //
    // Game Loop              ============================================================
    //
    private void NullCheckOnStartup()
    {
        if (myPlayer         == null) { Debug.LogError("The Player is NULL."); }
        if (laserPrefab      == null) { Debug.LogError("The Enemy Laser Prefab is NULL."); }
        if (myExplosion_anim == null) { Debug.LogError("The Explosion Animator is NULL."); }
        if (shieldAnim       == null) { Debug.LogError("The Shield Animation is NULL"); }
    }

    protected virtual void Start() 
    { 
        myPlayer           = GameObject.Find("Player").GetComponent<Player>();
        myExplosion_anim   = GetComponent<Animator>();

        NullCheckOnStartup();
        StartCoroutine(FireLaser());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (  other.tag == "Player"
           || other.tag == "Laser"
           || other.tag == "Shield"
           ) { EnemyDeathScene(); }
    }

    //
    // Watchdogs                 ============================================================
    //

    IEnumerator FireLaser()
    {
        //
        // This first yield keeps the enemy from firing as soon as it is instantiated.
        // The second yield must follow the laser instantiation to avoid firing after
        // the enemy has been "destroyed" and is just going through its explosion
        // animation.
        //
        yield return new WaitForSeconds(Random.Range(laserLowerTimer, laserUpperTimer));
        while (!ImDead)
        {
            //
            // If enemy is below the player, in particular off screen, then don't bother firing.
            // it's harmless, but irritating, especially when the laser fires off screen and can't
            // be seen but can be heard.
            //
            if (  myPlayer != null 
               && transform.position.y > -ScreenBoundary.Y)
            {
                Instantiate( laserPrefab
                           , transform.position
                           , Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(laserLowerTimer, laserUpperTimer));
        }
    }

    IEnumerator FireOnce()
    {
        Instantiate( laserPrefab
                   , transform.position
                   , Quaternion.identity);
        yield return new WaitForSeconds(3);
        PowerUpDetected = false;
    }

    //
    // Helper Methods            ============================================================
    //

    private void EnemyDeathScene()
    {
        if (shieldAnim.IsActive) { return; }
        NotifyPlayer();
        ImDead = true; // Flag to short circuit respawning in Update()
        DisableCollisionComponenets();
        TriggerExplosion();
        Destroy(this.gameObject, explosionTimer);
    }

    private void DisableCollisionComponenets()
    {
        Collider2D  myCollider  = GetComponent<Collider2D>();
        AudioSource myAudio     = GetComponent<AudioSource>();
        if (myCollider == null) { Debug.LogError("Enemy Collider2D is NULL"); }
                           else { myCollider.enabled = false; }
        if (myAudio    == null) { Debug.LogError("Enemy Audio Source is NULL"); }
                           else { myAudio.Play(); }
    }

    private void TriggerExplosion() { myExplosion_anim.SetTrigger("OnEnemyDeath"); }

    private void NotifyPlayer()     { myPlayer.EnemyDestroyed(); }

    public  void ActivateShield()   { shieldAnim.IsActive = true; }

    private void DeacivateShield()  { shieldAnim.IsActive = false; }

    public void ActivateAgression() { proximityCollider.enabled = true; }

    public void DeactivateAgression() { proximityCollider.enabled = false; }

    public void ChasePlayer()
    {
        chaseVector.x = (myPlayer.transform.position.x - transform.position.x) / proximityCollider.radius;
        chaseVector.y = (myPlayer.transform.position.y - transform.position.y) / proximityCollider.radius;
    }

    public void DuckAndDodge()
    {
        chaseVector.x = -(myPlayer.transform.position.x - transform.position.x);
        StartCoroutine(ResetDuckAndDodge());
    }

    IEnumerator ResetDuckAndDodge()
    {
        yield return new WaitForSeconds(3);
        chaseVector.x = 0;
    }

    protected void CheckBoundaries()
    {
        if (Mathf.Abs(transform.position.y) > VerticalSpawnBoundary.Y  ) { Teleport(); chaseVector = Vector3.zero; }
        if (Mathf.Abs(transform.position.x) > HorizontalSpawnBoundary.X) { Teleport(); chaseVector = Vector3.zero; }
    }

    protected void ScanForPowerUps()
    {
        if (PowerUpDetected) { return; }

        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 2, 0), Vector3.down);
        if (hit && hit.transform.tag.Contains("PU")) 
            {
            float distance = Mathf.Abs(hit.point.y - transform.position.y);
            PowerUpDetected = true;
            FireOnce();
        }
    }
}
