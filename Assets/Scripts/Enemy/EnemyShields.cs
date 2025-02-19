using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShields : MonoBehaviour
{

    //
    // Timers, counters, etc.
    //

    //[SerializeField] private int shieldsUpTimer = 15;
    //[SerializeField] private int shieldsWarningTimer = 5;
    //[SerializeField] private int hitsLeft = 0;
    //[SerializeField] private int hitLimit = 3;

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
        get { return isActive; }
        set
        {
            switch (value)
            {
                case false:
                    isActive = value;
                    gameObject.SetActive(value);
                    break;
                case true:
                    isActive = value;
                    gameObject.SetActive(value);
                    break;
            }

        }
    }

    //
    // Game Control
    //

    private void NullCheckOnStartup()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) { Debug.Log("My Sprite Render is NULL!"); }
    }

    void Start()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        NullCheckOnStartup();
    }

    //
    //
    //

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Player":
                TakeDamage();
                break;
            case "Laser":
//                if (other.transform.parent.GetComponent<EnemyFire>().HasHit) { return; }
//                other.transform.parent.GetComponent<EnemyFire>().HasHit = true;
                TakeDamage();
                break;
            default:
                break;
        }
    }

    //
    // Helper Methods
    //

    private void TakeDamage()
    {
        this.IsActive = false;
    }


}
