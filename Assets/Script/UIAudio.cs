using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class UIAudio : MonoBehaviour
{
    [SerializeField] AudioClip hover, click;
    AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SoundOnHover()
    {
        audioSource.PlayOneShot(hover);
    }

    public void SoundOnClick()
    {
        audioSource.PlayOneShot(click);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
