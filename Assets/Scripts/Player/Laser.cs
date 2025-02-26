using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //
    // Speed and Timers
    //
    [SerializeField] private float mySpeed         = 8.0f;
    [SerializeField] private float myTimeOut       = 3f;
    [SerializeField] private float rearLaserOffset = 2.5f;

    private int laserDirection = 1;

    //
    // Game Objects non-Serialized objects populated in code
    //
    private Player myPlayer;

    //
    // Game Control             ============================================================
    //

    private void Start()  
    {
        myPlayer = GameObject.Find("Player").GetComponent<Player>();

        SetEnemyLaserDirection();
        PutInContainer();
        Destroy(this.gameObject, myTimeOut); 
    }

    private void Update() 
    {
        if   ( transform.tag == "Enemy Laser" )
             { MoveEnemyLaser();  }
        else { MovePlayerLaser(); }
    }

    private void OnTriggerEnter2D(Collider2D other)        
    {
        if (  other.tag      == "Player" 
           && transform.tag  == "Enemy Laser" )
           {  Destroy(this.gameObject, 0.1f); }                               // small delay to disable second laser
        if (  transform.tag  == "Enemy Laser" ) { return; }                   // If it is Enemy fire and hits anything other than the Player, return.
        if (  other.tag      == "Enemy"       ) { Destroy(this.gameObject); } // Destroy [Player] laser if it collides with an Enemy
    }

    //
    // Helper Methods           ============================================================
    //

    private void SetEnemyLaserDirection()
    {
        if (  transform.tag == "Enemy Laser"
           && transform.position.y < myPlayer.transform.position.y) 
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + rearLaserOffset, 0);
            laserDirection = -1;  
        }
    }

    //
    //  This is kinda dumb.  Change it Vector3.down
    //
    private void MoveEnemyLaser()
        { transform.Translate(Vector3.down * (Time.deltaTime * mySpeed * laserDirection)); }

    private void MovePlayerLaser()
        { transform.Translate(Vector3.up * (Time.deltaTime * mySpeed)); }

    private void PutInContainer()
    {
        GameObject weaponsContainer = GameObject.FindGameObjectWithTag("Weapons Container");
        if (  weaponsContainer      != null
           && this.transform.parent == null ) { transform.parent = weaponsContainer.transform; }
    }

}
