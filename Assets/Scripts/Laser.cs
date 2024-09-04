using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    //
    // Speed and Timers
    //
    [SerializeField] private float mySpeed   = 8.0f;
    [SerializeField] private float myTimeOut = 3f;

    //
    // Game Control             ============================================================
    //

    private void Start()  
    {
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

    private void MoveEnemyLaser()
        { transform.Translate(Vector3.up * Time.deltaTime * -mySpeed); }

    private void MovePlayerLaser()
        { transform.Translate(Vector3.up * Time.deltaTime *  mySpeed); }

    private void PutInContainer()
    {
        GameObject weaponsContainer = GameObject.FindGameObjectWithTag("Weapons Container");
        if (  weaponsContainer      != null
           && this.transform.parent == null ) { transform.parent = weaponsContainer.transform; }
    }

}
