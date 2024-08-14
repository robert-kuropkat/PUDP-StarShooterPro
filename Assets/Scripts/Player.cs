using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float        mySpeed         = 3.5f;
    [SerializeField] private float        fireRate        = 0.05f;
    [SerializeField] private float        tripleShotTimer = 5f;
    [SerializeField] private int          playerLives     = 3;
    [SerializeField] private bool         laserCanFire    = true;
    [SerializeField] private bool         tripleShot      = false;
    [SerializeField] private Vector3      laserOffest     = new Vector3(0, 1.006f, 0);
    [SerializeField] private GameObject   laserPrefab;
    [SerializeField] private GameObject   tripleShotPrefab;
    [SerializeField] private SpawnManager spawnManager;

    private void Start()
    {
        //ToDo:  Add actual error handling rather than just a debug message.
        if (spawnManager     == null) { Debug.LogError("The Spawn Manager is NULL."); }
        if (laserPrefab      == null) { Debug.LogError("The Laser Prefab is NULL."); }
        if (tripleShotPrefab == null) { Debug.LogError("The TripleShot PowerUp Prefab is NULL");  }

        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        MovePlayer();
        CheckBoundaries();

        if (  playerHasFired()  // check order.  For fast fail.  Again, a useful optimization!!!
           && laserCanFire 
           ) { FireLaser(); }
    }

    private void MovePlayer()
    {
        transform.Translate(new Vector3( Input.GetAxis("Horizontal")
                                       , Input.GetAxis("Vertical")
                                       , 0
                                       ) * Time.deltaTime * mySpeed);
    }

    private void CheckBoundaries()
    {
        transform.position = new Vector3( CheckLeftRight(transform.position.x)
                                        , CheckTopBottom(transform.position.y)
                                        , 0);
    }

    private float CheckLeftRight(float x) 
    {
        float leftRightBoundary = 11.2f;  // Move up so Designer can change.  Note, you messed up, these actually cost more processing because they are called from Update.  
                                          // So contsntatly created and destroyed!!!  doh!
        return Mathf.Abs(x) < leftRightBoundary ? x : -x; 
    }

    private float CheckTopBottom(float y) 
    {
        float topBoundary    =  0;  // Move up so Designer can change
        float bottomBoundary = -4;  // Move up so Designer can change
        return Mathf.Clamp(y, bottomBoundary, topBoundary); 
    }

    private bool playerHasFired()  //Change init cap.  Also look up return function vs return method. Remember this for when we cover new Input system
            { return Input.GetKeyDown(KeyCode.Space); }

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

    private void OnTriggerEnter2D(Collider2D other)        
    { 
        if (  other.tag == "Enemy") { TakeDamage(); }
        if (  other.tag == "TripleShotPU"
           && !tripleShot)          { StartCoroutine(PowerUpTripleShot()); }
    }

    private void TakeDamage()
    {
        playerLives--;
        if (playerLives < 1) 
        {
            spawnManager.PlayerDied();
            Destroy(this.gameObject);  
        }
    }
}
