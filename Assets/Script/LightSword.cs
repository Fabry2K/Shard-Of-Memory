using System.Collections;
using UnityEngine;

public class LightSword : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackDelay = 1f;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {

    }


    // Chiamata tramite Animation Event
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}