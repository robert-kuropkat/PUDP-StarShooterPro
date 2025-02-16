using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyV2 : MonoBehaviour, ISpawnable
{

    //
    // Template
    //

    abstract protected Vector3 SpawnPosition { get; }
    abstract protected void MoveMe();
    abstract protected void Update();

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
    // Properties and Data
    //

    protected bool ImDead { get; set; } = false;

    //[SerializeField] protected float mySpeed = 4.0f;
    //public float MySpeed { get { return mySpeed; } set { mySpeed = value; } }
    public float MySpeed { get; set; } = 4.0f;

    [System.Serializable]
    protected struct Boundary
    {
        [SerializeField] private float _x, _y;
        public Boundary(float x, float y) { _x = x; _y = y; }
        public float X { get { return _x; } }
        public float Y { get { return _y; } }
    }
    [SerializeField] protected Boundary ScreenBoundary          = new Boundary(9.5f, 6.0f);
    [SerializeField] protected Boundary HorizontalSpawnBoundary = new Boundary(11.5f, 5.0f);
    [SerializeField] protected Boundary VerticalSpawnBoundary   = new Boundary(8.5f, 8.0f);

    //
    // Game Objects non-Serialized objects are populated in code
    //
    [SerializeField] private GameObject laserPrefab;
                     private Player     myPlayer;
                     private Animator   myExplosion_anim;

    //
    // Game Loop
    //
    protected virtual void Start()
    {
        myPlayer         = GameObject.Find("Player").GetComponent<Player>();
        myExplosion_anim = GetComponent<Animator>();
        //AcivateShield();

        transform.position = SpawnPosition;
        transform.rotation = Quaternion.identity;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (  other.tag == "Player"
           || other.tag == "Laser"
           || other.tag == "Shield"
           ) { EnemyDeathScene(); }
    }


    //
    //  Helper Methods
    //


    /// <summary>
    /// 
    /// </summary>
    /// 
    protected void Teleport()
    { transform.position = SpawnPosition; }

    private void EnemyDeathScene()
    {
        //if (shieldAnim.IsActive) { return; }
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


}
