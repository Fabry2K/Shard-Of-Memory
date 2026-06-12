using UnityEngine;

public class Skeleton : Enemy
{
    float flipTimer;
    float maxTimer;

    [SerializeField] private float flipWaitTime;
    [SerializeField] private float walkMaxTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] protected DetectionBox attackZone;
    [SerializeField] protected DetectionBox playerDetection;

    private Vector2 chaseStartPosition;
    private bool canHit = false;
    private bool hitted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        currentEnemyState = EnemyStates.Skeleton_Walk;
        rb.gravityScale = 12f;
    }

    protected override void UpdateEnemyStates()
    {
        switch (currentEnemyState)
        {
            case EnemyStates.Skeleton_Walk:
                maxTimer += Time.deltaTime;

                if (maxTimer > walkMaxTime && walkMaxTime > 0)
                {
                    maxTimer = 0;
                    ChangeState(EnemyStates.Skeleton_Flip);
                }

                Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Skeleton_Flip);
                }

                anim.SetBool("HasTarget", attackZone.detectedColliders.Count > 0);
                if (anim.GetBool("HasTarget"))
                {
                    ChangeState(EnemyStates.Skeleton_Attack);
                }

                anim.SetBool("Chasing", playerDetection.detectedColliders.Count > 0);
                if (anim.GetBool("Chasing"))
                {
                    chaseStartPosition = transform.position;
                    ChangeState(EnemyStates.Skeleton_Chasing);
                }


                if (transform.localScale.x > 0)
                {
                    rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
                } else
                {
                    rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
                }


                break;

            case EnemyStates.Skeleton_Flip:
                flipTimer += Time.deltaTime;

                if(flipTimer > flipWaitTime)
                {
                    flipTimer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Skeleton_Walk);
                }
                break;

            case EnemyStates.Skeleton_Attack:

                anim.SetBool("HasTarget", attackZone.detectedColliders.Count > 0);

                rb.linearVelocity = Vector2.zero;

                if (canHit && !hitted && anim.GetBool("HasTarget"))
                {
                    hitted = true;
                    DealDamage();
                }


                break;

            case EnemyStates.Skeleton_Chasing:
                anim.SetBool("Chasing", playerDetection.detectedColliders.Count > 0);

                if (!anim.GetBool("Chasing"))
                {
                    ChangeState(EnemyStates.Skeleton_BackToOriginalPosition);
                }

                float direction = player.transform.position.x - transform.position.x;


                if (direction > 0)
                {
                    
                    if (transform.localScale.x < 0)
                    {
                        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    }

                    rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
                }
                else
                {
                    
                    if (transform.localScale.x > 0)
                    {
                        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    }

                    rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
                }

                // attack trigger
                anim.SetBool("HasTarget", attackZone.detectedColliders.Count > 0);
                if (anim.GetBool("HasTarget"))
                {
                    ChangeState(EnemyStates.Skeleton_Attack);
                }

                break;

            case EnemyStates.Skeleton_BackToOriginalPosition:

                // back to chasing
                anim.SetBool("Chasing", playerDetection.detectedColliders.Count > 0);
                if (anim.GetBool("Chasing"))
                {
                    chaseStartPosition = transform.position;
                    ChangeState(EnemyStates.Skeleton_Chasing);
                }

                Vector2 dir = chaseStartPosition - (Vector2)transform.position;

                float distance = dir.magnitude;

                // se è arrivato abbastanza vicino
                if (distance < 0.1f)
                {
                    rb.linearVelocity = Vector2.zero;
                    ChangeState(EnemyStates.Skeleton_Walk);
                    break;
                }

                // normalizza direzione
                dir = dir.normalized;

                rb.linearVelocity = new Vector2(dir.x * speed, rb.linearVelocity.y);

                // flip visivo opzionale
                if (dir.x > 0 && transform.localScale.x < 0)
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                else if (dir.x < 0 && transform.localScale.x > 0)
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);


                break;
        }
    }

    protected void EnableHit()
    {
        canHit = true;
    }

    protected void DisableHit()
    {
        canHit = false;
    }

    protected void EndAttack()
    {
        hitted = false;
        canHit = false;
        ChangeState(EnemyStates.Skeleton_Walk);
    }

}
