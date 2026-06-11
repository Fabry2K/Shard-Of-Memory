using UnityEngine;

public class Skeleton : Enemy
{
    float timer;

    [SerializeField] private float flipWaitTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }

    protected override void UpdateEnemyStates()
    {

        switch (currentEnemyState)
        {
            case EnemyStates.Skeleton_Idle:
                Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Skeleton_Flip);
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
                timer += Time.deltaTime;

                if(timer > flipWaitTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Skeleton_Idle);
                }
                break;
        }
    }

}
