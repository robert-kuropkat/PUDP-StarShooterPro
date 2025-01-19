using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHorizontal : Enemy
{
    private void Update()
    {
        MoveMe();
        if (imDead) { return; }           // ensure an exploding enemy does not respawn at the top
        switch (SpawnSide)
        {
            case "LEFT":
                if (transform.position.x > (screenBoundary_LR + spawnPoint_LR)) { RespawnAtLeft(); }
                break;
            case "RIGHT":
                if (transform.position.x < -(screenBoundary_LR + spawnPoint_LR)) { RespawnAtRight(); }
                break;
            default:
                if (transform.position.x < -(screenBoundary_LR + spawnPoint_LR)) { RespawnAtRight(); }
                break;
        }
    }

    private void RespawnAtLeft()
    { transform.position = new Vector3(-(screenBoundary_LR + spawnPoint_LR), Random.Range(-(screenBoundary_TB - spawnPoint_T), screenBoundary_TB), 0); }

    private void RespawnAtRight()
    { transform.position = new Vector3((screenBoundary_LR + spawnPoint_LR), Random.Range(-(screenBoundary_TB - spawnPoint_T), screenBoundary_TB), 0); }

}
