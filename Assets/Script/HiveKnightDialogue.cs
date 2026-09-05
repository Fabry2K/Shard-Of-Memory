using UnityEngine;
using System.Collections;
using TMPro;

public class HiveKnightDialogue : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Earthquake Effect")]
    [SerializeField] private AudioClip screamClip;
    [SerializeField] private GameObject debrisPrefab;
    [SerializeField] private float shakeMagnitude = 0.15f;
    [SerializeField] private int debrisCount = 10;

    private AudioSource audioSource;
    private CameraFollow cameraFollow;
    private bool hasTriggered;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        // Already talked to in a previous visit to this scene: this NPC no longer belongs here.
        if (GameManager.Instance != null && GameManager.Instance.hiveKnightDialogueDone)
        {
            Destroy(gameObject);
            return;
        }

        if (dialogueBox != null) dialogueBox.SetActive(false);

        Camera cam = Camera.main;
        if (cam != null) cameraFollow = cam.GetComponent<CameraFollow>();
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (hasTriggered) return;
        if (!_collision.CompareTag("Player")) return;

        hasTriggered = true;
        StartCoroutine(PlayDialogue());
    }

    private IEnumerator PlayDialogue()
    {
        PlayerController.Instance.pState.cutScene = true;
        PlayerController.Instance.ResetInputs();

        dialogueBox.SetActive(true);

        yield return ShowLine("OH... another warrior. What world do you come from?");

        dialogueText.text = "";
        yield return new WaitForSeconds(0.5f);

        yield return ShowLine("You don't talk much, do you?");
        yield return ShowLine("Well, you're probably still recovering your memory. It happens to everyone here: one moment you're fighting for your life, and then suddenly... poof... you wake up in this stinking place.");

        dialogueBox.SetActive(false);

        float effectDuration = screamClip != null ? screamClip.length : 3f;

        if (audioSource != null && screamClip != null)
        {
            audioSource.PlayOneShot(screamClip);
        }
        if (cameraFollow != null)
        {
            cameraFollow.Shake(effectDuration, shakeMagnitude);
        }
        SpawnDebris();

        yield return new WaitForSeconds(effectDuration);

        dialogueBox.SetActive(true);
        yield return ShowLine("Indeed, we are not the only ones here. A catastrophe I've already witnessed with my own eyes is about to happen again.");

        dialogueBox.SetActive(false);

        PlayerController.Instance.pState.cutScene = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.hiveKnightDialogueDone = true;
        }
    }

    private IEnumerator ShowLine(string line)
    {
        dialogueText.text = line;
        yield return null;
        yield return new WaitUntil(() => Input.GetButtonDown("Interact"));
    }

    private void SpawnDebris()
    {
        if (debrisPrefab == null) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;

        for (int i = 0; i < debrisCount; i++)
        {
            float x = camPos.x + Random.Range(-halfWidth, halfWidth);
            float y = camPos.y + halfHeight + Random.Range(0.5f, 3f);
            Vector3 spawnPos = new Vector3(x, y, 0f);

            GameObject debris = Instantiate(debrisPrefab, spawnPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            float scale = Random.Range(0.6f, 1.6f);
            debris.transform.localScale = Vector3.one * scale;

            DebrisPiece piece = debris.GetComponent<DebrisPiece>();
            float fallSpeed = Random.Range(4f, 8f);
            float rotSpeed = Random.Range(-180f, 180f);
            float lifeTime = (halfHeight * 2f + 4f) / fallSpeed + 1f;
            piece.Init(fallSpeed, rotSpeed, lifeTime);
        }
    }
}
