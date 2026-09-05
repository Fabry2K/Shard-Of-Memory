using UnityEngine;

public class Bench : MonoBehaviour
{
    public bool inRange;
    public bool interacted;

    [SerializeField] private GameObject interactPrompt;
    [SerializeField] private GameObject restEffectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inRange && Input.GetButtonDown("Interact"))
        {
            interacted = true;

            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.Health = PlayerController.Instance.maxHealth;
            }

            if (restEffectPrefab != null && PlayerController.Instance != null)
            {
                Instantiate(restEffectPrefab, PlayerController.Instance.transform.position, Quaternion.identity);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            inRange = true;
            if (interactPrompt != null) interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            inRange = false;
            //interacted = false;
            if (interactPrompt != null) interactPrompt.SetActive(false);
        }
    }
}
