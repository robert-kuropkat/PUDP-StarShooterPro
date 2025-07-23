using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralShot : MonoBehaviour
{
    //
    // Static data
    //
    [SerializeField] private static int count;
    public static int Count
    {
        get { return count; }
        set
        {
            count = value;
            count = (count < 0) ? 0 : count;
        }
    }
    public static bool Enabled { get; set; } = false;
    public static bool Armed   { get; set; } = false;

    // ============================================================

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
        transform.parent            = weaponsContainer?.transform;
    }
}
