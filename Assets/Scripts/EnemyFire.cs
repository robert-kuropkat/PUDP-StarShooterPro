using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    //
    // Properties
    //

    public bool hasHit = false;
    public bool HasHit
    {
        get { return hasHit;  }
        set { hasHit = value; }
    }

    //
    // Game Control             ============================================================
    //
    private void Update()
        { if ( transform.childCount < 1 ) { Destroy(this.gameObject); } }

}
