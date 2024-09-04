using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    //
    // Speed and Timers
    //
    [SerializeField] private float     mySpeed   = 3f;
    [SerializeField] private float     myTimeOut = 5f;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private AudioClip myAudioClip;

    //
    // Game Control             ============================================================
    //

    private void NullCheckOnStartup()
    {
        if (myAudioClip == null) { Debug.LogError("Audio Clip is NULL"); }
    }

    private void Start()                            
    {
        NullCheckOnStartup();
        Destroy(this.gameObject, myTimeOut); 
    }

    private void Update() 
        { MoveMe(); }

    //
    // Watchdogs                ============================================================
    //

    private void OnTriggerEnter2D(Collider2D other)
        { if ( other.tag == "Player" )  { CollectMe(); } }

    //
    // Helper Methods           ============================================================
    //

    private void MoveMe()
        { transform.Translate(Vector3.down * Time.deltaTime * mySpeed); }

    private void CollectMe()
    {
        AudioSource.PlayClipAtPoint(myAudioClip, Camera.main.transform.position, 1f);
        Destroy(this.gameObject);
    }

}
