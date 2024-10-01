using System;
using UnityEngine;
using System.Collections;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Weapon")] 
    public BaseWeapon equippedWeapon;
    
    [Header("Arms")]
    [SerializeField] private Transform rArm;
    [SerializeField] private Transform lArm;
    [SerializeField] private Transform rArmOgPos;
    [SerializeField] private Transform hands;
    
    
    [Header("Sweep Attack")]
    [SerializeField] private float arcHeight = 0.5f;
    [SerializeField] private float arcDistance = 1f;
    [SerializeField] private float arcHandRotation = 150f;

    [Header("Attack")]
    [SerializeField] private float attackSpeed = 1f; // Attacks per second
    private const float ATTACK_DURATION_PERCENTAGE = 0.6f; // 60% of total duration
    private const float RETURN_DURATION_PERCENTAGE = 0.4f; // 40% of total duration

    [Header("Other")]
    
    private PlayerStatus _playerStatus;
    private PlayerMovement _playerMovement;
    private Vector3 handsInitialScale;
    private SpriteRenderer lArmSprite;
    private SpriteRenderer rArmSprite;
    private float _attackDuration;
    private float _attackReturnDuration;
    private float _lastAttackTime;
    private Coroutine _currentAttackCoroutine;

    private PhotonView _pv;

    private void Start()
    {
        _pv = GetComponent<PhotonView>();
        _playerStatus = GetComponent<PlayerStatus>();
        _playerMovement = GetComponent<PlayerMovement>();
        handsInitialScale = hands.localScale;
        lArmSprite = lArm.GetComponent<SpriteRenderer>();
        rArmSprite = rArm.GetComponent<SpriteRenderer>();
        if (!_pv.IsMine)
            return;
        UpdateAttackSpeed();
        equippedWeapon.UpdateAttackDamage(_playerStatus.attackDamageMultiplier);
    }

    private void Update()
    {
        //equippedWeapon.UpdateWeaponLength(_playerStatus.weaponLengthMultiplier);
    }

    private void UpdateAttackSpeed()
    {
        attackSpeed = _playerStatus.attackSpeedMultiplier * equippedWeapon.baseAttackSpeed;
    }

    
    enum TurnDirection
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

    private void SetArmsBehindPlayer(bool leftBehind, bool rightBehind)
    {
        lArmSprite.sortingOrder = leftBehind ? -4 : 6;
        rArmSprite.sortingOrder = rightBehind ? -4 : 6;
    }
    
    private void SetLastTurnDirection()
    {
        switch (GetTurnDirection())
        {
            case TurnDirection.Down:
                _playerMovement.lastMovedirection = new Vector2(0f,-1f);
                _playerMovement.Flip(true);
                hands.localScale = new Vector3(handsInitialScale.x * 1, handsInitialScale.y * -1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 0);
                SetArmsBehindPlayer(false, false);
                equippedWeapon.weaponGFX.sortingOrder = 6;
                break;
            case TurnDirection.DownRight:
                _playerMovement.lastMovedirection = new Vector2(1f,-1f);
                _playerMovement.Flip(true);
                hands.localScale = new Vector3(handsInitialScale.x, handsInitialScale.y * -1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 45);
                SetArmsBehindPlayer(false, false);
                equippedWeapon.weaponGFX.sortingOrder = 6;
                break;
            case TurnDirection.Right:
                _playerMovement.lastMovedirection = new Vector2(1f,0f);
                _playerMovement.Flip(true);
                hands.localScale = new Vector3(handsInitialScale.x, handsInitialScale.y * -1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 90);
                SetArmsBehindPlayer(true, false);
                equippedWeapon.weaponGFX.sortingOrder = 6;
                break;
            case TurnDirection.UpRight:
                _playerMovement.lastMovedirection = new Vector2(1f,1f);
                _playerMovement.Flip(true);
                hands.localScale = new Vector3(handsInitialScale.x, handsInitialScale.y * -1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 135);
                SetArmsBehindPlayer(true, true);
                equippedWeapon.weaponGFX.sortingOrder = 4;
                break;
            case TurnDirection.Up:
                _playerMovement.lastMovedirection = new Vector2(0f,1f);
                _playerMovement.Flip(true);
                hands.localScale = new Vector3(handsInitialScale.x * -1, handsInitialScale.y * 1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 0);
                SetArmsBehindPlayer(true, true);
                equippedWeapon.weaponGFX.sortingOrder = 4;
                break;
            case TurnDirection.UpLeft:
                _playerMovement.lastMovedirection = new Vector2(-1f,1f);
                _playerMovement.Flip(false);
                hands.localScale = new Vector3(handsInitialScale.x, handsInitialScale.y * 1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 45);
                SetArmsBehindPlayer(true, true);
                equippedWeapon.weaponGFX.sortingOrder = 4;
                break;
            case TurnDirection.Left:
                _playerMovement.lastMovedirection = new Vector2(-1f,0f);
                _playerMovement.Flip(false);
                hands.localScale = new Vector3(handsInitialScale.x, handsInitialScale.y * 1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 90);
                SetArmsBehindPlayer(false, true);
                equippedWeapon.weaponGFX.sortingOrder = 4;
                break;
            case TurnDirection.DownLeft:
                _playerMovement.lastMovedirection = new Vector2(-1f,-1f);
                _playerMovement.Flip(false);
                hands.localScale = new Vector3(handsInitialScale.x, handsInitialScale.y * 1, handsInitialScale.z);
                hands.eulerAngles = new Vector3(0, 0, 135);
                SetArmsBehindPlayer(false, false);
                equippedWeapon.weaponGFX.sortingOrder = 5;
                break;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!_pv.IsMine)
            return;
        UpdateAttackSpeed();
        equippedWeapon.UpdateAttackDamage(_playerStatus.attackDamageMultiplier);
        //equippedWeapon.UpdateWeaponLength(_playerStatus.weaponLengthMultiplier);
        if(!_pv.IsMine) return;
        UpdateAttackDurations();
        
        if (context.performed)
        {
            if (!equippedWeapon.isAlreadyAttacking)
                SetLastTurnDirection();
            
            PerformSweepAttack();
            equippedWeapon.WeaponPerformAttack(_attackDuration);
        }
    }
    
    private void UpdateAttackDurations()
    {
        float totalDuration = 1f / attackSpeed;
        _attackDuration = totalDuration * ATTACK_DURATION_PERCENTAGE;
        _attackReturnDuration = totalDuration * RETURN_DURATION_PERCENTAGE;
    }

    private void PerformSweepAttack()
    {
        float currentTime = Time.time;
        float timeSinceLastAttack = currentTime - _lastAttackTime;

        if (timeSinceLastAttack >= 1f / attackSpeed)
        {
            _lastAttackTime = currentTime;
            
            // Stop the current attack coroutine if it's still running
            if (_currentAttackCoroutine != null)
            {
                StopCoroutine(_currentAttackCoroutine);
            }

            // Start a new attack coroutine
            _currentAttackCoroutine = StartCoroutine(SweepAttackCoroutine());
        }
    }

    private IEnumerator SweepAttackCoroutine()
    {

        Vector3 initialLocalPosition = rArm.localPosition;
        Quaternion initialLocalRotation = rArm.localRotation;

        float attackTimeElapsed = 0f;

        while (attackTimeElapsed < _attackDuration)
        {
            attackTimeElapsed += Time.deltaTime;
            float t = attackTimeElapsed / _attackDuration;

            // Calculate the arc movement relative to the original local position
            Vector3 arcPosition = initialLocalPosition + Vector3.right * (arcDistance * t);
            arcPosition.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

            // Set the arm's local position and local rotation
            rArm.localPosition = arcPosition;
            rArm.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, arcHandRotation, t));

            yield return null;
        }

        // Start arm return as a separate coroutine
        StartCoroutine(ReturnArmCoroutine(initialLocalPosition, initialLocalRotation));
    }

    private IEnumerator ReturnArmCoroutine(Vector3 initialLocalPosition, Quaternion initialLocalRotation)
    {
        float returnTimeElapsed = 0f;

        Vector3 startPosition = rArm.localPosition;
        Quaternion startRotation = rArm.localRotation;

        while (returnTimeElapsed < _attackReturnDuration)
        {
            returnTimeElapsed += Time.deltaTime;
            float t = returnTimeElapsed / _attackReturnDuration;

            rArm.localPosition = Vector3.Lerp(startPosition, initialLocalPosition, t);
            rArm.localRotation = Quaternion.Slerp(startRotation, initialLocalRotation, t);

            yield return null;
        }

        rArm.localPosition = initialLocalPosition;
        rArm.localRotation = initialLocalRotation;
    }
}
