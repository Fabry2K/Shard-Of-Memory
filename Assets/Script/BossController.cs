using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Pure Vessel boss AI. Inherits Enemy so the existing damage pipelines already used by the
// player's sword (PlayerController.Hit -> Enemy.EnemyHit) and spells (Fireball -> Enemy.EnemyHit)
// work against the boss without touching those scripts.
//
// Enemy.OnCollisionStay2D (generic body-contact damage) is deliberately hidden below: every
// hit the boss lands comes from a scripted attack's own BossAttackHitbox instead. Letting the
// body itself also deal contact damage caused incidental overlaps (e.g. Light Lancer's landing
// slam) to burn the player's hit-invincibility window a frame before the real attack hitbox
// (the ground swords) got a chance to register, making that attack seem to deal no damage at all.
//
// None of the boss's animation clips carry root motion or position curves, so every bit of
// on-screen movement (teleport repositioning, the jump-attack arc, the lunge dash) is driven
// from here in code, timed against the clips' own lengths and existing Animation Events.
public class BossController : Enemy
{
    [Header("Detection")]
    [SerializeField] private DetectionBox spawnDetectionBox;

    [Header("Arena")]
    [SerializeField] private Collider2D arenaBounds;
    [SerializeField] private float teleportEdgeMargin = 2f;

    [Header("Attack Hitboxes")]
    [SerializeField] private BossAttackHitbox meleeHitbox;

    [Header("AI Tuning")]
    [SerializeField] private float closeRangeDistance = 6f;
    [SerializeField] private float attackRecoveryTime = 0.6f;
    [SerializeField] private float phase2HealthFraction = 0.5f;
    [SerializeField] private float lungeSpeed = 22f;
    [SerializeField] private float lungeDuration = 0.35f;
    [SerializeField] private float attackDamage = 1f;
    [SerializeField] private AudioClip bossHurtSound;

    [Header("Effects")]
    [SerializeField] private GameObject breakEffect;
    [SerializeField] private Transform breakEffectPoint;

    [SerializeField] private GameObject whiteSpawnEffect;
    [SerializeField] private Transform whiteSpawnEffectPoint;

    [SerializeField] private GameObject blackSpawnEffect;
    [SerializeField] private Transform blackSpawnEffectPoint;

    [SerializeField] private GameObject circleSpawnEffect;
    [SerializeField] private Transform circleSpawnEffectPoint;

    [SerializeField] private GameObject dashEffect;
    [SerializeField] private Transform dashEffectPoint;

    [SerializeField] private GameObject linesSpawnEffect;
    [SerializeField] private Transform linesSpawnEffectPoint;

    [SerializeField] private GameObject lightSwordEffect;
    [SerializeField] private Transform lightSwordEffectPoint;

    [SerializeField] private GameObject groundHitEffect;
    [SerializeField] private Transform groundHitEffectPoint;

    [SerializeField] private GameObject voidTendrillsEffect;
    [SerializeField] private Transform voidTendrillsEffectPoint;

    [SerializeField] private GameObject castEffect;
    [SerializeField] private Transform castEffectPoint;

    [SerializeField] private GameObject castSmallEffect;
    [SerializeField] private Transform castSmallEffectPoint;

    [SerializeField] private GameObject bladesEffect;
    [SerializeField] private Transform bladesEffectPoint;

    [Header("Sounds")]
    [SerializeField] private AudioClip fightSong;
    [SerializeField] private AudioClip rumbleSound;
    [SerializeField] private AudioClip stompSound;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip groundHitSong;
    [SerializeField] private AudioClip slash1;
    [SerializeField] private AudioClip slash2;
    [SerializeField] private AudioClip slash3;

    private enum AttackType { TripleSlash, Lunge, ShiningDagger, LightLancer, VoidTendrils, Focus }

    private bool hasSpawned;
    private bool aiRunning;
    private int facingDirection = 1;
    private AttackType lastAttack;
    private bool hasLastAttack;
    private float maxHealthAtStart;

    protected override void Start()
    {
        base.Start();
        maxHealthAtStart = health;
        facingDirection = transform.localScale.x >= 0 ? 1 : -1;
    }

    protected override void UpdateEnemyStates()
    {
        if (!hasSpawned && PlayerDetected())
        {
            StartSpawn();
        }
    }

    private bool PlayerDetected()
    {
        if (spawnDetectionBox == null) return false;
        foreach (var col in spawnDetectionBox.detectedColliders)
        {
            if (col != null && col.CompareTag("Player")) return true;
        }
        return false;
    }

    public void StartSpawn()
    {
        hasSpawned = true;
        anim.SetTrigger("Spawn");
        StartCoroutine(WaitForSpawnThenBeginAI());
    }

