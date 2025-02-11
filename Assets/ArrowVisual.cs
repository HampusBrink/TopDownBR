using System;
using UnityEngine;

public class ArrowVisual : MonoBehaviour
{
    [SerializeField] private Transform alignToVelocity;
    [SerializeField] Arrow arrow;
    [SerializeField] private float alignSharpness = 30f;
    
    private void Update()
    {
        Align();
    }

    private void Align()
    {
        Quaternion desired = Quaternion.LookRotation(arrow.GetVelocity(), Vector3.up);
        Debug.DrawRay(transform.position, Quaternion.LookRotation(arrow.GetVelocity(), Vector3.up) * Vector3.forward, Color.green);
        alignToVelocity.rotation = Quaternion.Slerp(alignToVelocity.rotation, desired, Time.deltaTime * alignSharpness);
    }
}
