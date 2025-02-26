using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontalZigZagLeft : EnemyHorizontalZigZag
{
    protected override Vector3 SpawnPosition
    {
        get
        {
            return new Vector3((HorizontalSpawnBoundary.X)
                              , Random.Range( -(HorizontalSpawnBoundary.Y)
                                            ,  (HorizontalSpawnBoundary.Y)), 0);
        }
    }

    //
    // Game Loop
    //
    protected override void Update()
    {
        MoveMe();
        if (ImDead) { return; }           // ensure an exploding enemy does not respawn
        CheckBoundaries();
    }

    protected override void MoveMe()
    {
        //
        // might want to randomize the angle  right now it is just a 45degree angle (x=1, y=1)
        //
        transform.Translate(new Vector3( -1
                                       , -1  * (changeDirection ? 1 : -1)
                                       ,  0) * (Time.deltaTime * mySpeed), Space.Self);
    }

}
