using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [NonSerialized] public Transform _followTransform;
    void Start()
    {
        
    }

    void Update()
    {
        if(!_followTransform) return;

        transform.position = new Vector3(_followTransform.position.x,_followTransform.position.y,transform.position.z);
    }
}
