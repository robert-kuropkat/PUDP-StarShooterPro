using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shields : MonoBehaviour
{
    //
    // Timers, counters, etc.
    //

    [SerializeField] private int shieldsUpTimer      = 15;
    [SerializeField] private int shieldsWarningTimer = 5;
    [SerializeField] private int hitsLeft            = 0;
    [SerializeField] private int hitLimit            = 3;

    //
    // Game Objects
    //

    [SerializeField] private SpriteRenderer spriteRenderer;

    //
    // Properties
    //

    [SerializeField] private bool isActive = false;
    public bool IsActive
    {
        get { return isActive;  }
        set 
        { 
            switch (value)
            {
                case false:
                    isActive = value;
                    gameObject.SetActive(value);
                    StopCoroutine(ShieldTimer());
                    hitsLeft = 0;
                    break;
                case true:
                    isActive = value;
                    gameObject.SetActive(value);
                    hitsLeft = hitLimit;
                    StartCoroutine(ShieldTimer());
                    break;
            }

        }
    }

    //
    // Game Control
    //

    private void NullCheckOnStartup()
    {
        if (spriteRenderer == null) { Debug.Log("My Sprite Render is NULL!"); }
    }

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        NullCheckOnStartup();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Enemy":
                TakeDamage();
                break;
            case "Enemy Laser":
                if (other.transform.parent.GetComponent<EnemyFire>().HasHit) { return; }
                other.transform.parent.GetComponent<EnemyFire>().HasHit = true;
                TakeDamage();
                break;
            default:
                break;
        }
    }

    //
    // Watchdogs
    //

    private IEnumerator ShieldTimer()
    {
        //
        // Set timer
        //
        yield return new WaitForSeconds(shieldsUpTimer - shieldsWarningTimer);
        //
        // Warning flash
        //
        for (int i=0; i < shieldsWarningTimer; i++)
        {
            spriteRenderer.color = Color.black;
            yield return new WaitForSeconds(0.5f);
            ShieldColor();
            yield return new WaitForSeconds(0.5f);
        }
        //
        // timeout/reset
        //
        spriteRenderer.color = Color.white;
        this.IsActive = false;
    }

    //
    // Helper Methods
    //

    private void TakeDamage()
    {
        hitsLeft--;
        if (hitsLeft < 1) { this.IsActive = false; }
        ShieldColor();
    }

    private void ShieldColor()
    {
        switch (hitsLeft)
        {
            case 3:
                spriteRenderer.color = Color.white;
                break;
            case 2:
                spriteRenderer.color = new Color(1f, .37f, 0, 1);  //  Weird multiplier intended to get a yellowish color.
                break;
            case 1:
                spriteRenderer.color = Color.red;
                break;
            case 0:
                spriteRenderer.color = Color.white;
                break;
        }
    }

}
