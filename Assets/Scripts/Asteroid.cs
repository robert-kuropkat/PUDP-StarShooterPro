using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float       myRoataionSpeed = -0.2f;
    [SerializeField] private Animator    myExplosion;
    [SerializeField] private GameManager myGameManager;

    private void NullCheckOnStartup()
    {
        if (myExplosion   == null) { Debug.Log("My Explosion Animator is NULL!"); }
        if (myGameManager == null) { Debug.Log("My Game Manager is NULL!"); }
    }

    private void Start() { NullCheckOnStartup(); }

    void Update() { transform.Rotate(0, 0, myRoataionSpeed); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser") 
        {
            GameOn();
            DisableCollisionComponenets();
            DestroyMyself();
        }
    }

    private void GameOn() { myGameManager.GameLive = true; }
    private void DisableCollisionComponenets()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        Renderer myRenderer   = GetComponent<Renderer>();
        if (myCollider == null) { Debug.LogError("Asteroid Collider is NULL"); }
                           else { myCollider.enabled = false; }
        if (myRenderer == null) { Debug.LogError("Asteroid Renderer is NULL"); }
                           else { myRenderer.enabled = false; }
    }

    private void DestroyMyself()
    {
        Instantiate(myExplosion, transform.position, Quaternion.identity);
        Destroy(this.gameObject, 3f);
    }

}
