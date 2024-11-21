using Player;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatusInteract : MonoBehaviour, IInteractable
{
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

    public void Interact(PlayerStatus interactingPlayer)
    {
        onInteract.Invoke(interactingPlayer);
    }
}
