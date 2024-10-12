using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnDirection = PlayerMovement.TurnDirection;

public class Bow : BaseWeapon
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private float maxWindUpTime = 2f;
    [SerializeField] private float minWindUpTimeForShot = 0.5f;
    [SerializeField] private float maxBowPivotAngle = 30f;
    [SerializeField] private float maxShootForce = 20f;

    private float windUpTimeElapsed = 0f;
    private Vector2 initialMousePosition;
    private Quaternion initialBowRotation;
    private bool isCharging = false;
    private float initialBowAngle;

    private void Update()
    {
        if (isCharging)
        {
            windUpTimeElapsed += Time.deltaTime;
            PivotBowRotation();
        }
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
        initialBowAngle = GetBowRotationFromTurnDirection(turnDirection);
        transform.rotation = Quaternion.Euler(0f, 0f, initialBowAngle);
        initialMousePosition = Input.mousePosition;
    }

    private void PivotBowRotation()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        float verticalDelta = currentMousePosition.y - initialMousePosition.y;
        
        float pivotAngle = Mathf.Clamp(verticalDelta / Screen.height * maxBowPivotAngle, -maxBowPivotAngle, maxBowPivotAngle);
        float newAngle = initialBowAngle + pivotAngle;

        // Ensure the new angle stays within 45 degrees of the initial angle
        float minAngle = initialBowAngle - 45f;
        float maxAngle = initialBowAngle + 45f;
        newAngle = Mathf.Clamp(newAngle, minAngle, maxAngle);

        transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
    }
    
    private void SpawnArrow(float shootForce)
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, transform.rotation);
        Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
        if (arrowRb != null)
        {
            arrowRb.AddForce(transform.up * -1 * shootForce, ForceMode2D.Impulse);
        }
    }

    private TurnDirection _currentTurnDirection = TurnDirection.Down;
    public override void WeaponPerformAttack(TurnDirection turnDirection)
    {
        SetAttackInitialRotation(turnDirection);
        _currentTurnDirection = turnDirection;
        isCharging = true;
        windUpTimeElapsed = 0f;
        isAttacking = true;
    }
    
    public override void WeaponReleaseAttack()
    {
        isCharging = false;
        isAttacking = false;

        if (windUpTimeElapsed >= minWindUpTimeForShot)
        {
            float chargePercentage = Mathf.Clamp01(windUpTimeElapsed / maxWindUpTime);
            float shootForce = chargePercentage * maxShootForce;
            SpawnArrow(shootForce);
            Debug.Log($"Arrow shot with force: {shootForce}");
        }
        else
        {
            Debug.Log("Attack canceled - not enough charge");
        }

        windUpTimeElapsed = 0f;
    }
}

