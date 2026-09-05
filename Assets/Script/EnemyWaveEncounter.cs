using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaveEncounter : MonoBehaviour
{
    [Header("Gate Pillars")]
    [SerializeField] private Transform pillar1;
    [SerializeField] private Transform pillar2;
    [SerializeField] private float pillarRiseHeight = 4f;
    [SerializeField] private float pillarMoveDuration = 1.5f;
    [SerializeField] private AudioClip gateSound;

    [Header("Spawn Area")]
    [SerializeField] private BoxCollider2D spawnZone;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject batPrefab;
    [SerializeField] private GameObject spawnIndicatorPrefab;
    [SerializeField] private float spawnLeadTime = 0.7f;
    [SerializeField] private float waveClearDelay = 2f;

    [Header("Key Reward")]
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private GameObject keySpawnEffectPrefab;

    private AudioSource audioSource;
    private bool hasTriggered;
    private bool encounterActive;
    private Coroutine mainCoroutine;
    private Vector3 pillar1BasePos;
    private Vector3 pillar2BasePos;
    private readonly List<GameObject> currentWaveEnemies = new List<GameObject>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (pillar1 != null) pillar1BasePos = pillar1.position;
        if (pillar2 != null) pillar2BasePos = pillar2.position;
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (hasTriggered) return;
        if (!_collision.CompareTag("Player")) return;

        hasTriggered = true;
        encounterActive = true;
        mainCoroutine = StartCoroutine(RunEncounter());
        StartCoroutine(WatchForPlayerDeath());
    }

    private IEnumerator RunEncounter()
    {
        yield return StartCoroutine(MovePillarsToHeight(pillarRiseHeight));

        Bounds b = spawnZone.bounds;
        float groundY = b.min.y + 0.4f;
        float flyY = b.min.y + b.size.y * 0.55f;
        float centerX = b.center.x;
        float halfW = b.extents.x;

        // Wave 1: two Skeletons, left and right of center
        yield return StartCoroutine(SpawnWave(new SpawnPoint[]
        {
            new SpawnPoint(skeletonPrefab, new Vector2(centerX - halfW * 0.4f, groundY)),
            new SpawnPoint(skeletonPrefab, new Vector2(centerX + halfW * 0.4f, groundY)),
        }));
        yield return StartCoroutine(WaitWaveClear());

        // Wave 2: two Bats, slightly elevated
        yield return StartCoroutine(SpawnWave(new SpawnPoint[]
        {
            new SpawnPoint(batPrefab, new Vector2(centerX - halfW * 0.4f, flyY)),
            new SpawnPoint(batPrefab, new Vector2(centerX + halfW * 0.4f, flyY)),
        }));
        yield return StartCoroutine(WaitWaveClear());

        // Wave 3: two Skeletons and two Bats
        yield return StartCoroutine(SpawnWave(new SpawnPoint[]
        {
            new SpawnPoint(skeletonPrefab, new Vector2(centerX - halfW * 0.6f, groundY)),
            new SpawnPoint(skeletonPrefab, new Vector2(centerX + halfW * 0.6f, groundY)),
            new SpawnPoint(batPrefab, new Vector2(centerX - halfW * 0.2f, flyY)),
            new SpawnPoint(batPrefab, new Vector2(centerX + halfW * 0.2f, flyY)),
        }));
        yield return StartCoroutine(WaitWaveClear());

        yield return StartCoroutine(SpawnKey(new Vector2(centerX, flyY)));

        yield return StartCoroutine(MovePillarsToHeight(0f));

        // Encounter completed successfully: stop watching for death, this arena is done for good.
        encounterActive = false;
    }

    // Runs alongside RunEncounter. If the player dies before the encounter finishes,
    // abort the encounter, clear any surviving enemies, reopen the gate and let the
    // trigger fire again from scratch - as if the player had never interacted with it.
    private IEnumerator WatchForPlayerDeath()
    {
        while (encounterActive)
        {
            if (PlayerController.Instance != null && !PlayerController.Instance.pState.alive)
            {
                encounterActive = false;

                if (mainCoroutine != null) StopCoroutine(mainCoroutine);

                foreach (var enemy in currentWaveEnemies)
                {
                    if (enemy != null) Destroy(enemy);
                }
                currentWaveEnemies.Clear();

                yield return StartCoroutine(MovePillarsToHeight(0f));

                hasTriggered = false;
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator SpawnKey(Vector2 position)
    {
        if (keySpawnEffectPrefab != null)
        {
            Instantiate(keySpawnEffectPrefab, position, Quaternion.identity);
        }

        yield return new WaitForSeconds(spawnLeadTime);

        if (keyPrefab != null)
        {
            Instantiate(keyPrefab, position, Quaternion.identity);
        }
    }

    private struct SpawnPoint
    {
        public GameObject prefab;
        public Vector2 position;
        public SpawnPoint(GameObject _prefab, Vector2 _position)
        {
            prefab = _prefab;
            position = _position;
        }
    }

    private IEnumerator SpawnWave(SpawnPoint[] points)
    {
        currentWaveEnemies.Clear();

        if (spawnIndicatorPrefab != null)
        {
            foreach (var p in points)
            {
                Instantiate(spawnIndicatorPrefab, p.position, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(spawnLeadTime);

        foreach (var p in points)
        {
            GameObject enemy = Instantiate(p.prefab, p.position, Quaternion.identity);
            currentWaveEnemies.Add(enemy);
        }
    }

    private IEnumerator WaitWaveClear()
    {
        yield return new WaitUntil(() => currentWaveEnemies.TrueForAll(e => e == null));
        yield return new WaitForSeconds(waveClearDelay);
    }

    // heightOffset is relative to each pillar's original resting position (0 = fully lowered/open,
    // pillarRiseHeight = fully raised/closed). Using an absolute target instead of a delta from the
    // pillar's current position means this is safe to call even if a previous move was interrupted partway.
    private IEnumerator MovePillarsToHeight(float heightOffset)
    {
        if (audioSource != null && gateSound != null) audioSource.PlayOneShot(gateSound);

        Vector3 start1 = pillar1.position;
        Vector3 start2 = pillar2.position;
        Vector3 end1 = pillar1BasePos + new Vector3(0f, heightOffset, 0f);
        Vector3 end2 = pillar2BasePos + new Vector3(0f, heightOffset, 0f);

        float t = 0f;
        while (t < pillarMoveDuration)
        {
            t += Time.deltaTime;
            float f = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / pillarMoveDuration));
            pillar1.position = Vector3.Lerp(start1, end1, f);
            pillar2.position = Vector3.Lerp(start2, end2, f);
            yield return null;
        }

        pillar1.position = end1;
        pillar2.position = end2;
    }
}
