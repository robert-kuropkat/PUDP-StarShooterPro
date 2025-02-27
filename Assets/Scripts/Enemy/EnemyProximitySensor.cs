using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximitySensor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
            { this.transform.parent.GetComponent<Enemy>().ChasePlayer(); }

        if (collision.CompareTag("Laser")) 
            { this.transform.parent.GetComponent<Enemy>().DuckAndDodge(); }
    }
}
