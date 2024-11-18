using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using TurnDirection = PlayerMovement.TurnDirection;

public abstract class BaseWeapon : MonoBehaviour
{

    // Components
    public Animator animator;
    protected PlayerInputHandler InputHandler;
    //public SpriteRenderer weaponGFX;
    
    [Header("Weapon Stats")]
    public float baseDamage = 10f;
    public float baseAttackRange = 1.0f;
    public float baseAttackSpeed = 2f;
    

    protected float MultipliedDamage;
    protected float MultipliedRange;
    protected float MultipliedAttackSpeed;

    [HideInInspector] public bool isAttacking;

    protected virtual void Start()
    {
        BaseWeaponGetComponents();
        MultipliedDamage = baseDamage;
    }

    private void BaseWeaponGetComponents()
    {
        animator = gameObject.GetComponent<Animator>(); // doesn't work?? might have to assign through unity
        InputHandler = PlayerInputHandler.Instance;
    }

    public void UpdateWeaponUpgrades(PlayerStatus.CombatUpgrades genericStats)
    {
        UpdateAttackDamage(genericStats.attackDamageMultiplier);
        UpdateAttackRange(genericStats.attackRangeMultiplier);
        UpdateAttackSpeed(genericStats.attackSpeedMultiplier);
    }

    public virtual void UpdateWeaponSpecificUpgrades(PlayerStatus playerStatus)
    {
        
    }
    
    
    public virtual void UpdateWeaponTurnDir(TurnDirection turnDir)
    {
        
    }
    
    

    public virtual void WeaponPerformAttack(TurnDirection turnDirection)
    {
        if(isAttacking) return;
        Debug.Log("Tries to trigger attack");
        
    }

    public virtual void WeaponReleaseAttack()
    {
        
    }

    protected virtual void UpdateAttackDamage(float multiplier)
    {
        MultipliedDamage = baseDamage * multiplier;
    }

    private void UpdateAttackRange(float multiplier)
    {
        MultipliedRange = baseAttackRange * multiplier;
    }

    private void UpdateAttackSpeed(float multiplier)
    {
        MultipliedAttackSpeed = baseAttackSpeed * multiplier;
    }

    
}
