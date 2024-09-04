using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : MonoBehaviour
{
    //
    // Timers
    //
    [SerializeField] private float myTimeOut = 3f;

    //
    // Game Control
    //

    private void Start()
    {
        PutInContainer();
        Destroy(this.gameObject, myTimeOut);
    }

    //
    // Helper Methods        ============================================================
    //

    private void PutInContainer()
    {
        GameObject weaponsContainer = GameObject.FindGameObjectWithTag("Weapons Container");
        if (weaponsContainer != null) { transform.parent = weaponsContainer.transform; }
    }

}
