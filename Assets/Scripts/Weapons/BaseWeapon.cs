using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;
using TurnDirection = PlayerMovement.TurnDirection;

public class BaseWeapon : MonoBehaviourPunCallbacks
{

    // Components
    public Animator animator;
    //public SpriteRenderer weaponGFX;
    
    [Header("Weapon Stats")]
    public float baseDamage = 10f;
    public float baseAttackRange = 1.0f;
    public float baseAttackSpeed = 2f;
    

    protected float _multipliedDamage;
    protected float _multipliedRange;
    protected float _multipliedAttackSpeed;

    [HideInInspector] public bool isAttacking;

    protected virtual void Start()
    {
        BaseWeaponGetComponents();
        _multipliedDamage = baseDamage;
    }

    private void BaseWeaponGetComponents()
    {
        animator = gameObject.GetComponent<Animator>(); // doesn't work?? might have to assign through unity
    }

    public void UpdateWeaponStats(PlayerStatus.WeaponStatMultipliers stats)
    {
        UpdateAttackDamage(stats.attackDamageMultiplier);
        UpdateAttackRange(stats.attackRangeMultiplier);
        UpdateAttackSpeed(stats.attackSpeedMultiplier);
    }
    
    
    public virtual void UpdateWeaponTurnDir(TurnDirection turnDir)
    {
        
    }
    
    

    public virtual void WeaponPerformAttack(TurnDirection turnDirection)
    {
        if(isAttacking) return;
        Debug.Log("Tries to trigger attack");
        
    }

    private void UpdateAttackDamage(float multiplier)
    {
        _multipliedDamage = baseDamage * multiplier;
    }

    private void UpdateAttackRange(float multiplier)
    {
        _multipliedDamage = baseAttackRange * multiplier;
    }

    private void UpdateAttackSpeed(float multiplier)
    {
        _multipliedAttackSpeed = baseAttackSpeed * multiplier;
    }

    
}
