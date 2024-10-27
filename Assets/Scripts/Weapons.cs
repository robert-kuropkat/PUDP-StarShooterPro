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
    [SerializeField] private GameObject laser360Prefab;
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

    //
    //  Need a Collected property
    //
    [SerializeField] private bool laser360Enabled = false;
    public bool Laser360Enabled
    {
        get { return laser360Enabled; }
        set
        {
            laser360Enabled = value;
            //StartCoroutine(PowerUpTripleShot());
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
    public void FireWeapon()
    {
        if (Laser360Enabled) { StartCoroutine(Fire360Laser()); return; }
        
        if (!laserCanFire)   { return; }
        if (AmmoCount < 1)   { return; }

        if   ( TripleShotEnabled )  
             { FireTripleShot(); }
        else { FireLaser();      }

        laserCanFire = false;
        StartCoroutine(LaserCoolDown());
    }

    private void FireLaser()
    {
        Instantiate( laserPrefab
                   , transform.position + laserOffest
                   , Quaternion.identity);
        AmmoCount = AmmoCount - 1;
    }

    //
    // Weapons Fire
    //

    private IEnumerator Fire360Laser()
    {
        Laser360Enabled = false;
        for (int i = 0; i < 36; i++)
        {
            GameObject fire = Instantiate(laser360Prefab, transform.position, Quaternion.identity);
            fire.transform.Rotate(0, 0, fire.transform.rotation.z + (10 * i));
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void FireTripleShot()
    {
        Instantiate( tripleShotPrefab
                   , transform.position + laserOffest
                   , Quaternion.identity);
        AmmoCount = AmmoCount - 3;
    }

    public void CollectAmmo() { AmmoCount = AmmoCount + ammoCollectible; }

    //
    // Watchdogs            ============================================================
    //

    private IEnumerator Laser360CoolDown()
    {
        yield return new WaitForSeconds(0.5f);
    }

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