    private IEnumerator WaitForSpawnThenBeginAI()
    {
        // Boss_start is ~12.27s and the Animator already transitions itself to Boss_idle at the end.
        yield return new WaitForSeconds(12.4f);
        aiRunning = true;
        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (aiRunning && health > 0f && !isDead)
        {
            yield return StartCoroutine(TeleportAndFace());

            AttackType attack = ChooseAttack();
            yield return StartCoroutine(PerformAttack(attack));
            lastAttack = attack;
            hasLastAttack = true;

            if (isDead) yield break;

            anim.Play("Boss_idle");

            bool isPhase2 = health <= maxHealthAtStart * phase2HealthFraction;
            float recovery = isPhase2 ? attackRecoveryTime * 0.5f : attackRecoveryTime;
            yield return new WaitForSeconds(recovery);

            // In the second half of its health, the boss sometimes chains a second attack
            // from where it's standing before teleporting away again - a bit more relentless.
            if (isPhase2 && Random.value < 0.5f && health > 0f && !isDead)
            {
                AttackType secondAttack = ChooseAttack();
                yield return StartCoroutine(PerformAttack(secondAttack));
                lastAttack = secondAttack;

                if (isDead) yield break;

                anim.Play("Boss_idle");
                yield return new WaitForSeconds(recovery);
            }
        }
    }

    private AttackType ChooseAttack()
    {
        float dist = player != null ? Vector2.Distance(transform.position, player.transform.position) : 999f;
        List<AttackType> pool = new List<AttackType>();

        if (dist <= closeRangeDistance)
        {
            pool.Add(AttackType.TripleSlash);
            pool.Add(AttackType.Lunge);
        }
        else
        {
            pool.Add(AttackType.ShiningDagger);
            pool.Add(AttackType.LightLancer);
            pool.Add(AttackType.VoidTendrils);
            pool.Add(AttackType.Focus);
        }

        if (hasLastAttack && pool.Count > 1)
        {
            pool.Remove(lastAttack);
        }

        return pool[Random.Range(0, pool.Count)];
    }

    private IEnumerator PerformAttack(AttackType attack)
    {
        switch (attack)
        {
            case AttackType.TripleSlash: yield return StartCoroutine(DoTripleSlash()); break;
            case AttackType.Lunge: yield return StartCoroutine(DoLunge()); break;
            case AttackType.ShiningDagger: yield return StartCoroutine(DoShiningDagger()); break;
            case AttackType.LightLancer: yield return StartCoroutine(DoLightLancer()); break;
            case AttackType.VoidTendrils: yield return StartCoroutine(DoVoidTendrils()); break;
            case AttackType.Focus: yield return StartCoroutine(DoFocus()); break;
        }
    }

    // --- Teleport + turn to face the player (boss_teleport is used to reposition to an edge
    // of the arena, then the boss flips to face the player before picking its next attack) ---

    private IEnumerator TeleportAndFace()
    {
        const float teleportClipLength = 0.9333333f;
        const float halfClip = teleportClipLength / 2f;

        anim.Play("Boss_Teleport");

        // The clip fades the sprite to fully transparent around its midpoint and back in by the
        // end - reposition exactly while invisible so the teleport reads as instantaneous.
        yield return new WaitForSeconds(halfClip);
        transform.position = ComputeTeleportTarget();
        yield return new WaitForSeconds(teleportClipLength - halfClip);

        yield return StartCoroutine(FacePlayer());
    }

    private Vector3 ComputeTeleportTarget()
    {
        if (arenaBounds == null) return transform.position;

        Bounds b = arenaBounds.bounds;
        float minX = b.min.x + teleportEdgeMargin;
        float maxX = b.max.x - teleportEdgeMargin;

        bool goLeft = Random.value < 0.5f;
        float x = goLeft ? minX : maxX;

        return new Vector3(x, transform.position.y, transform.position.z);
    }

    private IEnumerator FacePlayer()
    {
        if (player == null) yield break;

        int desiredDirection = player.transform.position.x >= transform.position.x ? 1 : -1;
        if (desiredDirection != facingDirection)
        {
            facingDirection = desiredDirection;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facingDirection;
            transform.localScale = scale;

            anim.Play("Flip");
            yield return new WaitForSeconds(0.0833334f);
        }
    }

    // Effects that aren't parented to the boss (they're free-standing Instantiate calls) don't
    // automatically mirror when the boss flips to face left - this compensates so a directional
    // effect (thrown blades, ground tendrils reaching toward the player, etc.) points the right way.
    private GameObject SpawnEffectFacingDirection(GameObject prefab, Vector3 position)
    {
        GameObject fx = Instantiate(prefab, position, Quaternion.identity);
        Vector3 s = fx.transform.localScale;
        fx.transform.localScale = new Vector3(Mathf.Abs(s.x) * facingDirection, s.y, s.z);
        return fx;
    }

