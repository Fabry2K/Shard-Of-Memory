using UnityEngine;

public class SmallRingController : MonoBehaviour
{
    [SerializeField] private GameObject effect1;
    [SerializeField] private GameObject effect2;

    private GameObject effect1Instance;
    private GameObject effect2Instance;

    public void SpawnEffect1()
    {
        if (effect1Instance == null)
            effect1Instance = Instantiate(effect1, transform.position, Quaternion.identity);
    }

    public void SpawnEffect2()
    {
        if (effect2Instance == null)
            effect2Instance = Instantiate(effect2, transform.position, Quaternion.identity);

        // The ring only becomes dangerous once it "explodes" (effect2), not on the initial spawn.
        var hitbox = GetComponent<BossAttackHitbox>();
        if (hitbox != null) hitbox.Activate(0.5f);
    }


    // Richiamabile tramite Animation Event
    public void DestroyAll()
    {
        if (effect1Instance != null)
            Destroy(effect1Instance);

        if (effect2Instance != null)
            Destroy(effect2Instance);

        Destroy(gameObject);
    }
}