using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using MultiplayerBase.Scripts;

public class Arrow : NetworkBehaviour
{
    [SerializeField] private CapsuleCollider collider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public Rigidbody rb;
    [SerializeField] private float arrowLength = 0.477f;

    [Range(0f, 1f)] 
    [SerializeField] private float stuckRatio = 0.5f;

    [HideInInspector] public int OwnerID;

    private float _damage;
    private float _range;
    
    private float _elapsedLifeTime;
    
    [Header("Ground Distance Settings")]
    [SerializeField] private float initialGroundDistance = 5f; // Set in Inspector
    [SerializeField] private float groundDistanceReductionRate = 1f; // Rate at which ground distance decreases
    [SerializeField] private float adjustGroundDistanceHeight = 5f;
    [SerializeField] private float forwardRayLength = 1f;
    //[SerializeField] private float attackRangeMultiplier = 1f; // Multiplies ground distance reduction and speed
    [SerializeField] private Transform rayCastOrigin;
    [SerializeField] private LayerMask groundLayerMask;
    
    private float _currentGroundDistance;
    private bool _isGroundDistanceZero = false;

    private void Start()
    {
        _currentGroundDistance = initialGroundDistance;
        
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }
    
    private void Update()
    {
        // Track the elapsed time and normalize it based on the arrow's range
        _elapsedLifeTime += Time.deltaTime;
        float t = _elapsedLifeTime / _range; // Calculate how far we are through the arrow's range

        // Lerp between white and black based on how much time has passed
        //spriteRenderer.color = Color.Lerp(_startColor, _endColor, t * t);
        
        AdjustArrowRot();
        
        
    }

    private void FixedUpdate()
    {
        Debug.DrawLine(rayCastOrigin.position, rayCastOrigin.position + rayCastOrigin.forward * forwardRayLength, Color.red, 1f);
        Debug.DrawLine(rayCastOrigin.position + new Vector3(0f,adjustGroundDistanceHeight,0f), rayCastOrigin.position, Color.cyan, 1f);

        if (!_isGroundDistanceZero)
        {
            AdjustGroundDistance();
        }
    }

    private void AdjustGroundDistance()
    {
        
        Ray downRay = new Ray(rayCastOrigin.position, Vector3.down);
        Ray forwardRay = new Ray(rayCastOrigin.position, Vector3.forward);
        if (Physics.Raycast(forwardRay, out RaycastHit forwardHitInfo, forwardRayLength, groundLayerMask))
        {
            // Debug.DrawLine(rayCastOrigin.position, rayCastOrigin.position + rayCastOrigin.forward * forwardRayLength, Color.red, 1f);
            Ray elevatedDownRay = new Ray(rayCastOrigin.position + new Vector3(0f,adjustGroundDistanceHeight,0f), Vector3.down);
            if (Physics.Raycast(elevatedDownRay, out RaycastHit elevatedDownRayHitInfo, adjustGroundDistanceHeight, groundLayerMask))
            {
                // Debug.DrawLine(rayCastOrigin.position + new Vector3(0f,adjustGroundDistanceHeight,0f), rayCastOrigin.position, Color.cyan, 1f);
                SetArrowToGroundHeight(elevatedDownRayHitInfo.point.y);
            }
        }
        // Ray to check for ground under the arrow
        else if (Physics.Raycast(downRay, out RaycastHit downHitInfo, adjustGroundDistanceHeight, groundLayerMask))
        {
            SetArrowToGroundHeight(downHitInfo.point.y);
        }

        // Reduce ground distance over time
        _currentGroundDistance -= groundDistanceReductionRate * Time.deltaTime * _range;

        // Handle when ground distance reaches 0
        if (_currentGroundDistance <= 0f)
        {
            _currentGroundDistance = 0f;
            _isGroundDistanceZero = true;
            EnableGravity();
        }
    }

    private void SetArrowToGroundHeight(float pointY)
    {
        // Calculate target height based on ground distance
        float targetHeight = pointY + _currentGroundDistance;

        // Adjust position smoothly or directly
        if (rb)
        {
            rb.MovePosition(new Vector3(rb.position.x, Mathf.Lerp(rb.position.y, targetHeight, 0.5f), rb.position.z));
        }
        else
        {
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
        }
    }

    private void EnableGravity()
    {
        if (rb != null)
        {
            rb.useGravity = true; // Re-enable gravity
        }
    }
    
    public void SetArrowStats(float damage, float range)
    {
        _damage = damage;
        _range = range;
    }

    private void AdjustArrowRot()
    {
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            // Make the arrow point in the direction of its velocity
            Quaternion targetRotation = Quaternion.LookRotation(rb.linearVelocity);

            // Apply the rotation offset
            transform.rotation = targetRotation * Quaternion.Euler(new Vector3(270f, 0f, 0f));
        }
    }

    private Vector3 ApproximatePastPosition()
    {
        return transform.position - rb.linearVelocity * Time.fixedDeltaTime * 5; // We approximate 5 time steps in the past to compensate for extreme speeds :)
    }

    private bool _hasStuckToObject = false;
    private void StickToObject(Collider col)
    {
        // Stop the arrow's movement
        if (rb == null) return;
        Ray ray = new Ray(ApproximatePastPosition(), rb.linearVelocity.normalized);
        if (col.Raycast(ray, out RaycastHit hit, rb.linearVelocity.magnitude))
        {
            Vector3 to = hit.point + (Quaternion.LookRotation(-transform.up, transform.forward) * Vector3.back) * stuckRatio * arrowLength * transform.localScale.y;
            StartCoroutine(StuckAnimation(Vector3.Dot(rb.linearVelocity, -transform.up), transform.position, to));
        }
        
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        _hasStuckToObject = true;

        // Attach the arrow to the object it collided with
        //transform.parent = col.transform;
    }

    private IEnumerator StuckAnimation(float speed, Vector3 from, Vector3 to)
    {
        float t = Vector3.Distance(from, to) / speed;
        float elapsedTime = 0;
        while (elapsedTime < t)
        {
            elapsedTime += Time.deltaTime;
            float elapsed01 = elapsedTime / t;
            transform.position = Vector3.Lerp(from, to, elapsed01);
            yield return null;
        }

        transform.position = to;
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if(!GameManager.Instance.localPlayer.IsServerStarted) return;
        
        //if (!GameManager.Instance.GameStarted) return; 

        if (col.gameObject.layer is 7) // Assuming PlayerHitbox is layer 7
        {
            if (col.transform.parent.TryGetComponent(out NetworkObject no))
            {
                if(no.OwnerId == OwnerID)
                    return;
                if (no.TryGetComponent(out IDamagable damagable))
                {
                    damagable.TakeDamage(_damage);
                }
            }
            Destroy(gameObject);
        }
        else if (col.gameObject.layer is 8) // Assuming Damager is layer 8
        {
            return;
        }
        else if (!_hasStuckToObject)
        {
            StickToObject(col);
        }
    }

}