    private Vector3 ClampToArena(Vector3 pos)
    {
        if (arenaBounds == null) return pos;
        Bounds b = arenaBounds.bounds;
        float x = Mathf.Clamp(pos.x, b.min.x + 0.5f, b.max.x - 0.5f);
        return new Vector3(x, pos.y, pos.z);
    }

    // --- Triple Slash: three quick melee hits in place ---

    private IEnumerator DoTripleSlash()
    {
        anim.Play("BossTripleSlash");
        yield return new WaitForSeconds(1.3666667f);
    }

    // --- Lunge: a fast forward dash-strike along the ground ---

    private IEnumerator DoLunge()
    {
        const float clipLength = 1.2f;
        const float dashStart = 0.4666667f; // matches the existing SpawnDashEffect event

        anim.Play("Boss_landing");
        yield return new WaitForSeconds(dashStart);

        if (meleeHitbox != null)
        {
            meleeHitbox.transform.position = transform.position;
            meleeHitbox.Activate(lungeDuration + 0.1f);
        }

        Vector3 start = transform.position;
        Vector3 end = ClampToArena(start + new Vector3(lungeSpeed * lungeDuration * facingDirection, 0f, 0f));

        float t = 0f;
        while (t < lungeDuration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(t / lungeDuration));
            yield return null;
        }
        transform.position = end;

