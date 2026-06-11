using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;

    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rb;

    protected Animator anim;
    protected bool isDead;

    protected enum EnemyStates
    {
        //skeleton
        Skeleton_Idle,
        Skeleton_Flip
    }
    protected EnemyStates currentEnemyState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
        anim = GetComponentInChildren<Animator>();
        Debug.Log(anim);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateEnemyStates();

        if(health <= 0)
        {
            //Destroy(gameObject);
            Die();
        }

        if (isRecoiling)
        {
            if(recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            } else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;

        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.3f);
        }
    }

    protected virtual void UpdateEnemyStates()
    {

    }

    protected void ChangeState(EnemyStates _newState)
    {
        currentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("Die");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Destroy(gameObject, 1f);
    }
}
