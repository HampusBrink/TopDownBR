using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using MultiplayerBase.Scripts;

public class Arrow : NetworkBehaviour
{
    private Rigidbody rb;
    
    [HideInInspector] public int OwnerID;
    
    private float _damage;
    private float _range;
    
    [Header("Height Adjust Things")]
    [SerializeField] private float surfaceSnapDistance = 5f;
    [SerializeField] private LayerMask adjustToLayers;
    [SerializeField] private AnimationCurve heightOverTime;
    [SerializeField] private float defaultArrowSpeed = 2f;
    private float _currentFlyTime = 0f;
    
    
    
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void Start()
    {
    }


    public void Shoot(Vector3 shootVec)
    {
        rb.linearVelocity = shootVec * defaultArrowSpeed;
    }
    
    public void SetArrowStats(float damage, float range)
    {
        _damage = damage;
        _range = range;
    }

    private void FixedUpdate()
    {
        if (CheckForEndFly())
            return;
        
        CollideCheck();
        _currentFlyTime += Time.fixedDeltaTime;
        AlignVerticalVelocity();
    }

    public Vector3 GetVelocity() => rb.linearVelocity;

    private bool CheckForEndFly()
    {
        // Assuming the curve ends at x = 1
        if (_currentFlyTime * defaultArrowSpeed >= 1f)
        {
            rb.useGravity = true;
            return true;
        }

        return false;
    }

    private Vector3 PredictPos()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 pos = transform.position;
        Vector3 predictedPos = pos + vel * Time.fixedDeltaTime;
        return predictedPos;
    }

    private RaycastHit[] heightHits = new RaycastHit[128];

    private void CollideCheck()
    {
        Vector3 from = transform.position;
        Vector3 to = PredictPos();
        Vector3 dir = to - from;
        Debug.DrawRay(from, dir, Color.red, 1.5f);
        if (Physics.Raycast(from, dir, dir.magnitude, adjustToLayers)) // Wall in front of arrow
        {
            float maxClimbDistance = surfaceSnapDistance;
            Ray ray = new Ray(transform.position, Vector3.up);
            if (Physics.Raycast(ray, out RaycastHit hit, surfaceSnapDistance, adjustToLayers))
            {
                maxClimbDistance = hit.distance;
            }
            if (!TryAdjustHeightUp(maxClimbDistance))
            {
                Destroy(gameObject); // stick instead
            }
        }
        else
        {
            TryAdjustHeightDown();
        }
    }
    
    private bool TryAdjustHeightUp(float maxClimbDistance)
    {
        Vector3 from = PredictPos() + Vector3.up * surfaceSnapDistance;
        Ray ray = new Ray(from, Vector3.down);
        
        Debug.DrawRay(from, Vector3.down * surfaceSnapDistance, Color.green, 1.5f);
        int count = Physics.RaycastNonAlloc(ray, heightHits, surfaceSnapDistance, adjustToLayers);
        if (count <= 0)
            return false;

        RaycastHit lowest = GetExtremeHit(heightHits, count, false);
        if (lowest.distance > maxClimbDistance)
        {
            return false;
        }
        transform.position = new Vector3(transform.position.x, lowest.point.y + GetHoverHeight(), transform.position.z);
        
        return true;
    }

    private bool TryAdjustHeightDown()
    {
        Vector3 from = PredictPos();
        Ray ray = new Ray(from, Vector3.down);
        
        Debug.DrawRay(from, Vector3.down * surfaceSnapDistance, Color.cyan, 1.5f);
        int count = Physics.RaycastNonAlloc(ray, heightHits, surfaceSnapDistance, adjustToLayers);
        if (count <= 0)
            return false;
        
        RaycastHit highest = GetExtremeHit(heightHits, count, true);
        transform.position = new Vector3(transform.position.x, highest.point.y + GetHoverHeight(), transform.position.z);
        
        return true;
    }
    
    private void AlignVerticalVelocity()
    {
        float diff = GetHoverHeight() - GetHoverHeight(-Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, diff / Time.fixedDeltaTime, rb.linearVelocity.z);
    }
    
    private float GetHoverHeight()
    {
        return heightOverTime.Evaluate(_currentFlyTime * defaultArrowSpeed);
    }
    
    private float GetHoverHeight(float offset)
    {
        return heightOverTime.Evaluate((Mathf.Clamp01(_currentFlyTime*defaultArrowSpeed) + offset));
    }
    
    private RaycastHit GetExtremeHit(RaycastHit[] hits, int count, bool highest)
    {
        RaycastHit compareHit = hits[0];
        if (count == 1)
            return compareHit;

        for (int i = 1; i < count; i++)
        {
            if (highest)
            {
                if (compareHit.point.y < hits[i].point.y)
                {
                    compareHit = hits[i];
                }
            }
            else
            {
                if (compareHit.point.y > hits[i].point.y)
                {
                    compareHit = hits[i];
                }
            }
        }
        return compareHit;
    }
}
