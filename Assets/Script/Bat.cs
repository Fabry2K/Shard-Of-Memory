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
                    anim.SetBool("Idle", false);
                    anim.SetBool("Chase", true);
                    ChangeState(EnemyStates.Bat_Chasing);
                }
                break;

            case EnemyStates.Bat_Chasing:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));

                if (_dist > chaseDistance)
                {
                    anim.SetBool("Idle", true);
                    anim.SetBool("Chase", false);
                    ChangeState(EnemyStates.Bat_Idle);
                }

                FlipBat();
                break;

            case EnemyStates.Bat_Stunned:

                break;
        }
    }


    void FlipBat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }

    protected override void Death()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("Death");

        //rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 12;
        rb.simulated = false;

    }
}
