using UnityEngine;
public class EnemyAttackHitbox : MonoBehaviour
{
    private Enemy enemy;

    private void Start()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            //enemy.DoAttackHit();
            Debug.Log("Attack");
        }
    }
}