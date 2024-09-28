using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class PlayerCombat : MonoBehaviour
{
    [Header("Arms")]
    [SerializeField] private Transform lArm;
    [SerializeField] private Transform rArm;
    [SerializeField] private Transform lArmOgPos;
    [SerializeField] private Transform rArmOgPos;
    [SerializeField] private float armReturnTime = 0.3f;
    
    [Header("Sweep Attack")]
    [SerializeField] private float sweepDuration = 0.5f; // Duration of the sweep
    [SerializeField] private float arcHeight = 0.5f; // Height of the arc
    [SerializeField] private float arcDistance = 1f; // How far the arm should move horizontally
    [SerializeField] private float arcHandRotation = 150f;

    private bool _isAttacking = false;

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !_isAttacking)
        {
            PerformSweepAttack();
        }
    }
    
    
    private void PerformSweepAttack()
    {
        StartCoroutine(SweepAttack(rArm));
    }

    private IEnumerator SweepAttack(Transform arm)
    {
        _isAttacking = true;
        float attackTimeElapsed = 0f;

        // Store the initial local position and rotation
        Vector3 initialLocalPosition = arm.localPosition;
        Quaternion initialLocalRotation = arm.localRotation;

        while (attackTimeElapsed < sweepDuration)
        {
            attackTimeElapsed += Time.deltaTime;
            float t = attackTimeElapsed / sweepDuration;

            // Calculate the arc movement relative to the original local position
            Vector3 arcPosition = initialLocalPosition + Vector3.right * (arcDistance * t); // Move to the right locally
            arcPosition.y += Mathf.Sin(t * Mathf.PI) * arcHeight; // Apply arc height locally

            // Set the arm's local position and local rotation
            arm.localPosition = arcPosition;
            arm.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(0, arcHandRotation, t)); // Rotate the arm locally

            yield return null;
        }

        float armReturnTimeElapsed = 0f;

        while (armReturnTimeElapsed < armReturnTime)
        {
            armReturnTimeElapsed += Time.deltaTime;
            float t = armReturnTimeElapsed / armReturnTime;

            arm.localPosition = Vector3.Lerp(arm.localPosition, initialLocalPosition, t);
            arm.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(arm.localRotation.z, initialLocalRotation.z, t));
            
            yield return null;
        }
        
        
        // Reset arm's local position and rotation after the attack
        arm.localPosition = initialLocalPosition;
        arm.localRotation = initialLocalRotation;
        _isAttacking = false;
    }
}
