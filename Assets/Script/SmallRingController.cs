using UnityEngine;

public class SmallRingController : MonoBehaviour
{
    [SerializeField] private GameObject effect1;
    [SerializeField] private GameObject effect2;

    [Header("Sounds")]
    [SerializeField] private AudioClip appearSound;
    [SerializeField] private AudioClip explodeSound;

    private GameObject effect1Instance;
    private GameObject effect2Instance;

    // Through the AudioManager rather than a local AudioSource: the ring destroys itself, which
    // would cut its own sound off, and this keeps it on the Master mixer group so the volume
    // slider still governs it.
    private void Play(AudioClip clip)
    {
        if (clip != null) AudioManager.PlayClipAtPoint(clip, transform.position);
    }

    public void SpawnEffect1()
    {
        if (effect1Instance == null)
            effect1Instance = Instantiate(effect1, transform.position, Quaternion.identity);

        Play(appearSound);
    }

    public void SpawnEffect2()
    {
        // effect2 (Intern Small Focus Ring) is the burst that hurts, and it carries its own
        // hitbox - so the damage window lines up with its lifetime for free.
        if (effect2Instance == null)
            effect2Instance = Instantiate(effect2, transform.position, Quaternion.identity);

        Play(explodeSound);
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
