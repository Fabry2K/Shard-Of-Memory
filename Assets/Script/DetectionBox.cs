using UnityEngine;
using System.Collections.Generic;

public class DetectionBox : MonoBehaviour
{
    Collider2D col;
    public List<Collider2D> detectedColliders = new List<Collider2D> ();

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("ENTER: " + collision.name);
        detectedColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
    }
}