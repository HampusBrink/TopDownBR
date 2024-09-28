using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BaseWeapon : MonoBehaviourPunCallbacks
{

    public BoxCollider2D weaponCol;
    
    [Header("Stats")]
    public float baseDamage = 10f;
    public float baseAttackSpeed = 2f;

    public void WeaponPerformAttack(float attackDuration)
    {
        StartCoroutine(ActivateAttack(attackDuration));
    }
    
    private IEnumerator ActivateAttack(float attackDuration)
    {
        weaponCol.enabled = true;
        yield return new WaitForSeconds(attackDuration);
        // ReSharper disable once Unity.InefficientPropertyAccess
        weaponCol.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent(out PhotonView pv))
        {
            if (pv.TryGetComponent(out IDamagable damagable))
            {
                if(!photonView.IsMine) return;
                pv.RPC(nameof(damagable.RPC_TakeDamage),RpcTarget.All,pv.ViewID,baseDamage);
                weaponCol.enabled = false;
            }
        }
    }
}
