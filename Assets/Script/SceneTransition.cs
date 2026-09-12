using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{

    [SerializeField] private string transitionTo;
    [SerializeField] Transform startPoint;
    [SerializeField] private Vector2 exitDirection;
    [SerializeField] private float exitTime;

    private void Start()
    {
        if(transitionTo == GameManager.Instance.transitionedFromScene)
        {
            PlayerController.Instance.transform.position = startPoint.position;

            // Without this the fallback respawn point keeps pointing at coordinates from the
            // previous scene, which is why dying in a room with no respawn points of its own
            // (the boss room) dropped the player at an arbitrary spot.
            GameManager.Instance.platformingRespawnPoint = startPoint.position;

            StartCoroutine(PlayerController.Instance.WalkIntoNewScene(exitDirection, exitTime));
        }
        StartCoroutine(FadeUpWhenSettled());
    }

    // The first couple of frames of a fresh scene carry all the Awake/Start work and the shader
    // warm-up, so they are the janky ones. Letting them go by while the screen is still black
    // means the fade up starts on a scene that is already running smoothly.
    private IEnumerator FadeUpWhenSettled()
    {
        yield return null;
        yield return null;

        UIManager.Instance.sceneFader.FadeOut();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            PlayerController.Instance.pState.cutScene = true;
            PlayerController.Instance.pState.invincible = true;

            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transitionTo));
        }
    }
}
