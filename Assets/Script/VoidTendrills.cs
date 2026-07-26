using System.Collections;
using UnityEngine;

public class VoidTendrills : MonoBehaviour
{
    [Header("Children")]
    [SerializeField] private GameObject[] children = new GameObject[5];

    [Header("Delay")]
    [SerializeField] private float lastChildrenDelay = 2f;

    private void Awake()
    {
        // Disattiva tutti i figli all'inizio
        foreach (GameObject child in children)
        {
            if (child != null)
                child.SetActive(false);
        }
    }

    private void Start()
    {
        // Attiva subito i primi tre
        for (int i = 0; i < 3 && i < children.Length; i++)
        {
            if (children[i] != null)
                children[i].SetActive(true);
        }

        // Attiva gli ultimi due dopo il ritardo
        StartCoroutine(ActivateLastChildren());

        // Distrugge il padre quando il primo figlio viene distrutto
        StartCoroutine(DestroyParentWhenFirstChildIsDestroyed());
    }

    private IEnumerator ActivateLastChildren()
    {
        yield return new WaitForSeconds(lastChildrenDelay);

        for (int i = 3; i < 5 && i < children.Length; i++)
        {
            if (children[i] != null)
                children[i].SetActive(true);
        }
    }

    private IEnumerator DestroyParentWhenFirstChildIsDestroyed()
    {
        yield return new WaitUntil(() => children[0] == null);

        Destroy(gameObject);
    }
}