using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShotPowerUp : MonoBehaviour
{
    [SerializeField] private float mySpeed = 3f;

    private void Start()
    {
        Destroy(this.gameObject, 5);
    }

    private void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * mySpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ( other.tag == "Player" ) { Destroy(this.gameObject);  }
    }
}
