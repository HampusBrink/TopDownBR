using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using MultiplayerBase.Scripts;

public class Arrow : NetworkBehaviour
{
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Rigidbody rb;
    
    public Color _startColor = Color.white; // Start color (white)
    public Color _endColor = Color.black;
    

    private float _damage;
    private float _range;
    
    private float _elapsedLifeTime;

    private void Start()
    {
        //Destroy(gameObject, _range);
    }
    
    private void Update()
    {
        // Track the elapsed time and normalize it based on the arrow's range
        _elapsedLifeTime += Time.deltaTime;
        float t = _elapsedLifeTime / _range; // Calculate how far we are through the arrow's range

        // Lerp between white and black based on how much time has passed
        //spriteRenderer.color = Color.Lerp(_startColor, _endColor, t * t);
        
        AdjustArrowRot();
    }
    
    public void SetArrowStats(float damage, float range)
    {
        _damage = damage;
        _range = range;
    }

    private void AdjustArrowRot()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            // Make the arrow point in the direction of its velocity
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity);

            // Apply the rotation offset
            transform.rotation = targetRotation * Quaternion.Euler(new Vector3(270f, 0f, 0f));
        }
    }
    
    private void StickToObject(Collider col)
    {
        // Stop the arrow's movement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Attach the arrow to the object it collided with
        //transform.parent = col.transform;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        //if (!GameManager.Instance.GameStarted) return;

        if (col.gameObject.layer is 7) // Assuming PlayerHitbox is layer 7
        {
            if (col.transform.parent.TryGetComponent(out NetworkObject no))
            {
                if(no.IsOwner || IsOffline)
                    return;
                if (no.TryGetComponent(out IDamagable damagable))
                {
                    

                    damagable.TakeDamage(_damage);
                }
            }
            Debug.Log("Collided with: " + col.gameObject);
            Destroy(gameObject);
        }
        else if (col.gameObject.layer is 8) // Assuming Damager is layer 8
        {
            return;
        }
        else 
        {
            StickToObject(col);
        }
    }
}
