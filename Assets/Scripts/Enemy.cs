using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float    mySpeed = 4f;
    [SerializeField] private bool     imDead  = false;
    [SerializeField] private Player   myPlayer;
                     private Animator myExplosion_anim;

    private void Start() 
    { 
        myPlayer         = GameObject.Find("Player").GetComponent<Player>();
        myExplosion_anim = GetComponent<Animator>();

        if (myPlayer         == null) { Debug.LogError("The Player is NULL."); }
        if (myExplosion_anim == null) { Debug.LogError("The Explosion Animator is NULL."); }
    }

    private void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * mySpeed);

        if (imDead) { return; }     // ensure an exploding enemy does not respawn at the top

        if (  transform.position.y < -8.0f )
            { transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 8, 0); }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (  other.tag == "Player"
           || other.tag == "Laser"
           ) { EnemyDeathScene(); }
    }

    private void EnemyDeathScene()
    {
        imDead                  = true;                                             // Flag to short circuit respawning in Update()
        Collider2D  myCollider  = GetComponent<Collider2D>();                       // Disable collider to prevent "false" collisions
        AudioSource myAudio     = GetComponent<AudioSource>();
        if (myCollider == null) { Debug.LogError("Enemy Collider2D is NULL"); }
                           else { myCollider.enabled = false; }
        if (myAudio    == null) { Debug.LogError("Enemy Audio Source is NULL"); }
                           else { myAudio.Play(); }
        myExplosion_anim.SetTrigger("OnEnemyDeath");                                // Trigger explosion animation
        if (myPlayer != null)   { myPlayer.EnemyDestroyed(); }                      // Alert player an enemy has been destroyed
        Destroy(this.gameObject, 2.4f);
    }

}
