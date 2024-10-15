using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using MultiplayerBase.Scripts;

public class Arrow : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D collider;
    
    private float _damage;

    private void Start()
    {
        StartCoroutine(EnableColliderAfterDelay(0.05f));
    }
    
    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay
        collider.enabled = true; // Enable the collider after the delay
    }

    public void SetDamage(float damageFromBow)
    {
       _damage = damageFromBow;
    }
    
    
    private void OnTriggerEnter2D(Collider2D col)
    {
        //if (!GameManager.Instance.GameStarted) return;

        if (col.gameObject.layer is 7 or 20 or 21) // Assuming PlayerHitbox is layer 7
        {
            if (col.TryGetComponent(out NetworkObject no))
            {
                if (no.TryGetComponent(out IDamagable damagable))
                {
                    //if (!GetComponentInParent<NetworkObject>().IsOwner) return;

                    damagable.TakeDamage(_damage);
                }
            }
            Destroy(gameObject);
        }
    }
}
