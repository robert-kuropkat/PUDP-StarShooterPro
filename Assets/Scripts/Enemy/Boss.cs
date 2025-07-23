using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private BossState     CurrentState { get; set; } = BossState.Enter;
    [SerializeField] private float         MySpeed      { get; set; } = 2f;
    [SerializeField] private float         StopSpot     { get; set; } = 3.5f;
    [SerializeField] private float         HoverSpot    { get; set; } = 6.75f;
    [SerializeField] private GameManager   gameManager;
    [SerializeField] private Animator      myExplosion;
    [SerializeField] private Player        myPlayer;
    //[SerializeField] private GameManager   gameManager;

    private int Health { get; set; } = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (CurrentState)
        {
            case BossState.None:
                break;
            case BossState.Enter:
                Enter();
                break;
            case BossState.Threaten:
                Threaten();
                break;
            case BossState.Return:
                Return();
                break;
            case BossState.Hover:
                Hover();
                break;
            case BossState.Attack:
                Attack();
                break;
            default:
                break;
        }
    }

    private void Enter()
    {
        transform.Translate(Vector3.down * (MySpeed * Time.deltaTime));
        if (transform.position.y <= StopSpot)
            CurrentState = BossState.Threaten;
    } 

    private void Threaten()
    {
        StartCoroutine(ChangeStateWithDelay(1f, BossState.Return));
    }

    private void Return()
    {
        transform.Translate(Vector3.up * (MySpeed * Time.deltaTime));
        if (transform.position.y >= HoverSpot)
            CurrentState = BossState.Hover;
    }

    private void Hover()
    {
        CurrentState = BossState.None;
    }

    private void Attack()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Taking a hit");
        if (Health < 1) { return; }
        if (other.CompareTag("Laser")) { Health--; }
        if (Health < 1) { DeathScene(); }
    }

    private void DeathScene()
    {
        Debug.Log("Dead Boss");
        gameManager.GameOver = true;
        NotifyPlayer();
        StartCoroutine(TriggerExplosions());
        Destroy(this.gameObject, 2.2f);
    }

    private void NotifyPlayer() { myPlayer.EnemyDestroyed(); }

    private IEnumerator TriggerExplosions()
    {
        Vector3 _myPos = transform.position;
        Instantiate(myExplosion, _myPos + new Vector3(  0, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3(  2, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3( -2, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3(  4, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3( -4, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3(  6, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3( -6, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3(  8, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3( -8, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3( 10, 0, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Instantiate(myExplosion, _myPos + new Vector3(-10, 0, 0), Quaternion.identity);
        Debug.Log("X = +- 10");
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator ChangeStateWithDelay(float delay, BossState newState)
    {
        yield return new WaitForSeconds(delay);
        CurrentState = newState;
    }
}