        float remaining = clipLength - dashStart - lungeDuration;
        if (remaining > 0f) yield return new WaitForSeconds(remaining);
    }

    // --- Shining Dagger: throws the existing ThrowBladeEffect (4 blades at different angles,
    // wired as "bladesEffect") at the player. The blade prefab it spawns carries its own
    // BossAttackHitbox, activated as soon as it exists (see SpawnBladesEffect below). ---

    private IEnumerator DoShiningDagger()
    {
        anim.Play("Boss_throw_blade");
        yield return new WaitForSeconds(1.6333333f);
    }

    // --- Light Lancer: jumps toward the player and slams down ---

    private IEnumerator DoLightLancer()
    {
        // StartJumpAnimation() (existing animation event at t=0.3s) kicks off the jump arc below;
        // SpawnGroundSlashEffect (t=1.3s) activates the landing hitbox.
        anim.Play("Boss_Jump_Attack");
        yield return new WaitForSeconds(2.8333333f);
    }

    public void StartJumpAnimation()
    {
        StartCoroutine(BossAttackMovement());
    }

    private IEnumerator BossAttackMovement()
    {
        yield return StartCoroutine(Move(new Vector3(5.5f * facingDirection, 6.5f, 0f), 0.4f));
        yield return StartCoroutine(FreezeInAir(0.3f));
        yield return StartCoroutine(Move(new Vector3(2.5f * facingDirection, -7.5f, 0f), 0.25f));
    }

    private IEnumerator Move(Vector3 offset, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = ClampToArena(startPos + offset);

        float timer = 0f;
        while (timer < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    private IEnumerator FreezeInAir(float duration)
    {
        float oldGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duration);

        rb.gravityScale = oldGravity;
    }

    // --- Void Tendrils: a line of eruptions reaching toward the player ---

    private IEnumerator DoVoidTendrils()
    {
        anim.Play("Boss_void");
        yield return new WaitForSeconds(1.9666667f);
    }


    // --- Focus: channels then releases a burst around itself ---

    private IEnumerator DoFocus()
    {
        if (castEffect != null && castEffectPoint != null) Instantiate(castEffect, castEffectPoint.position, Quaternion.identity);
        anim.Play("Boss_cast");
        yield return new WaitForSeconds(2.1f);
    }

    // Wired to the boss_cast animation event at t=0.8333s.
    // castSmallEffect (SmalRingSeries.prefab) spawns the orb children; each orb's own
    // BossAttackHitbox activates when the orb explodes, not on spawn or generic AoE.
    public void CastFocusBurst()
    {
        if (castSmallEffect != null && castSmallEffectPoint != null)
        {
            Instantiate(castSmallEffect, castSmallEffectPoint.position, Quaternion.identity);
        }
    }

    // --- Damage taken / death ---

    // Hides Enemy.OnCollisionStay2D: see the class comment above for why the boss's body must
    // not deal contact damage on its own (Unity resolves magic methods to the most-derived
    // declaration, same pattern already used by Ghost.cs elsewhere in this project).
    protected new void OnCollisionStay2D(Collision2D _other) { }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;

        if (audioSource != null && bossHurtSound != null) audioSource.PlayOneShot(bossHurtSound);

        if (blood != null)
        {
            GameObject _blood = Instantiate(blood, transform.position, Quaternion.identity);
            Destroy(_blood, 1.5f);
        }
        // No knockback: a boss this size doesn't flinch back from a single hit.
    }

    protected override void Death()
    {
        if (isDead) return;
        isDead = true;
        aiRunning = false;
        StopAllCoroutines();

        if (meleeHitbox != null) meleeHitbox.Deactivate();

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        rb.simulated = false;

        if (breakEffect != null && breakEffectPoint != null) Instantiate(breakEffect, breakEffectPoint.position, Quaternion.identity);
        if (audioSource != null && breakSound != null) audioSource.PlayOneShot(breakSound);

        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {
        const float duration = 1.2f;
        float t = 0f;
        Color startColor = sr.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            Color c = startColor;
            c.a = Mathf.Lerp(startColor.a, 0f, t / duration);
            sr.color = c;
            yield return null;
        }

        Destroy(gameObject, 0.5f);
    }

    // --- Functions called from Animation Events (unchanged names/points from the original script) ---

    public void SpawnBreakEffect()
    {
        Instantiate(breakEffect, breakEffectPoint.position, Quaternion.identity);
    }

    public void SpawnWhiteEffect()
    {
        Instantiate(whiteSpawnEffect, whiteSpawnEffectPoint.position, Quaternion.identity);
    }

    public void SpawnBlackEffect()
    {
        Instantiate(blackSpawnEffect, blackSpawnEffectPoint.position, Quaternion.identity);
    }

    public void SpawnCircleEffect()
    {
        Instantiate(circleSpawnEffect, circleSpawnEffectPoint.position, Quaternion.identity);
    }

    public void SpawnLinesEffect()
    {
        Instantiate(linesSpawnEffect, linesSpawnEffectPoint.position, Quaternion.identity);
    }

    public void SpawnDashEffect()
    {
        Instantiate(dashEffect, dashEffectPoint.position, Quaternion.identity);
    }

    public void SpawnLightSwordEffect()
    {
        Instantiate(lightSwordEffect, lightSwordEffectPoint.position, Quaternion.identity);
    }

    public void SpawnGroundSlashEffect()
    {
        // groundHitEffect (groundSlashEffect.prefab) carries its own BossAttackHitbox
        // that activates itself on spawn - the swords erupting from the ground are
        // the actual damage source, not a boss-centered zone.
        Instantiate(groundHitEffect, groundHitEffectPoint.position, Quaternion.identity);
    }

    public void SpawnVoidTendrillsEffect()
    {
        // Void_Tendrills.prefab's own children (voidEffect1, void_1..4) each carry their own
        // BossAttackHitbox, self-activating once their own extend animation has played out.
        SpawnEffectFacingDirection(voidTendrillsEffect, voidTendrillsEffectPoint.position);
    }

    public void SpawnCastEffect()
    {
        Instantiate(castEffect, castEffectPoint.position, Quaternion.identity);
    }

    public void SpawnCastSmallEffect()
    {
        Instantiate(castSmallEffect, castSmallEffectPoint.position, Quaternion.identity);
    }

    public void SpawnBladesEffect()
    {
        // This is the Shining Dagger visual (ThrowBladeEffect: 4 blades thrown at different
        // angles), wired to the boss_throw_blade animation event.
        SpawnEffectFacingDirection(bladesEffect, bladesEffectPoint.position);
    }

    public void PlayFightSong()
    {
        audioSource.PlayOneShot(fightSong);
    }

    public void PlayRumbleSong()
    {
        audioSource.PlayOneShot(rumbleSound);
    }

    public void PlayStompSound()
    {
        audioSource.PlayOneShot(stompSound);
    }

    public void PlayBreakSound()
    {
        audioSource.PlayOneShot(breakSound);
    }

    public void PlayGroundHitSound()
    {
        audioSource.PlayOneShot(groundHitSong);
    }

    public void PlaySlash1Sound()
    {
        audioSource.PlayOneShot(slash1);
        ActivateMeleeHitboxAt(bladesEffectPoint);
    }

    public void PlaySlash2Sound()
    {
        audioSource.PlayOneShot(slash2);
        ActivateMeleeHitboxAt(bladesEffectPoint);
    }

    public void PlaySlash3Sound()
    {
        audioSource.PlayOneShot(slash3);
        ActivateMeleeHitboxAt(bladesEffectPoint);
    }

    private void ActivateMeleeHitboxAt(Transform point)
    {
        if (meleeHitbox == null) return;
        meleeHitbox.transform.position = point != null ? point.position : transform.position;
        meleeHitbox.SetDamage(attackDamage);
        meleeHitbox.Activate(0.15f);
    }
}
