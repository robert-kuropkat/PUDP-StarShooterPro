using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //
    // Timers
    //
    [SerializeField] private float explosionTimer = 2.4f;


    //
    // Game Control          ============================================================
    //

    private void Start() { Destroy(this, explosionTimer); }

}
