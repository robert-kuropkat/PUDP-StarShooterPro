using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float mySpeed = 8.0f;

    void Start()  
            { Destroy(this.gameObject, 3); }

    void Update() 
            { transform.Translate(Vector3.up * Time.deltaTime * mySpeed); }

    private void OnTriggerEnter2D(Collider2D other)
            { Destroy(this.gameObject); }
}
