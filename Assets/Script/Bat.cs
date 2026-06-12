using UnityEngine;

public class Bat : Enemy
{
    [SerializeField] private float chaseDistance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentEnemyState = EnemyStates.Bat_Idle;
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (currentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                if(_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Bat_Chasing);
                }
                break;

            case EnemyStates.Bat_Chasing:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));

                FlipBat();
                break;

            case EnemyStates.Bat_Stunned:

                break;
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;

        if (!isRecoiling)
        {
            rb.linearVelocity = _hitForce * recoilFactor * _hitDirection;
        }

        GameObject _blood = Instantiate(blood, transform.position, Quaternion.identity);
        DestroyObject(_blood, 5.5f);
    }

    void FlipBat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }
}
