using Player;
using UnityEngine;

public interface IInteractable
{
    public void Highlight();
    public void CancelHighlight();
    public void Interact(PlayerStatus interactingPlayer);
}
