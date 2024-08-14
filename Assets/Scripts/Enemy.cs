using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float mySpeed = 4f;

    private void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * mySpeed);
        if (transform.position.y < -8.0f)
            { transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 8, 0); }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (  other.tag == "Player"
           || other.tag == "Laser"
           )
        {
            Destroy(this.gameObject);
        }
    }
    
}
