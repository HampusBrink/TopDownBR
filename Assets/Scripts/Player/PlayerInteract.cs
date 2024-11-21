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

    private void OnEnable()
    {
        interact.action.started += OnInteract;
    }

    private void OnDisable()
    {
        interact.action.started -= OnInteract;
    }
    
    private void Update()
    {
        if (_objectsWithinRange.Count == 0) 
            return;

        CullObjects();
        GameObject closestInteractable = GetClosestInteractable();
        if (!closestInteractable)
            return;
        if (closestInteractable.TryGetComponent(out IInteractable interactable))
        {
            if (_currentInteractable != null)
            {
                if (_currentInteractable != interactable)
                    _currentInteractable.CancelHighlight();
            }
            _currentInteractable = interactable;
            _currentInteractable.Highlight();
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        _currentInteractable?.Interact(player);
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
