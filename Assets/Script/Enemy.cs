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

    [SerializeField] protected GameObject blood;

    [SerializeField] AudioClip hurtSound;


     float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;

    protected Animator anim;
    protected bool isDead;

    protected AudioSource audioSource;

    protected enum EnemyStates
    {
        //skeleton
        Skeleton_Walk,
        Skeleton_Chasing,
        Skeleton_Flip,
        Skeleton_Attack,
        Skeleton_BackToOriginalPosition,

        //bat
        Bat_Idle,
        Bat_Chasing,
        Bat_Stunned
    }

    protected EnemyStates currentEnemyState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        player = PlayerController.Instance;
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }


    // Update is called once per frame
    protected virtual void Update()
    {
        if (GameManager.Instance.gameIsPaused) return;
        UpdateEnemyStates();

        if (health <= 0)
        {
            //Destroy(gameObject);
            Death();
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
        audioSource.PlayOneShot(hurtSound);

        if (!isRecoiling)
        {
            //audioSource.PlayOneShot(hurtSound);
            rb.linearVelocity = _hitForce * recoilFactor * _hitDirection;
        }

        GameObject _blood = Instantiate(blood, transform.position, Quaternion.identity);
        DestroyObject(_blood, 5.5f);
    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            DealDamage();
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.3f);
            }
            
        }
    }

    protected virtual void UpdateEnemyStates()
    {

    }

    protected void ChangeState(EnemyStates _newState)
    {
        currentEnemyState = _newState;
    }

    protected virtual void DealDamage()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

    protected virtual void Death()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("Death");

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }



}
