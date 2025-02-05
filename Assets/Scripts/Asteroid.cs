using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    //
    // Speed and Timers
    //
    [SerializeField] private float       myRoataionSpeed = -0.2f;
    [SerializeField] private float       explosionTimer  = 3f;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private Animator    myExplosion;
    [SerializeField] private GameManager myGameManager;


    //
    // Game Control         ============================================================
    //

    private void NullCheckOnStartup()
    {
        if (myExplosion   == null) { Debug.Log("My Explosion Animator is NULL!"); }
        if (myGameManager == null) { Debug.Log("My Game Manager is NULL!"); }
    }

    private void Start()  { NullCheckOnStartup(); }

    private void Update() { MoveMe(); }

    //
    // Game Start methods    ============================================================
    //

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser") 
        {
            GameOn();
            DestroyMyself();
        }
    }

    //
    // Helper Methods        ============================================================
    //

    private void MoveMe() { transform.Rotate(0, 0, myRoataionSpeed); }

    private void GameOn() 
    { 
        myGameManager.GameLive    = true;
        //myGameManager.CurrentWave = 1;
    }

    private void DestroyMyself()
    {
        DisableCollisionComponenets();
        TriggerExplosion();
        Destroy(this.gameObject, explosionTimer);
    }

    private void DisableCollisionComponenets()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        Renderer   myRenderer = GetComponent<Renderer>();
        if (myCollider == null) { Debug.LogError("Asteroid Collider is NULL"); }
                           else { myCollider.enabled = false; }
        if (myRenderer == null) { Debug.LogError("Asteroid Renderer is NULL"); }
                           else { myRenderer.enabled = false; }
    }

    private void TriggerExplosion() 
        { Instantiate(myExplosion, transform.position, Quaternion.identity); }

}
