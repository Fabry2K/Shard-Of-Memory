using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Spikes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {

        PlayerController.Instance.pState.cutScene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.ResetInputs();

        PlayerController.Instance.rb.linearVelocity = Vector2.zero;

        PlayerController.Instance.TakeDamageNoStop(1);

        if (!PlayerController.Instance.pState.alive)
        {
            // Lethal hit: let the normal death sequence (triggered inside TakeDamageNoStop) take over
            // instead of teleporting the corpse back to the platforming respawn point.
            PlayerController.Instance.pState.cutScene = false;
            yield break;
        }

        yield return new WaitForSecondsRealtime(0.1f);

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));

        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        PlayerController.Instance.transform.position =
            GameManager.Instance.platformingRespawnPoint;

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));

        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        PlayerController.Instance.pState.cutScene = false;
        PlayerController.Instance.pState.invincible = false;
    }
}
