 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TurnDirection = PlayerMovement.TurnDirection;

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

    public override void WeaponPerformAttack(float attackDuration, TurnDirection turnDirection)
    {
        base.WeaponPerformAttack(attackDuration, turnDirection);
        RotateSwordToTurnDirection(turnDirection);
        animator.SetTrigger("Attack");
        StartCoroutine(ActivateAttack(attackDuration));
    }

    private void RotateSwordToTurnDirection(TurnDirection turnDirection)
    {
        switch (turnDirection)
        {
            case TurnDirection.Down:
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case TurnDirection.DownRight:
                transform.rotation = Quaternion.Euler(0f, 0f, 45f);
                break;
            case TurnDirection.Right:
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case TurnDirection.UpRight:
                transform.rotation = Quaternion.Euler(0f, 0f, 135f);
                break;
            case TurnDirection.Up:
                transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;
            case TurnDirection.UpLeft:
                transform.rotation = Quaternion.Euler(0f, 0f, 225f);
                break;
            case TurnDirection.Left:
                transform.rotation = Quaternion.Euler(0f, 0f, 270f);
                break;
            case TurnDirection.DownLeft:
                transform.rotation = Quaternion.Euler(0f, 0f, 315f);
                break;
        }
    }

    public override void UpdateWeaponTurnDir(TurnDirection turnDir)
    {
        RotateSwordToTurnDirection(turnDir);
    }
    

    private IEnumerator ActivateAttack(float attackDuration)
    {
        // ReSharper disable once Unity.InefficientPropertyAccess
        isAttacking = true;
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
        isAttacking = false;
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
