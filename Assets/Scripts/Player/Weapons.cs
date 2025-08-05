using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapons : MonoBehaviour
{
    //
    // Speed, Boundaries and Timers
    //
    [SerializeField] private float   fireRate                = .15f;
    [SerializeField] private float   tripleShotTimer         = 5f;
    [SerializeField] private int     ammoCollectible         = 15;
    [SerializeField] private int     negativeAmmoCollectible = 7;
    [SerializeField] private int     tripleShotCollectible   = 1;
    [SerializeField] private int     torpedoCollectible      = 1;
    [SerializeField] private int     spiralShotCollectible   = 1;
    [SerializeField] private Vector3 laserOffest             = new Vector3(0, 1.006f, 0);
    //
    // Flags
    //
    [SerializeField] private bool laserCoolDown       = false;
    //
    // Game Objects populated in Inspector
    //
    [SerializeField] private GameObject  laserPrefab;
    [SerializeField] private GameObject  tripleShotPrefab;
    [SerializeField] private GameObject  laser360Prefab;
    [SerializeField] private GameObject  torpedoPrefab;
    [SerializeField] private UIManager   uiManager;
    [SerializeField] private Boss        enemyBoss;
    [SerializeField] private GameManager myGameManager;

    private void NullCheckOnStartup()
    {
        if (laserPrefab == null) { Debug.LogError("The Laser Prefab is NULL."); }
        if (tripleShotPrefab == null) { Debug.LogError("The TripleShot PowerUp Prefab is NULL"); }
        if (uiManager == null) { Debug.LogError("The UI Manager is NULL"); }
    }

    void Start()
    {
        NullCheckOnStartup();
        uiManager.AmmoCount(Laser.Count);
    }
    
    //
    // Public Methods
    //

    public void SetCurrent()
    {

    }

    public void DisarmWeapons()
    {
        Laser.Armed      = false;
        TripleShot.Armed = false;
        Torpedo.Armed    = false;
        SpiralShot.Armed = false;

        uiManager.DisArmLaser();
        uiManager.DisArmTripleShot();
        uiManager.DisArmTorpedo();
        uiManager.DisArmSpiralLaser();

        uiManager.DisArmTripleShot();
    }

    public void ArmLaser()
    {
        DisarmWeapons();
        Laser.Armed = true;
        uiManager.ArmLaser();
    }
    public void ArmTripleShot()
    {
        DisarmWeapons();
        if (TripleShot.Count < 1) { return; }
        TripleShot.Armed = true;
        uiManager.ArmTripleShot();
        StartCoroutine(PowerUpTripleShot());
    }

    public void ArmTorpedo()
    {
        DisarmWeapons();
        if (Torpedo.Count < 1) { return; }
        Torpedo.Armed = true;
        uiManager.ArmTorpedo();
    }

    public void ArmSpiralShot()
    {
        DisarmWeapons();
        if (SpiralShot.Count < 1) { return; }
        SpiralShot.Armed = true;
        uiManager.ArmSpiralLaser();
    }

    public void FireWeapon()
    {
        if (myGameManager.CurrentWave > 4 && enemyBoss.CurrentState != BossState.Hover) { return; }
        if (SpiralShot.Enabled && SpiralShot.Armed) { StartCoroutine(Fire360Laser()); return; }
        if (Torpedo.Enabled && Torpedo.Armed)  { FireTorpedo(); return; }

        if (laserCoolDown)   { return; }
        if (Laser.Count < 1) { return; }

        if   ( TripleShot.Armed )  
             { FireTripleShot(); }
        else { FireLaser();      }

        laserCoolDown = true;
        StartCoroutine(LaserCoolDown());
    }

    public void CollectAmmo() 
    { 
        Laser.Count += ammoCollectible;
        Laser.Enabled = true;
        uiManager.AmmoCount(Laser.Count); 
    }

    public void CollectTripelShot()
    {
        TripleShot.Count += tripleShotCollectible;
        TripleShot.Enabled = true;
        uiManager.TripleShotCount(TripleShot.Count);
    }

    public void ReduceAmmo() 
    { 
        Laser.Count -= negativeAmmoCollectible; 
        uiManager.AmmoCount(Laser.Count); 
    }

    public void CollectTorpedo() 
    { 
        Torpedo.Count += torpedoCollectible;
        Torpedo.Enabled = true;
        uiManager.TorpedoCount(Torpedo.Count); 
    }

    public void CollectSpiralShot()
    {
        SpiralShot.Count += spiralShotCollectible;
        SpiralShot.Enabled = true;
        uiManager.SpiralLaserCount(SpiralShot.Count);
    }

    //
    // Weapons Fire
    //

    private IEnumerator Fire360Laser()
    {
        for (int i = 0; i < 36; i++)
        {
            GameObject fire = Instantiate(laser360Prefab, transform.position, Quaternion.identity);
            fire.transform.Rotate(0, 0, fire.transform.rotation.z + (10 * i));
            yield return new WaitForSeconds(0.1f);
        }
        SpiralShot.Armed = false;
        SpiralShot.Count--;
            uiManager.SpiralLaserCount(SpiralShot.Count);
        DisarmWeapons();
        if (SpiralShot.Count < 1) { SpiralShot.Enabled = false; }
        uiManager.DisArmSpiralLaser();
        ArmLaser();
    }

    private void FireTorpedo()
    {
        Torpedo.FindTarget();
        if (!Torpedo.MyTarget) { return; }
        GameObject shot = 
        Instantiate( torpedoPrefab
                   , transform.position + laserOffest
                   , Quaternion.identity);
        Torpedo.Count--;
        uiManager.TorpedoCount(Torpedo.Count);
        if (Torpedo.Count<1)
        {
            Debug.Log("Disarm Torpedo");
            Torpedo.Enabled = false;
            DisarmWeapons();
            ArmLaser();
        }
    }

    private void FireLaser()
    {
        Instantiate(laserPrefab
                   , transform.position + laserOffest
                   , Quaternion.identity);
        Laser.Count -= 1;
    }

    private void FireTripleShot()
    {
        Instantiate( tripleShotPrefab
                   , transform.position + laserOffest
                   , Quaternion.identity);
        Laser.Count -= 3;
    }


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
        laserCoolDown = false;
    }

    private IEnumerator PowerUpTripleShot()
    {
        yield return new WaitForSeconds(tripleShotTimer);
        TripleShot.Count--;
        uiManager.TripleShotCount(TripleShot.Count);
        DisarmWeapons();
        ArmLaser();
    }
}
