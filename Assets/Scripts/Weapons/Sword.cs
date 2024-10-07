 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Sword : BaseWeapon
{
    [Header("Sword Settings")]
    public GameObject weaponColObject;
    private Transform weaponColTransform;
    private BoxCollider2D weaponCol;
    [SerializeField] private float sweepStartAngle = 90f;
    [SerializeField] private float sweepEndAngle = -90f;
    
    [Header("Sword Specific Stats")]
    public float baseWeaponLength = 1.4f;

    protected override void Start()
    {
        base.Start();
        SwordGetComponents();
        UpdateWeaponLength(1.0f); // fix this later
    }

    private void SwordGetComponents()
    {
        weaponColTransform = weaponColObject.transform;
        weaponCol = weaponColObject.GetComponent<BoxCollider2D>();
    }
    
    
    private float _multipliedWeaponLength;
    public void UpdateWeaponLength(float multiplier)
    {
        _multipliedWeaponLength = baseWeaponLength * multiplier;
        weaponCol.transform.localScale = new Vector3(weaponCol.transform.localScale.x, _multipliedWeaponLength, weaponCol.transform.localScale.z);
    }

    public override void WeaponPerformAttack(float attackDuration)
    {
        base.WeaponPerformAttack(attackDuration);
        animator.SetTrigger("Attack");
        StartCoroutine(ActivateAttack(attackDuration));
    }
    
    private IEnumerator ActivateAttack(float attackDuration)
    {
        // ReSharper disable once Unity.InefficientPropertyAccess
        isAlreadyAttacking = true;
        weaponCol.enabled = true;
        
        float attackTimeElapsed = 0f;
        while (attackTimeElapsed < attackDuration)
        {
            attackTimeElapsed += Time.deltaTime;
            float t = attackTimeElapsed / attackDuration;
            weaponColTransform.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(sweepStartAngle, sweepEndAngle, t));
            yield return null;
        }
        
        weaponCol.enabled = false;
        isAlreadyAttacking = false;
    }
    
    // private void OnTriggerEnter2D(Collider2D col)
    // {
    //     if(!GameManager.Instance.GameStarted) return;
    //     if (col.TryGetComponent(out PhotonView pv))
    //     {
    //         if (pv.TryGetComponent(out IDamagable damagable))
    //         {
    //             if(!photonView.IsMine) return;
    //             pv.RPC(nameof(damagable.RPC_TakeDamage),RpcTarget.All,pv.ViewID,_multipliedDamage);
    //             weaponCol.enabled = false;
    //         }
    //     }
    // }
}
