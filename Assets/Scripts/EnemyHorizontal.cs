using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontal : Enemy
{
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
    }

    protected override void Update()
    {
        MoveMe();
        if (ImDead) { return; }           // ensure an exploding enemy does not respawn
        if (transform.position.x > (HorizontalSpawnBoundary.X)) { Teleport(); }
    }

    protected override void MoveMe()
    { transform.Translate(Vector3.right * Time.deltaTime * mySpeed); }

    private void Teleport()
    { transform.position = SpawnPosition; }

}
