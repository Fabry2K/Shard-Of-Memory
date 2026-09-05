using UnityEngine;

public class Bat : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float loseInterestDistance;
    [SerializeField] private float postEngageSpeedMultiplier = 0.25f;

    private bool wantsToChase;
    private bool hasEngaged;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentEnemyState = EnemyStates.Bat_Idle;

        if (loseInterestDistance <= 0f)
        {
            loseInterestDistance = chaseDistance * 1.6f;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Bat_Idle);
        }

    }

    private void FixedUpdate()
    {
        if (wantsToChase)
        {
            float currentSpeed = hasEngaged ? speed * postEngageSpeedMultiplier : speed;
            rb.MovePosition(Vector2.MoveTowards(rb.position, PlayerController.Instance.transform.position, Time.fixedDeltaTime * currentSpeed));
        }
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (currentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                wantsToChase = false;

                if(_dist < chaseDistance)
                {
                    anim.SetBool("Idle", false);
                    anim.SetBool("Chase", true);
                    ChangeState(EnemyStates.Bat_Chasing);
                }
                break;

            case EnemyStates.Bat_Chasing:
                wantsToChase = true;
                FlipBat();

                if (_dist > loseInterestDistance)
                {
                    anim.SetBool("Idle", true);
                    anim.SetBool("Chase", false);
                    ChangeState(EnemyStates.Bat_Idle);
                }

                break;

            case EnemyStates.Bat_Stunned:
                wantsToChase = false;
                break;
        }
    }

    protected override void DealDamage()
    {
        base.DealDamage();
        hasEngaged = true;
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        hasEngaged = true;
    }

    void FlipBat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

    protected override void Death()
    {
        if (isDead) return;

        isDead = true;
        wantsToChase = false;

        anim.SetTrigger("Death");

        //rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 12;
        rb.simulated = false;

    }
}
