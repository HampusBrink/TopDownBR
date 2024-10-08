using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.InputSystem;

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
        //equippedWeapon.UpdateWeaponLength(_playerStatus.weaponLengthMultiplier);
    }

    private void UpdateAttackSpeed()
    {
        //attackSpeed = _playerStatus.attackSpeedMultiplier * equippedWeapon.baseAttackSpeed;
    }
    
    public enum TurnDirection
    {
        Down = 0,
        DownRight = 1,
        Right = 2,
        UpRight = 3,
        Up = 4,
        UpLeft = 5,
        Left = 6,
        DownLeft = 7
    }
    
    private TurnDirection GetTurnDirection()
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
    
    private void SetLastTurnDirection()
    {
        TurnDirection direction = GetTurnDirection();
        Vector2[] directions = {
            new(0f, -1f),  // Down
            new(1f, -1f),  // DownRight
            new(1f, 0f),   // Right
            new(1f, 1f),   // UpRight
            new(0f, 1f),   // Up
            new(-1f, 1f),  // UpLeft
            new(-1f, 0f),  // Left
            new(-1f, -1f)  // DownLeft
        };

        _playerMovement.lastMovedirection = directions[(int)direction];
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Pressed attack");
            TurnDirection direction = GetTurnDirection();
            SetLastTurnDirection();
            equippedWeapon.WeaponPerformAttack(0.5f, direction);
        }
        
        if (!_pv.IsMine)
            return;
        UpdateStats();
        //UpdateAttackDurations();
    }
    
    

}
