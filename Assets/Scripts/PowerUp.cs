using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float     mySpeed = 3f;
    [SerializeField] private AudioClip myAudioClip;

    private void Start()                            
        { Destroy(this.gameObject, 5); }

    private void Update()                           
        { transform.Translate(Vector3.down * Time.deltaTime * mySpeed); }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.tag == "Player" ) 
        {
            AudioSource.PlayClipAtPoint(myAudioClip, Camera.main.transform.position, 1f);
            //AudioSource myAudio = GetComponent<AudioSource>();
            //if (myAudio == null) { Debug.LogError("PowerUp Audio Source is NULL"); }
            //                else { myAudio.Play(); Debug.Log("PowerUP Audio!"); }
            //DisableCollisionComponenets();
            Destroy(this.gameObject);  
        } 
    }

    private void DisableCollisionComponenets()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        Renderer myRenderer   = GetComponent<Renderer>();
        if (myCollider == null) { Debug.LogError("Asteroid Collider is NULL"); }
                           else { myCollider.enabled = false; }
        if (myRenderer == null) { Debug.LogError("Asteroid Renderer is NULL"); }
                           else { myRenderer.enabled = false; }
    }


}
