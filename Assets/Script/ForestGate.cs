using UnityEngine;

public class ForestGate : MonoBehaviour
{
    [SerializeField] private AudioClip openSound;

    private Animator anim;
    private AudioSource audioSource;
    private PolygonCollider2D blockingCollider;
    private bool hasOpened;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        blockingCollider = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.forestGateOpened)
        {
            OpenInstantly();
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (hasOpened) return;
        if (!_other.CompareTag("Player")) return;
        if (GameManager.Instance == null || !GameManager.Instance.hasKey) return;

        hasOpened = true;

        if (blockingCollider != null) blockingCollider.enabled = false;
        if (anim != null) anim.SetTrigger("Opened");
        if (audioSource != null && openSound != null) audioSource.PlayOneShot(openSound);

        if (GameManager.Instance != null) GameManager.Instance.forestGateOpened = true;
    }

    private void OpenInstantly()
    {
        hasOpened = true;
        if (blockingCollider != null) blockingCollider.enabled = false;
        if (anim != null) anim.Play("Gate", 0, 1f);
    }
}
