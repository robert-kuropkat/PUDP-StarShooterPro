using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShields : MonoBehaviour
{

    //
    // Timers, counters, etc.
    //

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
