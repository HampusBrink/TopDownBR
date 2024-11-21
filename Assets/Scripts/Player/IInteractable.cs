using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public void Highlight();
    public void CancelHighlight();
    public void Interact(PlayerStatus interactingPlayer, float time = 0f);
}
