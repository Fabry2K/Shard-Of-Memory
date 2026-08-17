using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallRingSeriesController : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject childPrefab;
    [SerializeField] private int childrenCount = 5;
    [SerializeField] private float spawnInterval = 0.5f;

    [Header("Spawn Area")]
    [SerializeField] private float xRange = 5f;
    [SerializeField] private float yRange = 3f;

    [Header("Minimum Distance")]
    [SerializeField] private float minDistance = 1f;

    [Header("Generation")]
    [SerializeField] private int maxAttempts = 100;

    private readonly List<GameObject> spawnedChildren = new();
    private readonly List<Vector3> usedPositions = new();

    private void Start()
    {
        StartCoroutine(SpawnChildren());
    }

    private IEnumerator SpawnChildren()
    {
        for (int i = 0; i < childrenCount; i++)
        {
            Vector3 spawnPosition = GetRandomPosition();

            GameObject child = Instantiate(
                childPrefab,
                spawnPosition,
                Quaternion.identity,
                transform);

            spawnedChildren.Add(child);

            yield return new WaitForSeconds(spawnInterval);
        }

        StartCoroutine(WaitForChildrenToBeDestroyed());
    }

    private Vector3 GetRandomPosition()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 candidate = transform.position + new Vector3(
                Random.Range(-xRange, xRange),
                Random.Range(-yRange, yRange),
                0f);

            bool valid = true;

            foreach (Vector3 pos in usedPositions)
            {
                if (Vector3.Distance(candidate, pos) < minDistance)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
            {
                usedPositions.Add(candidate);
                return candidate;
            }
        }

        Debug.LogWarning("Impossibile trovare una posizione valida. Aumento dell'area consigliato.");
        return transform.position;
    }

    private IEnumerator WaitForChildrenToBeDestroyed()
    {
        while (true)
        {
            spawnedChildren.RemoveAll(child => child == null);

            if (spawnedChildren.Count == 0)
            {
                Destroy(gameObject);
                yield break;
            }

            yield return null;
        }
    }
}