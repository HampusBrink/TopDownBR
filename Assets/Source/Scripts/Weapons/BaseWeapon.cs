using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using MultiplayerBase.Scripts;
using UnityEngine;

public class BaseWeapon : NetworkBehaviour
{

    public BoxCollider2D weaponCol;
    
    [Header("Stats")]
    public float baseDamage = 10f;
    public float baseAttackSpeed = 2f;
    public float baseWeaponLength = 1.4f;

    private float _multipliedDamage;
    private float _multipliedWeaponLength;

    private bool _isAlreadyAttacking;

    private void Start()
    {
        _multipliedDamage = baseDamage;
        _multipliedWeaponLength = baseWeaponLength;
    }

    public void WeaponPerformAttack(float attackDuration)
    {
        if(_isAlreadyAttacking) return;
        StartCoroutine(ActivateAttack(attackDuration));
    }

    public void UpdateAttackDamage(float multiplier)
    {
        _multipliedDamage = baseDamage * multiplier;
    }

    public void UpdateWeaponLength(float multiplier)
    {
        _multipliedWeaponLength = baseWeaponLength * multiplier;
        weaponCol.transform.localScale = new Vector3(weaponCol.transform.localScale.x, _multipliedWeaponLength, weaponCol.transform.localScale.z);
    }
    
    private IEnumerator ActivateAttack(float attackDuration)
    {
        _isAlreadyAttacking = true;
        weaponCol.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        // ReSharper disable once Unity.InefficientPropertyAccess
        weaponCol.enabled = false;
        _isAlreadyAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!GameManager.Instance.GameStarted) return;
        if (col.TryGetComponent(out NetworkObject no))
        {
            if (no.TryGetComponent(out IDamagable damagable))
            {
                if(!no.IsOwner) return;
                damagable.TakeDamage(_multipliedDamage);
                weaponCol.enabled = false;
            }
        }
    }
}
