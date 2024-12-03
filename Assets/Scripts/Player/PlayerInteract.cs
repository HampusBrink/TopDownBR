using System;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private PlayerStatus player;
    private IInteractable _currentInteractable = null;
    private List<GameObject> _objectsWithinRange = new List<GameObject>();
    public InputActionReference interact;
    
    private float _currentInteractTime = 0f;
    private bool _wasHolding = false;
    private void Update()
    {
        bool isSame = false;
        bool isHolding = interact.action.ReadValue<float>() != 0f;
        
        CullObjects();
        GameObject closestInteractable = GetClosestInteractable();
        if (!closestInteractable)
        {
            _currentInteractable?.CancelHighlight();
        }
        else if (closestInteractable.TryGetComponent(out IInteractable interactable))
        {
            isSame = !(interactable == null || _currentInteractable == null); //
            if (isSame)
            {
                isSame = _currentInteractable.Equals(interactable);
                interactable?.Highlight();
            }

            if (!isSame)
            {
                _currentInteractable?.CancelHighlight();
                _currentInteractable = interactable;
                interactable?.Highlight();
            }
                
        }

        bool justPressed = isHolding && !_wasHolding;

        if (justPressed || _currentInteractTime > 0 && isHolding && isSame)
        {
            _currentInteractTime += Time.deltaTime;
            _currentInteractable?.Interact(player, _currentInteractTime);
        }
        else
        {
            _currentInteractTime = 0;
            
        }
        
        _wasHolding = isHolding;
    }
    
    private GameObject GetClosestInteractable()
    {
        if (_objectsWithinRange == null || _objectsWithinRange.Count <= 0)
            return null;
        if (_objectsWithinRange.Count == 1)
            return _objectsWithinRange[0];
        float minDistance = Vector3.Distance(transform.position, _objectsWithinRange[0].transform.position);
        GameObject closestInteractable = _objectsWithinRange[0];
        foreach (GameObject interactable in _objectsWithinRange)
        {
            float distance = Vector3.Distance(transform.position, interactable.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestInteractable = interactable;
            }
        }
        return closestInteractable;
    }

    private void CullObjects()
    {
        for (int i = 0; i < _objectsWithinRange.Count; i++)
        {
            if (_objectsWithinRange[i] == null)
            {
                _objectsWithinRange.RemoveAt(i);
                i--;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_objectsWithinRange.Contains(other.gameObject))
            _objectsWithinRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_objectsWithinRange.Contains(other.gameObject))
            _objectsWithinRange.Remove(other.gameObject);
    }
}
