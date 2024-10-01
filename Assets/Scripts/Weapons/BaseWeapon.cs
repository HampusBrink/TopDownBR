using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseWeapon : MonoBehaviourPunCallbacks
{

    public BoxCollider2D weaponCol;
    public SpriteRenderer weaponGFX;
    
    [Header("Stats")]
    public float baseDamage = 10f;
    public float baseAttackSpeed = 2f;
    public float baseWeaponLength = 1.4f;

    private float _multipliedDamage;
    private float _multipliedWeaponLength;

    [HideInInspector] public bool isAlreadyAttacking;

    private void Start()
    {
        _multipliedDamage = baseDamage;
        _multipliedWeaponLength = baseWeaponLength;
    }

    public void WeaponPerformAttack(float attackDuration)
    {
        if(isAlreadyAttacking) return;
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
        isAlreadyAttacking = true;
        weaponCol.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        // ReSharper disable once Unity.InefficientPropertyAccess
        weaponCol.enabled = false;
        isAlreadyAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(!GameManager.Instance.GameStarted) return;
        if (col.TryGetComponent(out PhotonView pv))
        {
            if (pv.TryGetComponent(out IDamagable damagable))
            {
                if(!photonView.IsMine) return;
                pv.RPC(nameof(damagable.RPC_TakeDamage),RpcTarget.All,pv.ViewID,_multipliedDamage);
                weaponCol.enabled = false;
            }
        }
    }
}
