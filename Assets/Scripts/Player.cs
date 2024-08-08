using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float      mySpeed      = 3.5f;
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private Vector3    laserOffest  = new Vector3(0,0.9f,0);
    [SerializeField] private float      fireRate     = 0.15f;
    [SerializeField] private bool       laserCanFire = true;

    void Start()
    {
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

    bool playerHasFired() { return Input.GetKeyDown(KeyCode.Space); }

    void MovePlayer()
    {
        // float horizontalInput = Input.GetAxis("Horizontal");
        // float verticalInput   = Input.GetAxis("Vertical");
        // transform.Translate(new Vector3(1 * horizontalInput
        //                                , 1 * verticalInput
        transform.Translate(new Vector3( Input.GetAxis("Horizontal")
                                       , Input.GetAxis("Vertical")
                                       , 0
                                       ) * Time.deltaTime * mySpeed);
    }

    void CheckBoundaries()
    {
        //Mathf.Abs(transform.position.x) < 11.25 ? transform.position.x : -transform.position.x
        //Mathf.Clamp(transform.position.y, -4, 0)
        transform.position = new Vector3( CheckLeftRight(transform.position.x)
                                        , CheckTopBottom(transform.position.y)
                                        , 0);
    }

    float CheckLeftRight(float x) {
        float leftRightBoundary = 11.2f;
        return Mathf.Abs(x) < leftRightBoundary ? x : -x; 
    }

    float CheckTopBottom(float y) {
        float topBoundary    =  0;
        float bottomBoundary = -4;
        return Mathf.Clamp(y, bottomBoundary, topBoundary); 
    }

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
}
