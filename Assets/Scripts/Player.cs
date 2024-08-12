using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float        mySpeed      = 3.5f;
    [SerializeField] private float        fireRate     = 0.05f;
    [SerializeField] private int          playerLives  = 3;
    [SerializeField] private bool         laserCanFire = true;
    [SerializeField] private Vector3      laserOffest  = new Vector3(0, 1.006f, 0);
    [SerializeField] private GameObject   laserPrefab;
    [SerializeField] private SpawnManager spawnManager;

    void Start()
    {
        if (spawnManager == null) { Debug.LogError("The Spawn Manager is NULL."); }
        if (laserPrefab == null) { Debug.LogError("The Laser Prefab is NULL."); }

        transform.position = new Vector3(0, 0, 0);
    }

    void Update()
    {
        MovePlayer();
        CheckBoundaries();

        if (  playerHasFired()
           && laserCanFire 
           ) { FireLaser(); }
    }

    void MovePlayer()
    {
        transform.Translate(new Vector3( Input.GetAxis("Horizontal")
                                       , Input.GetAxis("Vertical")
                                       , 0
                                       ) * Time.deltaTime * mySpeed);
    }

    void CheckBoundaries()
    {
        transform.position = new Vector3( CheckLeftRight(transform.position.x)
                                        , CheckTopBottom(transform.position.y)
                                        , 0);
    }

    float CheckLeftRight(float x) 
    {
        float leftRightBoundary = 11.2f;
        return Mathf.Abs(x) < leftRightBoundary ? x : -x; 
    }

    float CheckTopBottom(float y) 
    {
        float topBoundary    =  0;
        float bottomBoundary = -4;
        return Mathf.Clamp(y, bottomBoundary, topBoundary); 
    }

    bool playerHasFired()
            { return Input.GetKeyDown(KeyCode.Space); }

    void FireLaser()
    {
        Instantiate(laserPrefab, transform.position + laserOffest, Quaternion.identity);
        laserCanFire = false;
        StartCoroutine(LaserCoolDown());
    }

    IEnumerator LaserCoolDown()
    {
        yield return new WaitForSeconds(fireRate);
        laserCanFire = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
            { if (other.tag == "Enemy") { TakeDamage(); } }

    public void TakeDamage()
    {
        playerLives--;
        if (playerLives < 1) 
        {
            spawnManager.PlayerDied();
            Destroy(this.gameObject);  
        }
    }
}
