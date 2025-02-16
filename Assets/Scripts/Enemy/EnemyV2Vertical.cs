using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyV2Vertical : EnemyV2
{

    //
    // Properties and Data
    //

    protected override Vector3 SpawnPosition
    {
        get
        {
            Debug.Log("Spawn Boundaries");
            return new Vector3(Random.Range(-(VerticalSpawnBoundary.X)
                                            , (VerticalSpawnBoundary.X))
                              , VerticalSpawnBoundary.Y, 0);
        }
    }


    //
    // Game Loop
    //

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        MoveMe();
        if (ImDead) { return; }           // ensure an exploding enemy does not respawn at the top
        if (transform.position.y < -VerticalSpawnBoundary.Y) { Teleport(); }
    }

    //
    // Helper Methods
    //  

    protected override void MoveMe()
    { transform.Translate(Vector3.down * Time.deltaTime * MySpeed); }

}
