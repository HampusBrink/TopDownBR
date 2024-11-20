using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    private List<GameObject> objectsWithinRange;
    public InputActionReference interact;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    private GameObject GetClosestInteractable()
    {
        if (objectsWithinRange == null || objectsWithinRange.Count <= 0)
            return null;
        if (objectsWithinRange.Count == 1)
            return objectsWithinRange[0];
        float minDistance = Vector3.Distance(transform.position, objectsWithinRange[0].transform.position);
        GameObject closestInteractable = objectsWithinRange[0];
        foreach (GameObject interactable in objectsWithinRange)
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

    private void OnTriggerEnter(Collider other)
    {
        if (!objectsWithinRange.Contains(other.gameObject))
            objectsWithinRange.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectsWithinRange.Contains(other.gameObject))
            objectsWithinRange.Remove(other.gameObject);
    }
}
