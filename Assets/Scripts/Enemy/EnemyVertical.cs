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

    //protected override Vector3 MoveDirection { get; set; }

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
        ScanForPowerUps();
        CheckBoundaries();
    }

    protected override void MoveMe()
    { transform.Translate((Vector3.down + chaseVector) * (Time.deltaTime * mySpeed)); }

    protected override void Teleport()
    { transform.position = SpawnPosition; }

}