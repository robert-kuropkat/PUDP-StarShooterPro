using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVertical : Enemy
{
    private void Update()
    {
        MoveMe();
        if (imDead) { return; }           // ensure an exploding enemy does not respawn at the top
        if (transform.position.y < -(screenBoundary_TB + spawnPoint_T)) { RespawnAtTop(); }
    }

}