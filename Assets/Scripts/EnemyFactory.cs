using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory
{
    public static Enemy GetEnemy(string enemyType)
    {
        Enemy myEnemy = null;
        switch (enemyType)
        {
            case "Vertical":
                myEnemy = new EnemyVertical();
                break;
            default:
                myEnemy = new EnemyVertical();
                break;
        }
        return myEnemy;
    }
}
