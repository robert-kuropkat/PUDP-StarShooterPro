using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : MonoBehaviour
{
    private void Start()
    {
        GameObject weaponsContainer = GameObject.FindGameObjectWithTag("Weapons Container");
        if (weaponsContainer != null) { transform.parent = weaponsContainer.transform; }
        Destroy(this.gameObject, 3);
    }

}
