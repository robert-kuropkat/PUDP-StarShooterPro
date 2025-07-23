using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMinionRight : EnemyMinion
{

    protected override Vector3 SpawnPosition
    {
        get
        {
            switch (TravelPattern)
            {
                case MinionPattern.Pattern_1:
                    return new Vector3(2f, VerticalSpawnBoundary.Y, 0);
                case MinionPattern.Pattern_2:
                    return new Vector3(HorizontalSpawnBoundary.X, HorizontalSpawnBoundary.Y, 0);
                case MinionPattern.Pattern_3:
                    return new Vector3(2f, VerticalSpawnBoundary.Y, 0);
                case MinionPattern.Pattern_4:
                    return new Vector3(HorizontalSpawnBoundary.X, HorizontalSpawnBoundary.Y + 2, 0);
                default:
                    return new Vector3(0.01f, VerticalSpawnBoundary.Y, 0);
            }
        }
    }


    protected override void Pattern_1()
    {
        transform.Translate((Vector3.down + chaseVector) * (Time.deltaTime * mySpeed));
        if (transform.position.y > 2.93)
        {
            //newPosition.x = -5*((-1.5f * (.5f * transform.position.y + 10)) / (19 * transform.position.y - 50));
            newPosition.x = 1f;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
        if (transform.position.y <= 2.93)
        {
            newPosition.x = -(Mathf.Pow((transform.position.y + 1), 2) - 23) / 2.5f;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }
    protected override void Pattern_2()
    {
        transform.Translate((Vector3.left + chaseVector) * (Time.deltaTime * mySpeed));
    }
    protected override void Pattern_3()
    {
        transform.Translate((Vector3.down) * (Time.deltaTime * mySpeed));
        if (transform.position.y > 3.5)
        {
            transform.Translate(Vector3.down * (Time.deltaTime * mySpeed));
        }
        if (transform.position.y <= 3.5)
        {
            transform.Translate(new Vector3(2f, -1, 0) * (Time.deltaTime * mySpeed));
        }
    }
    protected override void Pattern_4()
    {
        transform.Translate(new Vector3(-2, -1, 0) * (Time.deltaTime * mySpeed));
    }
}
