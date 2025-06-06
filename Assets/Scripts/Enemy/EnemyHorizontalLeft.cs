﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontalLeft : EnemyHorizontal
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
        ScanForPowerUps();
        CheckBoundaries();
    }

    protected override void MoveMe()
    { transform.Translate((Vector3.left + chaseVector) * (Time.deltaTime * mySpeed)); }

}
