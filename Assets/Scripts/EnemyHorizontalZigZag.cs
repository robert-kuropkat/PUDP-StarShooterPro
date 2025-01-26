using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontalZigZag : Enemy
{
    //
    // Flags
    //
    [SerializeField] private   bool  changeDirection  = false;

    //
    // Properties
    //
    [SerializeField] protected float mySpeed = 4.0f;
    protected override float   MySpeed { get { return mySpeed; } }
    protected override Vector3 SpawnPosition
    {
        get 
        {
            return new Vector3( -(HorizontalSpawnBoundary.X)
                              ,   Random.Range( -(HorizontalSpawnBoundary.Y)
                                              ,  (HorizontalSpawnBoundary.Y)), 0);
        }
    }

    //
    // Game Loop
    //
    protected override void Start()
    {
        base.Start();
        transform.position = SpawnPosition;
        transform.rotation = Quaternion.identity;
        StartCoroutine(ChangeDirection());
    }

    protected override void Update()
    {
        MoveMe();
        if (ImDead) { return; }  // ensure an exploding enemy does not respawn
        if (transform.position.x > (HorizontalSpawnBoundary.X)) { Teleport(); }
    }

    //
    // Game Object Control Methods
    //

    protected override void MoveMe()
    {
        //
        // might want to randomize the angle  right now it is just a 45degree angle (x=1, y=1)
        //
        transform.Translate(new Vector3(  1
                                       , -1  * (changeDirection ? 1 : -1)
                                       ,  0 ) * Time.deltaTime * mySpeed, Space.Self);
    }

    private void Teleport()
    { transform.position = SpawnPosition; }

    //
    //  Watchdogs
    //

    IEnumerator ChangeDirection()
    {
        while (true)
        {
            changeDirection = changeDirection ? false : true;
            yield return new WaitForSeconds(1);
        }
    }


}
