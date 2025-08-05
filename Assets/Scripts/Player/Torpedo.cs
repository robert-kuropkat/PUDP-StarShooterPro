using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    //
    // Static data
    //

    [SerializeField] private static int count = 0;
    public static int Count
    {
        get { return count; }
        set
        {
            count = value;
            count = (count < 0) ? 0 : count;
        }
    }

    public static bool Enabled { get; set; } = false;
    public static bool Armed { get; set; } = false;
    public static GameObject MyTarget { get; set; }


    // ============================================================

    //
    // Timers
    //
    [SerializeField] private float mySpeed = 8.0f;
    [SerializeField] private float myTimeOut = .5f;


    //
    // Game Control
    //

    void Start()
    {
        PutInContainer();
        Destroy(this.gameObject, myTimeOut);
    }

    void Update()
    {
        MoveMe();
    }

    private void OnTriggerEnter2D(Collider2D other)        //  This probably needs to go.  It looks like I copied it from the Laser.
    {
        if (  other.tag      == "Player" 
           && transform.tag  == "Enemy Laser" )
           {  Destroy(this.gameObject, 0.1f); }                               // small delay to disable second laser
        if (  transform.tag  == "Enemy Laser" ) { return; }                   // If it is Enemy fire and hits anything other than the Player, return.
        if (  other.tag      == "Enemy"       ) { Destroy(this.gameObject); } // Destroy [Player] laser if it collides with an Enemy
    }

    //
    // Helper Methods        ============================================================
    //

    private void PutInContainer()
    {
        GameObject weaponsContainer = GameObject.FindGameObjectWithTag("Weapons Container");
        transform.parent            = weaponsContainer?.transform;
    }

    private void MoveMe()
    {

        if (MyTarget != null)
        {
            Debug.Log(MyTarget.name);
            transform.up = MyTarget.transform.position - transform.position;
            transform.Translate(Vector3.up * (Time.deltaTime * mySpeed));
        }
    }

    public static void FindTarget()
    {
        MyTarget = GameObject.FindWithTag("Enemy");
    }
}
