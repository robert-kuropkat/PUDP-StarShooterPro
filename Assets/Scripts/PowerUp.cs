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
    // Properties
    //

    [System.Serializable]
    protected struct Boundary
    {
        [SerializeField] private float _x, _y;
        public Boundary(float x, float y) { _x = x; _y = y; }
        public float X { get { return _x; } }
        public float Y { get { return _y; } }
    }
    [SerializeField] protected Boundary ScreenBoundary = new Boundary(9.5f, 6.0f);
    //[SerializeField] protected Boundary HorizontalSpawnBoundary = new Boundary(11.5f, 5.0f);
    [SerializeField] protected Boundary VerticalSpawnBoundary = new Boundary(8.5f, 8.0f);

    protected Vector3 SpawnPosition
    {
        get
        {
            return new Vector3(Random.Range( -(VerticalSpawnBoundary.X)
                                            , (VerticalSpawnBoundary.X))
                              , VerticalSpawnBoundary.Y, 0);
        }
    }



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
        transform.position = SpawnPosition;
        transform.rotation = Quaternion.identity;
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
