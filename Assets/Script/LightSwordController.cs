using System.Collections;
using UnityEngine;

public class LightSwordController : MonoBehaviour
{
    [Header("Swords")]
    [SerializeField] private GameObject[] swords;


    [Header("Sounds")]
    private AudioSource audioSource;

    [SerializeField] private AudioClip swordsSpawn;
    [SerializeField] private AudioClip swordsAttack;

    [Header("Timing")]
    [SerializeField] private float spawnInterval = 0.15f;
    [SerializeField] private float attackDelay = 1f;

    private IEnumerator Start()
    {
        // Disattiva tutte le spade all'inizio
        foreach (GameObject sword in swords)
        {
            sword.SetActive(false);
            audioSource = GetComponent<AudioSource>();
        }

        // Spada 1
        swords[0].SetActive(true);
        yield return new WaitForSeconds(spawnInterval);
        audioSource.PlayOneShot(swordsSpawn, 5f);
        // Spade 2 e 3
        swords[1].SetActive(true);
        swords[2].SetActive(true);
        yield return new WaitForSeconds(spawnInterval);
        audioSource.PlayOneShot(swordsSpawn, 5f);
        // Spade 4 e 5
        swords[3].SetActive(true);
        swords[4].SetActive(true);
        yield return new WaitForSeconds(spawnInterval);
        audioSource.PlayOneShot(swordsSpawn, 5f);
        // Spade 6 e 7
        swords[5].SetActive(true);
        swords[6].SetActive(true);
        yield return new WaitForSeconds(spawnInterval);
        audioSource.PlayOneShot(swordsSpawn, 5f);
        // Spade 8 e 9
        swords[7].SetActive(true);
        swords[8].SetActive(true);
        audioSource.PlayOneShot(swordsSpawn, 5f);

        // Aspetta prima di far partire l'attacco
        yield return new WaitForSeconds(attackDelay);
        audioSource.PlayOneShot(swordsAttack, 5f);
        // Fa partire contemporaneamente l'attacco di tutte
        foreach (GameObject sword in swords)
        {
            Animator anim = sword.GetComponent<Animator>();

            if (anim != null)
                anim.SetTrigger("Start");
        }
    }
}