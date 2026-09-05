using UnityEngine;

// A reusable, normally-disabled damage zone for the boss's scripted attacks.
// The boss code enables it for a short window at the moment an attack should be
// able to connect, instead of leaving a permanent damaging collider around.
public class BossAttackHitbox : MonoBehaviour
{
    [SerializeField] private float damage = 1f;

    [Tooltip("If set, the hitbox activates itself as soon as it exists instead of waiting for an external Activate() call - useful when it lives on a standalone VFX prefab (e.g. a thrown blade) rather than being toggled by BossController.")]
    [SerializeField] private bool activateOnAwake;
    [SerializeField] private float activeDurationOnAwake = 999f;
    [Tooltip("Delay before the activateOnAwake self-activation actually enables the collider - use this when the visual needs time to grow/extend before it should start dealing damage (e.g. a tendril that only hits once fully stretched out).")]
    [SerializeField] private float activateDelay;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (activateOnAwake)
        {
            if (activateDelay > 0f) Invoke(nameof(ActivateFromAwake), activateDelay);
            else Activate(activeDurationOnAwake);
        }
    }

    private void ActivateFromAwake()
    {
        Activate(activeDurationOnAwake);
    }

    public void SetDamage(float value)
    {
        damage = value;
    }

    public void Activate(float duration)
    {
        if (col == null) return;

        col.enabled = true;
        CancelInvoke(nameof(Deactivate));
        Invoke(nameof(Deactivate), duration);
    }

    public void Deactivate()
    {
        if (col != null) col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (PlayerController.Instance == null || PlayerController.Instance.pState.invincible) return;

        PlayerController.Instance.TakeDamage(damage);
    }
}
