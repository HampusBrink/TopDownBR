using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnDirection = PlayerMovement.TurnDirection;

public class Bow : BaseWeapon
{
    [Header("Bow Settings")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private ParticleSystem chargeParticle;
    [SerializeField] private ParticleSystem fullChargeParticle;
    //[SerializeField] private float maxWindUpTime = 2f;
    //[SerializeField] private float minWindUpTimeForShot = 0.5f;
    [SerializeField] private float maxBowPivotAngle = 30f;
    [SerializeField] private float maxShootForce = 20f;
    
    [Header("Bonus arrow settings")]
    [SerializeField] private int bonusArrows = 1;
    [SerializeField] private float angleBetweenBonusArrows = 15;
    

    private float _windUpTimeElapsed = 0f;
    private Quaternion _initialBowRotation;
    private bool _isCharging = false;
    private bool _fullChargeParticlePlayed = false;
    private float _initialBowAngle;
    private float _maxWindUpTime = 1.0f;
    private ParticleSystem.MainModule _chargeParticleMain;

    private void Start()
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
                SpawnArrow(shootForce, bonusArrows);
                Debug.Log($"Arrow shot with max force: {shootForce}");

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

    private void PivotBowRotation()
    {
        // Convert screen mouse position to world position
        Vector3 mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f; // Make sure the z-axis is 0 for 2D

        // Calculate direction from character to the mouse
        Vector2 directionToMouse = (mouseWorldPosition - transform.position).normalized * -1;

        // Calculate target angle to mouse in degrees
        float targetAngle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;

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
        transform.rotation = Quaternion.Euler(0f, 0f, finalAngle);
    }

    private float GetShootForce()
    {
        return (MultipliedAttackSpeed * maxShootForce) / 3f;
    }

    private void PropellArrow(Rigidbody2D rb, Quaternion arrowRotation)
    {
        rb.AddForce(arrowRotation * Vector2.up * (-1 * GetShootForce()), ForceMode2D.Impulse);
    }
    
    private void SpawnArrow(float shootForce, int bonusArrows)
    {
        int totalArrows = 1 + (2 * bonusArrows);
        float angleIncrement = 15f; // Angle between arrows, can be adjusted

        for (int i = -Mathf.FloorToInt(bonusArrows); i <= Mathf.FloorToInt(bonusArrows); i++)
        {
            Quaternion arrowRotation = transform.rotation * Quaternion.Euler(0, 0, i * angleBetweenBonusArrows);
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowRotation);
            
            if (arrow.TryGetComponent(out Arrow arrowComponent))
                arrowComponent.SetArrowStats(MultipliedDamage, MultipliedRange);
            
            PropellArrow(arrowComponent.rb, arrowRotation);
        }
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

