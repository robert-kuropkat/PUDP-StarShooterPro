using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontal : Enemy
{
    //
    // Deferred
    //
    protected override Vector3 SpawnPosition { get; }
    protected override void MoveMe() { }
    protected override void Update() { }

    //
    // Properties
    //

    [SerializeField] protected float mySpeed = 4.0f;
    public override float MySpeed { get { return mySpeed; } set { mySpeed = value; } }


    //
    // Game Loop
    //
    protected override void Start()
    {
        base.Start();
        transform.position = SpawnPosition;
        transform.rotation = Quaternion.identity;
    }

    //
    // Game Object Control Methods
    //
    protected override void Teleport()
    { transform.position = SpawnPosition; }

}
