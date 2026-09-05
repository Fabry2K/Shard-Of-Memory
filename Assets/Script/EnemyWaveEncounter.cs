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

    private AudioSource audioSource;
    private bool hasTriggered;
    private readonly List<GameObject> currentWaveEnemies = new List<GameObject>();

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (hasTriggered) return;
        if (!_collision.CompareTag("Player")) return;

        hasTriggered = true;
        StartCoroutine(RunEncounter());
    }

    private IEnumerator RunEncounter()
    {
        yield return StartCoroutine(MovePillars(pillarRiseHeight));
        if (audioSource != null && gateSound != null) audioSource.PlayOneShot(gateSound);

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

        yield return StartCoroutine(MovePillars(-pillarRiseHeight));
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

    private IEnumerator MovePillars(float deltaY)
    {
        Vector3 start1 = pillar1.position;
        Vector3 start2 = pillar2.position;
        Vector3 end1 = start1 + new Vector3(0f, deltaY, 0f);
        Vector3 end2 = start2 + new Vector3(0f, deltaY, 0f);

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
