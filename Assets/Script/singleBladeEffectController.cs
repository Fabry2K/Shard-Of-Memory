using System.Collections;
using UnityEngine;

public class SingleBladeEffectController : MonoBehaviour
{
    [SerializeField] private GameObject bladePrefab1;
    [SerializeField] private GameObject bladePrefab2;

    [SerializeField] private float spawnGap = 0.2f;
    [SerializeField] private float destroyAfter = 2f;

    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    [SerializeField] private float rotationOffsetZ = 0f;

    private GameObject blade1;
    private GameObject blade2;

    private void Start()
    {
        StartCoroutine(SpawnBlades());
    }

    private IEnumerator SpawnBlades()
    {
        // Rotazione della prima spada
        Quaternion rotation1 = spawnPoint1.rotation * Quaternion.Euler(0f, 0f, rotationOffsetZ);

        // Spawn della prima spada
        blade1 = Instantiate(
            bladePrefab1,
            spawnPoint1.position,
            rotation1
        );

        // Aspetta il gap
        yield return new WaitForSeconds(spawnGap);

        // Rotazione della seconda spada
        Quaternion rotation2 = spawnPoint2.rotation * Quaternion.Euler(0f, 0f, rotationOffsetZ);

        // Spawn della seconda spada
        blade2 = Instantiate(
            bladePrefab2,
            spawnPoint2.position,
            rotation2
        );

        // Aspetta prima di distruggere
        yield return new WaitForSeconds(destroyAfter);

        if (blade1 != null)
            Destroy(blade1);

        if (blade2 != null)
            Destroy(blade2);

        Destroy(gameObject);
    }
}