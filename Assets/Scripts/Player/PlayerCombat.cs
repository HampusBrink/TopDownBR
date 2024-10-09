using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.InputSystem;
using TurnDirection = PlayerMovement.TurnDirection;

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon")] 
    public BaseWeapon equippedWeapon;
    
    [Header("Arms")]
    
    
    [Header("Sweep Attack")]


    [Header("Attack")]
    [SerializeField] private float attackSpeed = 1f; // Attacks per second

    [Header("Other")]
    
    // Components
    private PlayerStatus _playerStatus;
    private PhotonView _pv;
    private Animator _swordAnimator;
    private PlayerMovement _playerMovement;
        
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
        _playerMovement = GetComponent<PlayerMovement>();
    }
    
    private void UpdateStats()
    {
        UpdateAttackSpeed();
        //equippedWeapon.UpdateAttackDamage(_playerStatus.attackDamageMultiplier);
    }

    private void Update()
    {
        UpdateTurnDirection();
        //equippedWeapon.UpdateWeaponLength(_playerStatus.weaponLengthMultiplier);
    }

    private void UpdateAttackSpeed()
    {
        //attackSpeed = _playerStatus.attackSpeedMultiplier * equippedWeapon.baseAttackSpeed;
    }
    
    
    
    private TurnDirection GetTurnDirectionFromMouse()
    {
        Vector2 mouseScreenPosition = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 normalizedDirection = new Vector2(
            (mouseScreenPosition.x - screenCenter.x) / Screen.width,
            (mouseScreenPosition.y - screenCenter.y) / Screen.height
        );
    
        var angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg + 90;
        angle = (angle + 360) % 360;

        int directionIndex = Mathf.RoundToInt(angle / 45f) % 8;
        return (TurnDirection)directionIndex;
    }
    
    private void SetLastTurnDirectionFromMouse()
    {
        _playerMovement.SetTurnDirection(GetTurnDirectionFromMouse());
    }

    private void UpdateTurnDirection()
    {
        if (equippedWeapon.isAttacking)
        {
            _playerMovement.SetTurnDirection(_hitDirection);
            equippedWeapon.UpdateWeaponTurnDir(_hitDirection);
        }
        else if (!equippedWeapon.isAttacking && _playerMovement.isMoving)
        {
            _playerMovement.SetTurnDirection(_playerMovement.currentMoveDirection);
            equippedWeapon.UpdateWeaponTurnDir(_playerMovement.currentMoveDirection);
        }
        else if (!equippedWeapon.isAttacking && !_playerMovement.isMoving)
        {
            _playerMovement.SetTurnDirection(_playerMovement.lastMovedirection);
            equippedWeapon.UpdateWeaponTurnDir(_playerMovement.lastMovedirection);
        }
    }

    private TurnDirection _hitDirection;
    
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Pressed attack");
            _hitDirection = GetTurnDirectionFromMouse();
            SetLastTurnDirectionFromMouse();
            equippedWeapon.WeaponPerformAttack(0.5f, _playerMovement.currentTurnDirection);
        }
        
        if (!_pv.IsMine)
            return;
        UpdateStats();
        //UpdateAttackDurations();
    }
    
    

}
