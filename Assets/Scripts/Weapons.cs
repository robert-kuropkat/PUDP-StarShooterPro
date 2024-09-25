using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    //
    // Speed, Boundaries and Timers
    //
    [SerializeField] private float   fireRate        = .15f;
    [SerializeField] private float   tripleShotTimer = 5f;
    [SerializeField] private int     ammoCollectible = 15;
    [SerializeField] private int     ammoStartCount  = 26;
    [SerializeField] private Vector3 laserOffest     = new Vector3(0, 1.006f, 0);
    //
    // Flags
    //
    [SerializeField] private bool laserCanFire       = true;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private GameObject laserPrefab;
    [SerializeField] private GameObject tripleShotPrefab;
    [SerializeField] private UIManager  uiManager;
    //
    // Properties
    //
    [SerializeField] private int ammoCount;
    private int AmmoCount
    {
        get { return ammoCount; }
        set { 
              ammoCount = value;
              ammoCount = (ammoCount < 0) ? 0 : ammoCount;
              uiManager.AmmoCount(AmmoCount);
            }
    }

    [SerializeField] private bool tripleShotEnabled = false;
    public bool TripleShotEnabled
    {
        get { return tripleShotEnabled;  }
        set { 
                tripleShotEnabled = value; 
                StartCoroutine(PowerUpTripleShot());
            }
    }

    private void NullCheckOnStartup()
    {
        if (laserPrefab      == null) { Debug.LogError("The Laser Prefab is NULL."); }
        if (tripleShotPrefab == null) { Debug.LogError("The TripleShot PowerUp Prefab is NULL"); }
        if (uiManager        == null) { Debug.LogError("The UI Manager is NULL"); }
    }

    void Start()
    {
        NullCheckOnStartup();
        AmmoCount = ammoStartCount;
    }

    //
    // Public Methods
    //
    public void FireLaser()
    {
        if (!laserCanFire) { return; }
        if (AmmoCount < 1) { return; }
        if (TripleShotEnabled) 
             { AmmoCount = AmmoCount - 3; }
        else { AmmoCount = AmmoCount - 1; }

        Instantiate((TripleShotEnabled) ? tripleShotPrefab : laserPrefab
                   , transform.position + laserOffest
                   , Quaternion.identity);
        laserCanFire = false;
        StartCoroutine(LaserCoolDown());
    }

    public void CollectAmmo() { AmmoCount = AmmoCount + ammoCollectible; }

    //
    // Watchdogs            ============================================================
    //

    private IEnumerator LaserCoolDown()
    {
        yield return new WaitForSeconds(fireRate);
        laserCanFire = true;
    }

    private IEnumerator PowerUpTripleShot()
    {
        yield return new WaitForSeconds(tripleShotTimer);
        TripleShotEnabled = false;
    }
}
