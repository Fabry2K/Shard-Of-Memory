using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{

    private Animator anim;
    private bool hasSpawned = false;
    private int facingDirection = 1;

    [Header("Attacks")]
    [SerializeField] private DetectionBox jumpAttackDetectionBox;

    [Header("Effects")]
    [SerializeField] private DetectionBox spawnDetectionBox;

    [SerializeField] private GameObject breakEffect;
    [SerializeField] private Transform breakEffectPoint;

    [SerializeField] private GameObject whiteSpawnEffect;
    [SerializeField] private Transform whiteSpawnEffectPoint;

    [SerializeField] private GameObject blackSpawnEffect;
    [SerializeField] private Transform blackSpawnEffectPoint;

    [SerializeField] private GameObject circleSpawnEffect;
    [SerializeField] private Transform circleSpawnEffectPoint;

    [SerializeField] private GameObject linesSpawnEffect;
    [SerializeField] private Transform linesSpawnEffectPoint;

    [SerializeField] private GameObject lightSwordEffect;
    [SerializeField] private Transform lightSwordEffectPoint;

    [SerializeField] private GameObject groundHitEffect;
    [SerializeField] private Transform groundHitEffectPoint;



    [Header("Sounds")]
    private AudioSource audioSource;

    [SerializeField] private AudioClip fightSong;
    [SerializeField] private AudioClip rumbleSound;
    [SerializeField] private AudioClip stompSound;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private AudioClip groundHitSong;
    [SerializeField] private AudioClip slash1;
    [SerializeField] private AudioClip slash2;
    [SerializeField] private AudioClip slash3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {

        if (hasSpawned)
        {
            TryJumpAttack();
        }


        if (!hasSpawned && PlayerDetected())
        {
            StartSpawn();
        }


    }


    private bool PlayerDetected()
    {
        foreach (Collider2D col in spawnDetectionBox.detectedColliders)
        {
            if (col.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void StartSpawn()
    {
        hasSpawned = true;
        anim.SetTrigger("Spawn");
    }

    public void TryJumpAttack()
    {
        bool find = false;
        foreach (Collider2D col in jumpAttackDetectionBox.detectedColliders)
        {
            if (col.CompareTag("Player"))
            {
                anim.SetBool("JumpAttack", true);
                find = true;
            }
        }

        if (!find) anim.SetBool("JumpAttack", false);
    }

    public void StartJumpAnimation()
    {
        StartCoroutine(BossAttackMovement());
    }

    private IEnumerator BossAttackMovement()
    {
        // salta
        yield return StartCoroutine(Move(new Vector3(5.5f * facingDirection, 6.5f, 0f), 0.4f));

        // resta fermo
        yield return StartCoroutine(FreezeInAir(0.3f));

        // torna giù
        yield return StartCoroutine(Move(new Vector3(2.5f * facingDirection, -7.5f, 0f), 0.25f));

    }

    private IEnumerator Move(Vector3 offset, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + offset;

        float timer = 0f;

        while (timer < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timer/duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
    }

    private IEnumerator FreezeInAir(float duration)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        float oldGravity = rb.gravityScale;

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(duration);

        rb.gravityScale = oldGravity;
    }


    // Funzioni richiamabili dagli Animation Event

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

    public void SpawnLightSwordEffect()
    {
        Instantiate(lightSwordEffect, lightSwordEffectPoint.position, Quaternion.identity);
    }

    public void SpawnGroundSlashEffect()
    {
        Instantiate(groundHitEffect, groundHitEffectPoint.position, Quaternion.identity);
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
    }


    public void PlaySlash2Sound()
    {
        audioSource.PlayOneShot(slash2);
    }

    public void PlaySlash3Sound()
    {
        audioSource.PlayOneShot(slash3);
    }



}
