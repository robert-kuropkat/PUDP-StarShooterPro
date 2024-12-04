using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //
    // Speed, Timers and Boundaries
    //
    [SerializeField] private float      mySpeed         = 4.0f;
    [SerializeField] private float      laserLowerTimer = 2.0f;
    [SerializeField] private float      laserUpperTimer = 5.0f;
    [SerializeField] private float      explosionTimer  = 2.4f;
    //
    // need to separate screen boundaries from spawn boundaries
    // Note:  Upper point is based on the nose of the player, not center.    (-4,6)
    //        right/left is the right/left of the player, also not center.   (9.5,-9.5)
    //        Maybe need three sets of boundaries.  Screen, object and spawn...
    //
    [SerializeField] private float      screenBoundary_TB = 6.0f;
    [SerializeField] private float      screenBoundary_LR = 9.5f;
    [SerializeField] private float      spawnPoint_T      = 2.0f;
    [SerializeField] private float      spawnPoint_LR     = 2.0f;
    //[SerializeField] private float      upperLowerBound = 8.0f;
    //[SerializeField] private float      leftRightBound  = 9.0f;
    //
    // Flags
    //
    [SerializeField] private bool       imDead          = false;
    //
    // Game Objects populated in code
    //
    [SerializeField] private Player     myPlayer;
    [SerializeField] private GameObject laserPrefab;
                     private Animator   myExplosion_anim;

    //
    // Properties
    //
    [SerializeField] private string     spawnSide;
    public string SpawnSide
    {
        get { return spawnSide;  }
        set { spawnSide = value.ToUpper(); }
    }

    //
    // Game Control              ============================================================
    //
    private void NullCheckOnStartup()
    {
        if (myPlayer         == null) { Debug.LogError("The Player is NULL."); }
        if (laserPrefab      == null) { Debug.LogError("The Enemy Laser Prefab is NULL."); }
        if (myExplosion_anim == null) { Debug.LogError("The Explosion Animator is NULL."); }
    }

    private void Start() 
    { 
        myPlayer         = GameObject.Find("Player").GetComponent<Player>();
        myExplosion_anim = GetComponent<Animator>();

        NullCheckOnStartup();
        StartCoroutine(FireLaser());
    }

    private void Update()
    {
        MoveMe();
        if (imDead) { return; }           // ensure an exploding enemy does not respawn at the top
        switch (SpawnSide)
        {
            case "LEFT":
                if (transform.position.x >  (screenBoundary_LR + spawnPoint_LR)) { RespawnAtLeft(); }
                break;
            case "RIGHT":
                if (transform.position.x < -(screenBoundary_LR + spawnPoint_LR)) { RespawnAtRight(); }
                break;
            default:
                if (transform.position.y < -(screenBoundary_TB + spawnPoint_T) ) { RespawnAtTop(); }
                break;
        }
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
        while (!imDead)
        {
            //
            // If enemy is below the player, in particular off screen, then don't bother firing.
            // it's harmless, but irritating, especially when the laser fires off screen and can't
            // be seen but can be heard.
            //
            if (  myPlayer != null 
               && transform.position.y > myPlayer.transform.position.y)
            {
                Instantiate(laserPrefab
                           , transform.position
                           , Quaternion.identity);
            }
            // Debug.Break();
            yield return new WaitForSeconds(Random.Range(laserLowerTimer, laserUpperTimer));
        }
    }

    //
    // Helper Methods            ============================================================
    //

    private void MoveMe() 
    {
        switch (SpawnSide)
        {
            case "LEFT":
                transform.Translate(Vector3.right * Time.deltaTime * mySpeed);
                break;
            case "RIGHT":
                transform.Translate(Vector3.left * Time.deltaTime * mySpeed);
                break;
            case "TOP":
                transform.Translate(Vector3.down * Time.deltaTime * mySpeed);
                break;
            default:
                transform.Translate(Vector3.down * Time.deltaTime * mySpeed);
                break;
        }
    }

    private void RespawnAtTop()
        { transform.position = new Vector3(Random.Range(-(screenBoundary_LR), screenBoundary_LR), (screenBoundary_TB + spawnPoint_T), 0); }

    private void RespawnAtLeft()
    { transform.position = new Vector3(-(screenBoundary_LR + spawnPoint_LR), Random.Range(-(screenBoundary_TB - spawnPoint_T), screenBoundary_TB), 0); }

    private void RespawnAtRight()
    { transform.position = new Vector3( (screenBoundary_LR + spawnPoint_LR), Random.Range(-(screenBoundary_TB - spawnPoint_T), screenBoundary_TB), 0); }

    private void EnemyDeathScene()
    {
        imDead = true; // Flag to short circuit respawning in Update()
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

    private void TriggerExplosion()
        { myExplosion_anim.SetTrigger("OnEnemyDeath"); }

    private void NotifyPlayer()
        { myPlayer.EnemyDestroyed(); }

}
