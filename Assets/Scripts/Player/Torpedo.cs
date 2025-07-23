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
    // Game Objects non-Serialized objects populated in code
    //

    // private Rigidbody2D rb;

    //
    // Properties
    //

    //private GameObject myTarget;
    //public GameObject MyTarget 
    //{
    //    get { return myTarget; }
    //    set { myTarget = value; }
    //}

    //
    // Game Control
    //

    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        //FindTarget();
        PutInContainer();
        Destroy(this.gameObject, myTimeOut);
    }

    // Update is called once per frame
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
    //{ transform.Translate(Vector3.up * (Time.deltaTime * mySpeed)); }
    {
        //transform.up = myTarget.transform.position - transform.position;
        //rb.velocity  = transform.up * mySpeed;
        //transform.Translate(transform.up * mySpeed);

        if (MyTarget != null)
        {
            Debug.Log(MyTarget.name);
            //myTarget.GetComponentInChildren<EnemyShields>().IsActive = true;
            transform.up = MyTarget.transform.position - transform.position;
            transform.Translate(Vector3.up * (Time.deltaTime * mySpeed));


            //float rot_z = Mathf.Atan2(myTarget.transform.position.y - transform.position.y
            //                         , myTarget.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            //transform.eulerAngles = new Vector3(0f, 0f, rot_z - 90);
            //transform.Translate(transform.up * (Time.deltaTime * mySpeed));
        }
        //else { Destroy(this.gameObject); }
    }

    public static void FindTarget()
    {
        MyTarget = GameObject.FindWithTag("Enemy");
        //if (MyTarget == null) { Destroy(this.gameObject); Debug.Log("Null Target"); } // make myTarget public so we can check for it.
    }
}
