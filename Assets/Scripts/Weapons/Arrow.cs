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
        _adjustedHeightThisStep = false;
        Debug.DrawLine(rayCastOrigin.position, rayCastOrigin.position + rayCastOrigin.forward * forwardRayLength, Color.red, 1f);
        Debug.DrawLine(rayCastOrigin.position + new Vector3(0f,adjustGroundDistanceHeight,0f), rayCastOrigin.position, Color.cyan, 1f);

        if (!_isGroundDistanceZero)
        {
            AdjustGroundDistance();
        }
    }

    private RaycastHit[] hits = new RaycastHit[128];
    private void AlignTrajectory()
    {
        float climbDistance = 1f;

        Vector3 from = transform.position + Vector3.up * climbDistance;
        float distance = climbDistance * 2f;

        Ray ray = new Ray(from, Vector3.down);

        int count = Physics.RaycastNonAlloc(ray, hits, distance, groundLayerMask);

        RaycastHit lowest = FindLowestHit(hits, count);
    }

    private RaycastHit FindLowestHit(RaycastHit[] hits, int count)
    {
        if (hits.Length == 0)
            return new RaycastHit(); // Make it empty somehow.

        RaycastHit compareHit = hits[0];

        for (int i = 1; i < count; i++)
        {
            if (hits[i].point.y < compareHit.point.y)
                compareHit = hits[i];
        }

        return compareHit;
    }
    
    Vector3 GetMoveOnNormal(Vector3 move, Vector3 worldNormal)
    {
        Vector3 cross = Vector3.Cross(move, Vector3.up); // Cross product magic to get the vector we want to move on the normal.
        return Vector3.Cross(worldNormal, cross); // The move-input vector we'll use for moving on surfaces.
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
                SetArrowToGroundHeight(elevatedDownRayHitInfo);
            }
        }
        // Ray to check for ground under the arrow
        else if (Physics.Raycast(downRay, out RaycastHit downHitInfo, adjustGroundDistanceHeight, groundLayerMask))
        {
            SetArrowToGroundHeight(downHitInfo);
        }

        // Reduce ground distance over time
        _currentGroundDistance -= groundDistanceReductionRate * Time.deltaTime * _range;

        // Handle when ground distance reaches 0
        if (_currentGroundDistance <= 0f)
        {
            _currentGroundDistance = 0f;
            _isGroundDistanceZero = true;
            EnableGravity();
            Debug.Log("Enables gravity for " + this.gameObject.name);
        }
    }

    private void SetArrowToGroundHeight(RaycastHit hit)
    {
        // Calculate target height based on ground distance
        float targetHeight = hit.point.y + _currentGroundDistance;

        // Adjust position smoothly or directly
        if (rb)
        {
            // rb.MovePosition(new Vector3(rb.position.x, Mathf.Lerp(rb.position.y, targetHeight, 0.5f), rb.position.z));
            //rb.MovePosition(new Vector3(rb.position.x, rb.position.y + _currentGroundDistance, rb.position.z));
            Vector3 height = hit.point + _currentGroundDistance * Vector3.up; 
            Vector3 diff = rayCastOrigin.position - height;
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
            AlignVelocity(hit);
            _adjustedHeightThisStep = true;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, targetHeight, transform.position.z);
        }
    }

    private void AlignVelocity(RaycastHit hit)
    {
        Vector3 normal = hit.normal;
        
        Vector3 newVel = GetMoveOnNormal(rb.linearVelocity.normalized, normal) * 10f;
        Debug.DrawRay(transform.position, newVel, Color.yellow, 1f);
        rb.linearVelocity = newVel;
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
    
    private bool _adjustedHeightThisStep = false;
    
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
        else if (!_hasStuckToObject && !_adjustedHeightThisStep)
        {
            StickToObject(col);
        }
        
    }

}
