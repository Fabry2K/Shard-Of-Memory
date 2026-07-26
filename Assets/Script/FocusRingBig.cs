using UnityEngine;

public class FocusRingBig : MonoBehaviour
{
    [SerializeField] private GameObject effect1;
    [SerializeField] private GameObject effect2;
    [SerializeField] private GameObject effect3;

    private GameObject effect1Instance;
    private GameObject effect2Instance;
    private GameObject effect3Instance;

    public void SpawnEffect1()
    {
        if (effect1Instance == null)
            effect1Instance = Instantiate(effect1, transform.position, Quaternion.identity);
    }

    public void SpawnEffect2()
    {
        if (effect2Instance == null)
            effect2Instance = Instantiate(effect2, transform.position, Quaternion.identity);
    }

    public void SpawnEffect3()
    {
        if (effect3Instance == null)
            effect3Instance = Instantiate(effect3, transform.position, Quaternion.identity);
    }

    // Richiamabile tramite Animation Event
    public void DestroyAll()
    {
        if (effect1Instance != null)
            Destroy(effect1Instance);

        if (effect2Instance != null)
            Destroy(effect2Instance);

        if (effect3Instance != null)
            Destroy(effect3Instance);

        Destroy(gameObject);
    }
}