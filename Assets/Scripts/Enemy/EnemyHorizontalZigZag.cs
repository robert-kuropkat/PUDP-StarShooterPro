using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontalZigZag : EnemyHorizontal
{
    //
    // Deferred
    //
    protected override Vector3 SpawnPosition { get; }
    protected override void MoveMe() { }
    protected override void Update() { }

    //
    // Flags
    //
    [SerializeField] protected   bool  changeDirection  = false;

    //
    //  Populated in Inspector
    //
    [SerializeField] private EnemyDepthCharge myDepthCharge;

    //
    // Properties
    //
/*
    [SerializeField] protected float mySpeed = 4.0f;
    public    override float   MySpeed { get { return mySpeed; } set { mySpeed = value; } }
*/
    //
    // Game Loop
    //
    protected override void Start()
    {
        base.Start();
//        transform.position = SpawnPosition;
//        transform.rotation = Quaternion.identity;
        StartCoroutine(ChangeDirection());
    }

    //
    // Game Object Control Methods
    //
/*
    protected void Teleport()
    { transform.position = SpawnPosition; }
*/
    //
    //  Watchdogs
    //

    IEnumerator ChangeDirection()
    {
        while (!ImDead)
        {
            changeDirection = changeDirection ? false : true;
            DropDepthCharge();
            yield return new WaitForSeconds(1);
        }
    }

    private void DropDepthCharge()
    {
        Instantiate( myDepthCharge
                   , transform.position
                   , Quaternion.identity);
    }

}
