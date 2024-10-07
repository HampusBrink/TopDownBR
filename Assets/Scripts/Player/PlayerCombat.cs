using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon")] 
    
    [Header("Arms")]
    
    
    [Header("Sweep Attack")]


    [Header("Attack")]
    [SerializeField] private float attackSpeed = 1f; // Attacks per second

    [Header("Other")]
    
    // Components
    private PlayerStatus _playerStatus;
    private PhotonView _pv;
    private Animator _swordAnimator;
        
    private float _attackDuration;
    private float _attackReturnDuration;
    private float _lastAttackTime;
    private Coroutine _currentAttackCoroutine;
    

    private void Start()
    {
        // Get the Animator component from the sword object
        
        // If you haven't assigned the sword in the Inspector, you can try to find it:
        // if (swordObject == null)
        //     swordObject = transform.Find("SwordObjectName").gameObject;
        
        // Make sure we have a reference to the sword's Animator
        if (_swordAnimator == null)
        {
            //Debug.LogError("Sword Animator not found!");
        }
        
        GetNeededComponents();
        if (!_pv.IsMine)
            return;
        UpdateStats();
    }

    private void GetNeededComponents()
    {
        _pv = GetComponent<PhotonView>();
        _playerStatus = GetComponent<PlayerStatus>();
    }
    
    private void UpdateStats()
    {
        UpdateAttackSpeed();
        //equippedWeapon.UpdateAttackDamage(_playerStatus.attackDamageMultiplier);
    }

    private void Update()
    {
        //equippedWeapon.UpdateWeaponLength(_playerStatus.weaponLengthMultiplier);
    }

    private void UpdateAttackSpeed()
    {
        //attackSpeed = _playerStatus.attackSpeedMultiplier * equippedWeapon.baseAttackSpeed;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Trigger the attack animation on the sword
            if (_swordAnimator != null)
            {
                _swordAnimator.SetTrigger("Attack");
            }
            else
            {
                Debug.LogWarning("Sword Animator is null. Cannot trigger attack animation.");
            }
        }
        
        if (!_pv.IsMine)
            return;
        UpdateStats();
        //UpdateAttackDurations();
    }
    
    

}
