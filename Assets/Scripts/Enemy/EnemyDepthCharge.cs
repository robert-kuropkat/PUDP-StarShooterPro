using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDepthCharge : MonoBehaviour
{
    //
    // Speed and Timers
    //
    [SerializeField] private float mySpeed        = 0.75f;
    [SerializeField] private float myTimeOut      = 3f;
    [SerializeField] private float explosionTimer = 1f;
    [SerializeField] private float bounceForce    = 100f;

    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private Animator myExplosion;

    //
    // Component References
    //
    private Collider2D        myCollider;
    private CapsuleCollider2D myCapsuleCollider;
    private Rigidbody2D       myRigidBody;
    private MeshRenderer      myMeshRenderer;

    //
    // Properties
    //

    //
    // Game Control             ============================================================
    //

    private void NullCheckOnStartup()
    {
        //
        //  ToDo: Make sure we are checking everything we should here...
        //
        if (myCollider        == null) { Debug.LogError("The Collider is NULL."); }
        if (myCapsuleCollider == null) { Debug.LogError("The Capsule Collider is NULL."); }
        if (myRigidBody       == null) { Debug.LogError("The Rigidbody is NULL."); }
        if (myMeshRenderer    == null) { Debug.LogError("The Mesh Reender is NULL."); }
        if (myExplosion       == null) { Debug.LogError("The Explosion Animator is NULL."); }
    }

    private void Start()
    {
        myCollider        = GetComponent<Collider2D>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        myRigidBody       = GetComponent<Rigidbody2D>();
        myMeshRenderer    = GetComponent<MeshRenderer>();
        NullCheckOnStartup();
        //
        // Do a Nullcheck for the above
        //
        PutInContainer();
        StartCoroutine(CountDown());
        //Destroy(this.gameObject, myTimeOut);
    }

    //
    // Game Control             ============================================================
    //
    private void Update()
    { 
        MoveMe(); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player") { return; }
        myRigidBody.AddForce(transform.up * bounceForce);
        transform.Rotate(0, 0, Random.Range(0,360));
    }
    //
    // Watchdogs
    //

    IEnumerator CountDown()
    {
        yield return new WaitForSeconds(myTimeOut);
        DestroyMyself();
    }

    //
    // Helper Methods           ============================================================
    //

    private void MoveMe()
    { transform.Translate(Vector3.down * Time.deltaTime * mySpeed); }

    private void PutInContainer()
    {
        GameObject weaponsContainer = GameObject.FindGameObjectWithTag("Weapons Container");
        if (weaponsContainer != null
           && this.transform.parent == null) { transform.parent = weaponsContainer.transform; }
    }

    private void DestroyMyself()
    {
        SetBlastRadius();
        StopObjectMotion();
        TriggerExplosion();
        HideObject();
        Destroy(this.gameObject, explosionTimer);
    }

    private void SetBlastRadius()
    {
        myCollider.enabled = false;
        myCapsuleCollider.size = new Vector2(10, 2);
        myCollider.enabled = true;
        gameObject.tag = "Enemy";
    }

    private void StopObjectMotion()
    {
        myRigidBody.bodyType = RigidbodyType2D.Kinematic;
        myRigidBody.velocity = Vector2.zero;
        myRigidBody.angularVelocity = 0;
    }

    private void HideObject()
    {
        myMeshRenderer.enabled = false;
    }

    private void TriggerExplosion()
    { Instantiate(myExplosion, transform.position, Quaternion.identity); }

    private void DisableCollisionComponenets()
    {
        Collider2D myCollider = GetComponent<Collider2D>();
        if (myCollider == null) { Debug.LogError("Enemy Collider2D is NULL"); }
        else { myCollider.enabled = false; }
    }

}
