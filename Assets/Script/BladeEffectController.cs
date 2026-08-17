using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeEffectController : MonoBehaviour
{
    [SerializeField] private float spawnGap = 0.2f;

    private List<GameObject> blades = new List<GameObject>();

    private void Start()
    {
        // Prende tutti i figli del controller
        foreach (Transform child in transform)
        {
            blades.Add(child.gameObject);
            child.gameObject.SetActive(false);
        }

        StartCoroutine(SpawnBlades());
    }

    private IEnumerator SpawnBlades()
    {
        // Attiva i figli in ordine
        foreach (GameObject blade in blades)
        {
            if (blade != null)
                blade.SetActive(true);

            yield return new WaitForSeconds(spawnGap);
        }

        // Aspetta che tutte le Blade si siano autodistrutte
        yield return new WaitUntil(AllBladesDestroyed);

        // Distrugge il padre
        Destroy(gameObject);
    }

    private bool AllBladesDestroyed()
    {
        foreach (GameObject blade in blades)
        {
            if (blade != null)
                return false;
        }

        return true;
    }
}