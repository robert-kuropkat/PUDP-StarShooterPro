using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float mySpeed = 4f;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * mySpeed);
        if (transform.position.y < -8.0f) 
            { transform.position = new Vector3(Random.Range(-10.0f, 10.0f), 8, 0); }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("hit: " + other.transform.name);

        if (other.tag == "Player")
        {
            // Damage player
            Destroy(this.gameObject);
        }
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
