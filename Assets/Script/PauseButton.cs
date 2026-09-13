using UnityEngine;
using UnityEngine.EventSystems;

// The on-screen way into the pause menu, for touchscreens that have no Escape key.
//
// It reaches the manager through GameManager.Instance rather than a serialized reference: this
// button lives on the persistent Canvas prefab, which outlives every scene, so a reference wired
// in the prefab would point at whichever GameManager happened to exist when it was set.
public class PauseButton : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance != null) GameManager.Instance.PauseGame();
    }
}
