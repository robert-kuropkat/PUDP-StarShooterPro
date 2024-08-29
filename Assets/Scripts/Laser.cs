using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float     mySpeed = 8.0f;

    private void Start()  
    {
        GameObject  weaponsContainer = GameObject.FindGameObjectWithTag("Weapons Container");
        if (  weaponsContainer      != null
           && this.transform.parent == null) { transform.parent = weaponsContainer.transform; }
        Destroy(this.gameObject, 3); 
    }

    private void Update() 
            { transform.Translate(Vector3.up * Time.deltaTime * mySpeed); }

    private void OnTriggerEnter2D(Collider2D other)        
    { 
        if (other.tag == "Enemy") 
        {
            Collider2D  myCollider = GetComponent<Collider2D>();
            Renderer    myRenderer = GetComponent<Renderer>();
            if (myCollider == null) { Debug.LogError("Laser Collider2D is NULL"); }
                               else { myCollider.enabled = false; }
            if (myRenderer == null) { Debug.LogError("Laser Renderer is NULL"); }
                               else { myRenderer.enabled = false; }
            Destroy(this.gameObject, 1.5f); 
        } 
    }
}
