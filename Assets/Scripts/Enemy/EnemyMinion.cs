using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMinion : Enemy
{

    //
    // Template
    //
    abstract protected void Pattern_1();
    abstract protected void Pattern_2();
    abstract protected void Pattern_3();
    abstract protected void Pattern_4();

    protected Vector3 newPosition = new Vector3(0,0,0);

    //
    // Properties
    //

    [SerializeField] MinionPattern travlelPattern = MinionPattern.Pattern_1;
    protected MinionPattern TravelPattern
    {
        get { return travlelPattern; }
        set { travlelPattern = value; }
    }

    [SerializeField] protected float mySpeed = 5.0f;
    public override float MySpeed { get { return mySpeed; } set { mySpeed = value; } }

    //
    // Game Object Control Methods
    //
    protected override void Teleport()
    { 
        TravelPattern = (TravelPattern == MinionPattern.Reset) ? MinionPattern.Pattern_1 : TravelPattern + 1; 
        transform.position = SpawnPosition; 
    }

    protected override void MoveMe()
    {
        switch (TravelPattern)
        {
            case MinionPattern.Pattern_1:
                Pattern_1();
                break;
            case MinionPattern.Pattern_2:
                Pattern_2();
                break;
            case MinionPattern.Pattern_3:
                Pattern_3();
                break;
            case MinionPattern.Pattern_4:
                Pattern_4();
                break;
            default:
                Pattern_1();
                break;
        }
    }


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
        CheckBoundaries();
    }

}
