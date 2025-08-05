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
    // Game Loop
    //
    protected override void Start()
    {
        base.Start();
        StartCoroutine(ChangeDirection());
    }

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
