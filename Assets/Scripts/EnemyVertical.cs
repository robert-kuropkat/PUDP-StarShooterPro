using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVertical : Enemy
{
    //
    // Properties
    //
    [SerializeField] protected float mySpeed = 4.0f;
    public override float MySpeed { get { return mySpeed; } set { mySpeed = value; } }
    protected override Vector3 SpawnPosition
    {
        get {
            return new Vector3( Random.Range( -(VerticalSpawnBoundary.X)
                                            ,  (VerticalSpawnBoundary.X))
                              , VerticalSpawnBoundary.Y, 0);
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
        if (ImDead) { return; }           // ensure an exploding enemy does not respawn at the top
        if (transform.position.y < -VerticalSpawnBoundary.Y) { Teleport(); }
    }

    protected override void MoveMe()
    { transform.Translate(Vector3.down * Time.deltaTime * mySpeed); }

    protected void Teleport()
    { transform.position = SpawnPosition; }

}