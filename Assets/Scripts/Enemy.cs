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

    //
    // Timers
    //
    [SerializeField] private float laserLowerTimer = 2.0f;
    [SerializeField] private float laserUpperTimer = 5.0f;
    [SerializeField] private float explosionTimer  = 2.4f;

    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private EnemyShields shieldAnim;


    //
    // Game Objects non-Serialized objects are populated in code
    //
    [SerializeField] private GameObject laserPrefab;
                     private Player     myPlayer;
                     private Animator   myExplosion_anim;

    //
    // Properties
    //
    protected bool ImDead { get; set; } = false;

    //
    // Note:  Upper point is based on the nose of the player, not center.    (-4,6)
    //        right/left is the right/left of the player, also not center.   (9.5,-9.5)
    //        Maybe need three sets of boundaries.  Screen, object and spawn...
    //
    //  May need to give all four values, especially for the spawn boundaries
    //

    [System.Serializable]
    protected struct Boundary
    {
        [SerializeField] private float _x, _y;
        public Boundary(float x, float y) { _x = x; _y = y; }
        public float X { get { return _x; } }
        public float Y { get { return _y; } }
    }
    [SerializeField] protected Boundary ScreenBoundary          = new Boundary( 9.5f, 6.0f);
    [SerializeField] protected Boundary HorizontalSpawnBoundary = new Boundary(11.5f, 5.0f);
    [SerializeField] protected Boundary VerticalSpawnBoundary   = new Boundary( 8.5f, 8.0f);

    //
    // Game Loop              ============================================================
    //
    private void NullCheckOnStartup()
    {
        //
        //  ToDo: Make sure we are checking everything we should here...
        //
        if (myPlayer         == null) { Debug.LogError("The Player is NULL."); }
        if (laserPrefab      == null) { Debug.LogError("The Enemy Laser Prefab is NULL."); }
        if (myExplosion_anim == null) { Debug.LogError("The Explosion Animator is NULL."); }
        if (shieldAnim       == null) { Debug.LogError("The Shield Animation is NULL"); }

    }

    protected virtual void Start() 
    { 
        myPlayer           = GameObject.Find("Player").GetComponent<Player>();
        myExplosion_anim   = GetComponent<Animator>();
        AcivateShield();

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
               && transform.position.y > myPlayer.transform.position.y)
            {
                Instantiate( laserPrefab
                           , transform.position
                           , Quaternion.identity);
            }
            yield return new WaitForSeconds(Random.Range(laserLowerTimer, laserUpperTimer));
        }
    }

    //
    // Helper Methods            ============================================================
    //

    private void EnemyDeathScene()
    {
        if (shieldAnim.IsActive) { return; }
        ImDead = true; // Flag to short circuit respawning in Update()
        DisableCollisionComponenets();
        TriggerExplosion();
        NotifyPlayer();
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

    private void AcivateShield()    { shieldAnim.IsActive = true; }

    private void DeacivateShield()  { shieldAnim.IsActive = false; }
}
