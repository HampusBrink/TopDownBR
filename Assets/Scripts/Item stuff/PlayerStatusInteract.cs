using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStatusInteract : MonoBehaviour, IInteractable
{
    public float interactTime = 0.2f;
    public UnityEvent<PlayerStatus> onInteract;
    public UnityEvent onHighlight;
    public UnityEvent onCancelHighlight;

    public void Highlight()
    {
        onHighlight.Invoke();
    }

    public void CancelHighlight()
    {
        onCancelHighlight.Invoke();
    }

    public void Interact(PlayerStatus interactingPlayer, float time = 0)
    {
        if (time >= interactTime)
            onInteract.Invoke(interactingPlayer);
    }
}
