using System;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using TurnDirection = PlayerMovement.TurnDirection;

namespace Player
{
    public class PlayerCombat : NetworkBehaviour
    {
        [Header("Weapon")] 
        public BaseWeapon equippedWeapon;
    
        [Header("Arms")]
    
    
        [Header("Sweep Attack")]


        [Header("Attack")]


        [Header("Other")]
    
        // Components
        private PlayerStatus _playerStatus;
        private Animator _swordAnimator;
        private PlayerMovement _playerMovement;
        
        private float _attackDuration;
        private float _attackReturnDuration;
        private float _lastAttackTime;
        private Coroutine _currentAttackCoroutine;


        public override void OnStartClient()
        {
            base.OnStartClient();
        
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
            if (!IsOwner)
                return;
            UpdateCombatStats();
        }

        private void Start()
        {
            if (_swordAnimator == null)
            {
                //Debug.LogError("Sword Animator not found!");
            }
        
            GetNeededComponents();
            if (!IsOffline)
                return;
            UpdateCombatStats();
        }

        private void GetNeededComponents()
        {
            _playerStatus = GetComponent<PlayerStatus>();
            _playerMovement = GetComponent<PlayerMovement>();
        }
    
        private void Update()
        {
            if(!IsOwner && !IsOffline) return;
            UpdateTurnDirection();
            PlayAttackAnimation();
        }
    
        private void UpdateCombatStats()
        {
            UpdateAttackSpeed();
            equippedWeapon.UpdateWeaponStats(_playerStatus.weaponStatMultipliers);
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
            if (!equippedWeapon.isAttacking && _playerMovement.isMoving)
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
            if (!IsOwner && !IsOffline)
                return;
            if (context.performed)
            {
                Debug.Log("Pressed attack");
                if (!equippedWeapon.isAttacking)
                {
                    _hitDirection = GetTurnDirectionFromMouse();
                    SetLastTurnDirectionFromMouse();
                    equippedWeapon.UpdateWeaponTurnDir(_hitDirection);
                    equippedWeapon.WeaponPerformAttack(_playerMovement.currentTurnDirection);
                }
            
            }
            else if (context.canceled)
            {
                Debug.Log("Released Attack");
                
                equippedWeapon.WeaponReleaseAttack();
            }
            
            UpdateCombatStats();
            //UpdateAttackDurations();
        }
        
        bool IsOppositeOrAdjacentDirection(TurnDirection hitDirection, TurnDirection moveDirection)
        {
            int hitValue = (int)hitDirection;
            int moveValue = (int)moveDirection;
            int difference = (hitValue - moveValue + 8) % 8; // Ensure positive difference

            // Check if the difference is 3, 4, or 5 (opposite or adjacent)
            return difference >= 3 && difference <= 5;
        }

        void PlayAttackAnimation()
        {
            if (equippedWeapon.isAttacking)
            {
                _playerMovement.bodyAnim.SetBool("IsAttacking", true);
                _playerMovement.bodyAnim.SetFloat("AttackX", _playerMovement.TurnDirectionToVector2(_hitDirection).x);
                _playerMovement.bodyAnim.SetFloat("AttackY", _playerMovement.TurnDirectionToVector2(_hitDirection).y);
    
                _playerMovement.legsAnim.SetFloat("MoveX", _playerMovement.TurnDirectionToVector2(_playerMovement.currentMoveDirection).x);
                _playerMovement.legsAnim.SetFloat("MoveY", _playerMovement.TurnDirectionToVector2(_playerMovement.currentMoveDirection).y);
    
                _playerMovement.legsAnim.SetBool("Reverse", IsOppositeOrAdjacentDirection(_hitDirection, _playerMovement.currentMoveDirection));
            }
            else
            {
                _playerMovement.bodyAnim.SetBool("IsAttacking", false);
                _playerMovement.bodyAnim.SetFloat("AttackX", 0);
                _playerMovement.bodyAnim.SetFloat("AttackY", 0);
                _playerMovement.legsAnim.SetBool("Reverse", false);
            }
        }
    }
}
