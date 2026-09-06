using UnityEngine;

public class LightSword : MonoBehaviour
{
    // LightSwordAttack is a 30fps clip: the blade is out and dangerous for its first 26 frames,
    // then it visibly shrinks away and must stop hurting. LightSwordSpawn never deals damage.
    private const float AttackClipFrameRate = 30f;
    private const float DamageFrames = 26f;

    private Animator anim;
    private BossAttackHitbox hitbox;
    private bool damageWindowStarted;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        hitbox = GetComponent<BossAttackHitbox>();
    }

    private void Update()
    {
        if (damageWindowStarted || hitbox == null || anim == null) return;

        // GetNextAnimatorStateInfo catches the attack the instant the transition out of
        // LightSwordSpawn begins - that is frame 0 of LightSwordAttack. Waiting for it to become
        // the current state would only see it once the 0.25s blend is over, several frames late.
        bool attackStarted = anim.GetNextAnimatorStateInfo(0).IsName("LightSwordAttack")
                             || anim.GetCurrentAnimatorStateInfo(0).IsName("LightSwordAttack");
        if (!attackStarted) return;

        damageWindowStarted = true;
        hitbox.Activate(DamageFrames / AttackClipFrameRate);
    }

    // Richiamabile tramite Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
