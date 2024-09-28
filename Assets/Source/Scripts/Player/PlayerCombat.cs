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
    [SerializeField] private Transform rArm;
    [SerializeField] private Transform rArmOgPos;
    
    [Header("Sweep Attack")]
    [SerializeField] private float arcHeight = 0.5f;
    [SerializeField] private float arcDistance = 1f;
    [SerializeField] private float arcHandRotation = 150f;

    [Header("Attack")]
    [SerializeField] private float attackSpeed = 1f; // Attacks per second
    private const float ATTACK_DURATION_PERCENTAGE = 0.6f; // 60% of total duration
    private const float RETURN_DURATION_PERCENTAGE = 0.4f; // 40% of total duration

    private float _attackDuration;
    private float _attackReturnDuration;
    private float _lastAttackTime;
    private Coroutine _currentAttackCoroutine;

    private PhotonView _pv;

    private void Start()
    {
        _pv = GetComponent<PhotonView>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!_pv.IsMine) return;
        UpdateAttackDurations();
        
        if (context.performed)
        {
            PerformSweepAttack();
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
