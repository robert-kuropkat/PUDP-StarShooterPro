using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TripleShot : MonoBehaviour
{
    //
    // Static data
    //

    [SerializeField] private static int count = 0;
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
    // Game Objects non-Serialized objects populated in code
    //
    private UIManager uiManager;

    //
    // Timers
    //
    [SerializeField] private float myTimeOut = 3f;

    //
    // Game Control
    //

    private void Start()
    {
        uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        PutInContainer();
        uiManager.TripleShotCount(TripleShot.Count);
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
