using UnityEngine;

public class BossController : MonoBehaviour
{

    private Animator anim;
    private bool hasSpawned = false;

    [Header("Effects")]
    [SerializeField] private DetectionBox spawnDetectionBox;

    [SerializeField] private GameObject whiteSpawnEffect;
    [SerializeField] private Transform whiteSpawnEffectPoint;

    [SerializeField] private GameObject blackSpawnEffect;
    [SerializeField] private Transform blackSpawnEffectPoint;

    [SerializeField] private GameObject circleSpawnEffect;
    [SerializeField] private Transform circleSpawnEffectPoint;

    [SerializeField] private GameObject linesSpawnEffect;
    [SerializeField] private Transform linesSpawnEffectPoint;

    [Header("Sounds")]
    private AudioSource audioSource;

    [SerializeField] private AudioClip stompSound;
    [SerializeField] private AudioClip breakSound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    private void Update()
    {
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


    // Funzioni richiamabili dagli Animation Event

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

    public void PlayStompSound()
    {
        audioSource.PlayOneShot(stompSound);
    }

    public void PlayBreakSound()
    {
        audioSource.PlayOneShot(breakSound);
    }


}
