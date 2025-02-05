using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontalRight : EnemyHorizontal
{
    protected override Vector3 SpawnPosition
    {
        get
        {
            return new Vector3(-(HorizontalSpawnBoundary.X)
                              , Random.Range( -(HorizontalSpawnBoundary.Y)
                                            ,  (HorizontalSpawnBoundary.Y)), 0);
        }
    }

    protected override void Update()
    {
        MoveMe();
        if (ImDead) { return; }           // ensure an exploding enemy does not respawn
        if (transform.position.x > (HorizontalSpawnBoundary.X)) { Teleport(); }
    }

    protected override void MoveMe()
    { transform.Translate(Vector3.right * Time.deltaTime * mySpeed); }

}
