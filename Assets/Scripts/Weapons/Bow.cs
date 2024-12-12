using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using TurnDirection = PlayerMovement.TurnDirection;

public class Bow : BaseWeapon
{
    [Header("Bow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private ParticleSystem chargeParticle;
    [SerializeField] private ParticleSystem fullChargeParticle;
    [SerializeField] private float maxBowPivotAngle = 30f;
    [SerializeField] private float maxShootForce = 20f;
    [SerializeField] private float damageToArrowSizeScale = 1.1f;
    
    [FormerlySerializedAs("bonusArrows")]
    [Header("Bonus arrow settings")]
    [SerializeField] private int baseBonusArrows = 0;
    [SerializeField] private float angleBetweenBonusArrows = 15;
    
    private int _bonusArrows;
    private float _playerDamageMultiplier = 1f;
    private float _windUpTimeElapsed = 0f;
    private Quaternion _initialBowRotation;
    private bool _isCharging = false;
    private bool _fullChargeParticlePlayed = false;
    private float _initialBowAngle;
    private float _maxWindUpTime = 1.0f;
    private ParticleSystem.MainModule _chargeParticleMain;
    
    public InputActionReference mousePos;

    protected override void Start()
    {
        base.Start();
        BowGetComponents();
    }
    
    private void BowGetComponents()
    {
        _camera = Camera.main;
        _chargeParticleMain = chargeParticle.main;
    }

    private void Update()
    {
        if (_isCharging)
        {
            _windUpTimeElapsed += Time.deltaTime;

            float chargePercentage = Mathf.Clamp01(_windUpTimeElapsed / _maxWindUpTime);
            _chargeParticleMain.simulationSpeed = Mathf.Lerp(0.3f, 1.5f, chargePercentage);

            if (chargePercentage == 1.0f && !_fullChargeParticlePlayed)
            {
                fullChargeParticle.Play();
                _fullChargeParticlePlayed = true;

                // Automatically fire the arrow when fully charged
                float shootForce = maxShootForce;
                SpawnArrow(_bonusArrows);

                // Reset charging and particles after shooting
                ResetBowAfterShot();
            }

            PivotBowRotation();
        }
        
        // Old logic: 
        /*
        if (_isCharging)
        {
            _windUpTimeElapsed += Time.deltaTime;
            
            float chargePercentage = Mathf.Clamp01(_windUpTimeElapsed / _maxWindUpTime);
            _chargeParticleMain.simulationSpeed = Mathf.Lerp(0.3f, 1.5f, chargePercentage);
            if (chargePercentage == 1.0f && !_fullChargeParticlePlayed)
            {
                fullChargeParticle.Play();
                _fullChargeParticlePlayed = true;
            }
            
            PivotBowRotation();
        }
        */
    }

    private float GetBowRotationFromTurnDirection(TurnDirection turnDirection)
    {
        switch (turnDirection)
        {
            case TurnDirection.Down:
                return 0f;
            case TurnDirection.DownRight:
                return 45f;
            case TurnDirection.Right:
                return 90f;
            case TurnDirection.UpRight:
                return 135f;
            case TurnDirection.Up:
                return 180f;
            case TurnDirection.UpLeft:
                return 225f;
            case TurnDirection.Left:
                return 270f;
            case TurnDirection.DownLeft:
                return 315f;
        }

        return 734523478f; //throw exception here. dunno how to yet
    }
    
    private void SetAttackInitialRotation(TurnDirection turnDirection)
    {
        _initialBowAngle = GetBowRotationFromTurnDirection(turnDirection);
        transform.rotation = Quaternion.Euler(0f, 0f, _initialBowAngle);
    }

    private float GetBowToMouseAngle()
    {
        Vector2 characterScreenPos = _camera.WorldToScreenPoint(transform.position);
        Vector2 mouseScreenPos = mousePos.action.ReadValue<Vector2>();
        Vector2 direction = (mouseScreenPos - characterScreenPos).normalized * -1;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private void PivotBowRotation()
    {
        float targetAngle = GetBowToMouseAngle();

        // Adjust for the bow's default orientation (assuming it faces right by default)
        float bowOffset = -90f; // Adjust this if the bow points in a different direction
        targetAngle += bowOffset;  // Note: Changing this from "-" to "+" for proper rotation offset

        // Calculate the angle difference relative to the initial bow angle
        float angleDifference = Mathf.DeltaAngle(_initialBowAngle, targetAngle);

        // Clamp the angle difference to within ±45 degrees from the initial angle
        float clampedAngleDifference = Mathf.Clamp(angleDifference, -45f, 45f);

        // Calculate the final angle by adding the clamped difference to the initial bow angle
        float finalAngle = _initialBowAngle + clampedAngleDifference;

        // Apply the rotation to the bow
        transform.localRotation = Quaternion.Euler(65f, 0f, finalAngle);
    }

    public override void UpdateWeaponSpecificUpgrades(PlayerStatus playerStatus)
    {
        if (playerStatus is BowPlayerStatus bowPlayerStatus)
        {
            _bonusArrows = baseBonusArrows + bowPlayerStatus.bowSpecificUpgrades.bonusArrows;
        }
        else
        {
            Debug.LogWarning("PlayerStatus is not a BowPlayerStatus.");
        }
    }

    protected override void UpdateAttackDamage(float multiplier)
    {
        base.UpdateAttackDamage(multiplier);
        _playerDamageMultiplier = multiplier;
    }

    private float GetShootForce()
    {
        return (MultipliedRange * maxShootForce) / 3f;
    }

    private void PropellArrow(Rigidbody rb, Quaternion arrowRotation)
    {
        rb.AddForce(arrowRotation * Vector3.up * (-1 * GetShootForce()), ForceMode.Impulse);
    }

    private Vector3 GetArrowSize()
    {
        float multipliedSize = _playerDamageMultiplier * damageToArrowSizeScale;
        return new Vector3(multipliedSize, multipliedSize, multipliedSize);
    }
    
    private void SpawnArrow(int bonusArrows)
    {
        float angleIncrement = 15f; // Angle between arrows, can be adjusted

        for (int i = -Mathf.FloorToInt(bonusArrows); i <= Mathf.FloorToInt(bonusArrows); i++)
        {
            Quaternion arrowRotation = transform.rotation * Quaternion.Euler(0, 0, i * angleBetweenBonusArrows);
            //GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowRotation);

            SRPC_SpawnArrow(arrowRotation,OwnerId);
            // arrow.transform.localScale = GetArrowSize();
            //
            // if (arrow.TryGetComponent(out Arrow arrowComponent))
            //     arrowComponent.SetArrowStats(MultipliedDamage, MultipliedRange);
            //
            // PropellArrow(arrowComponent.rb, arrowRotation);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SRPC_SpawnArrow(Quaternion arrowRotation,int owner) => ORPC_SpawnArrow(arrowRotation, owner);
    

    [ObserversRpc]
    private void ORPC_SpawnArrow(Quaternion arrowRotation,int ownerId)
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowRotation);

        arrow.GetComponent<Arrow>().OwnerID = ownerId;
        
         arrow.transform.localScale = GetArrowSize();
            
        if (arrow.TryGetComponent(out Arrow arrowComponent))
            arrowComponent.SetArrowStats(MultipliedDamage, MultipliedRange);
            
        PropellArrow(arrowComponent.rb, arrowRotation);
    }


    private TurnDirection _currentTurnDirection = TurnDirection.Down;
    private Camera _camera;

    public override void WeaponPerformAttack(TurnDirection turnDirection)
    {
        _maxWindUpTime = 1.0f / MultipliedAttackSpeed;
        SetAttackInitialRotation(turnDirection);
        _currentTurnDirection = turnDirection;
        _isCharging = true;
        _windUpTimeElapsed = 0f;
        isAttacking = true;
        chargeParticle.Play();
    }

    private void ResetBowAfterShot()
    {
        chargeParticle.Stop();
        _isCharging = false;
        isAttacking = false;
        _fullChargeParticlePlayed = false;
        _windUpTimeElapsed = 0f; // Reset charge timer
    }
    
    /*
    public override void WeaponReleaseAttack()
    {
        chargeParticle.Stop();
        _isCharging = false;
        isAttacking = false;
        _fullChargeParticlePlayed = false;

        
        float minWindUpTimeForShot = _maxWindUpTime / 3.0f;
        if (_windUpTimeElapsed >= minWindUpTimeForShot)
        {
            float chargePercentage = Mathf.Clamp01(_windUpTimeElapsed / _maxWindUpTime);
            float shootForce = chargePercentage * maxShootForce;
            SpawnArrow(shootForce, bonusArrows);
            Debug.Log($"Arrow shot with force: {shootForce}");
        }
        else
        {
            Debug.Log("Attack canceled - not enough charge");
        }

        _windUpTimeElapsed = 0f;
    }
    */
}

