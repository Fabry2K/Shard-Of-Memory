using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private string tutorialId;
    [SerializeField] private GameObject promptUI;
    private bool hasShown;

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.HasShownTutorial(tutorialId))
        {
            hasShown = true;
        }

        if (promptUI != null) promptUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (hasShown) return;
        if (!_collision.CompareTag("Player")) return;

        hasShown = true;
        if (promptUI != null) promptUI.SetActive(true);

        if (GameManager.Instance != null) GameManager.Instance.MarkTutorialShown(tutorialId);
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        if (!_collision.CompareTag("Player")) return;
        if (promptUI != null) promptUI.SetActive(false);
    }
}
