using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseWeapon : MonoBehaviourPunCallbacks
{

    // Components
    public Animator animator;
    //public SpriteRenderer weaponGFX;
    
    [Header("Weapon Stats")]
    public float baseDamage = 10f;
    public float baseAttackSpeed = 2f;
    

    protected float _multipliedDamage;

    [HideInInspector] public bool isAlreadyAttacking;

    protected virtual void Start()
    {
        BaseWeaponGetComponents();
        _multipliedDamage = baseDamage;
    }

    private void BaseWeaponGetComponents()
    {
        animator = gameObject.GetComponent<Animator>(); // doesn't work?? might have to assign through unity
    }
    
    
    
    

    public virtual void WeaponPerformAttack(float attackDuration, PlayerCombat.TurnDirection turnDirection)
    {
        if(isAlreadyAttacking) return;
        Debug.Log("Tries to trigger attack");
        
    }

    public void UpdateAttackDamage(float multiplier)
    {
        _multipliedDamage = baseDamage * multiplier;
    }

    
    
    

    
}
