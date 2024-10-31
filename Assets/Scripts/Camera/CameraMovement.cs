using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiplayerBase.Scripts;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float mouseFollowStrength = 3f;          // How strongly the camera should adjust after the mouse
    
    [SerializeField] private float cameraMaxOffsetDistance = 3f;      // How far away the camera can possibly go from the FollowTarget
    [SerializeField] private float cameraFocusMaxOffsetDistance = 6f; // How far away the camera can possibly go from the FollowTarget while focusing
    
    [SerializeField] private float targetFollowStrength = 9f;         // How strongly the camera should follow the FollowTarget
    [SerializeField] private float maxMouseOffsetDistance = 5f;       // How far away the mouse can be from the target until the target to camera distance stops
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve mouseOffsetCurve;
    
    public Transform FollowTarget;

    private float _desiredMaxCameraOffsetDistance;
    
    private Camera _mainCamera;
    private Vector3 _desiredCamPos = Vector2.zero;
    private bool _isZooming = false;
    private GameObject spectatedPlayer;
    private int spectatePlayerIndex;

    private List<PlayerStatus> _alivePlayers;

    void Start()
    {
        _mainCamera = Camera.main;
        
        GameManager.Instance.OnAlivePlayersChanged += UpdateAlivePlayerList;
        _desiredMaxCameraOffsetDistance = _isZooming ? cameraFocusMaxOffsetDistance : cameraMaxOffsetDistance;

    }

    private void OnDisable()
    {
        GameManager.Instance.OnAlivePlayersChanged -= UpdateAlivePlayerList;
    }
    
    private void UpdateAlivePlayerList(List<PlayerStatus> alivePlayers)
    {
        _alivePlayers = alivePlayers;
    }

    void LateUpdate()
    {
        if(GameManager.Instance is not null && GameManager.Instance.isDead)
        {
            SpectateCamera();
            return;
        }
        if(!FollowTarget) return;

        //transform.position = new Vector3(FollowTarget.position.x,FollowTarget.position.y,transform.position.z);
        MoveCamera4();
    }

    public void OnZoomIn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _isZooming = true;
        }
        else if (context.canceled)
        {
            _isZooming = false;
        }
        
        _desiredMaxCameraOffsetDistance = _isZooming ? cameraFocusMaxOffsetDistance : cameraMaxOffsetDistance;
    }

    public Vector3 forwardOffsetVector = Vector3.down;
    public Vector3 upOffsetVector = Vector3.forward;
    
    private Vector3 _currentMouseOffset = Vector3.zero;
    private void MoveCamera4()
    {
        Vector2 mousePos = ((Vector2)_mainCamera.ScreenToViewportPoint(Input.mousePosition) - 0.5f * Vector2.one) * 2; // origin is bottom left
    
        //Vector3 mouseOffset = mousePos - FollowTarget.position;
        //mouseOffset.z = 0; // Lock the Z-axis movement
        
        Vector3 targetPos = FollowTarget.position + offset;
        Vector3 mouseVector = mousePos.normalized * mouseOffsetCurve.Evaluate(mousePos.magnitude);
        _currentMouseOffset = Vector3.Lerp(_currentMouseOffset, Quaternion.LookRotation(forwardOffsetVector, upOffsetVector) * mouseVector, Time.deltaTime * mouseFollowStrength);
            
        Vector3 camPos = _mainCamera.transform.position;
        
        
        camPos = _currentMouseOffset + Vector3.Lerp(camPos, targetPos, Time.deltaTime * targetFollowStrength);

        _mainCamera.transform.position = camPos;
        
        Debug.Log(mousePos);
        
        
    }
    
    private void MoveCamera3()
    {
        Vector3 mousePos = ((Vector2)_mainCamera.ScreenToViewportPoint(Input.mousePosition) - 0.5f * Vector2.one) * 2; // origin is bottom left
    
        Vector3 mouseOffset = mousePos - FollowTarget.position;
        mouseOffset.z = 0; // Lock the Z-axis movement

        float mouseOffsetMagnitude = mouseOffset.magnitude;

        if (mouseOffsetMagnitude <= maxMouseOffsetDistance)
        {
            float t = mouseOffsetMagnitude / maxMouseOffsetDistance;

            Vector3 desiredOffset = mouseOffset.normalized * Mathf.Min(mouseOffsetMagnitude, _desiredMaxCameraOffsetDistance);
            _desiredCamPos = Vector3.Lerp(FollowTarget.position, FollowTarget.position + desiredOffset, t);
        }
        else
        {
            Vector3 clampedOffset = mouseOffset.normalized * _desiredMaxCameraOffsetDistance;
            _desiredCamPos = FollowTarget.position + clampedOffset;
        }

        Vector3 targetFollowPos = Vector3.Lerp(_mainCamera.transform.position, FollowTarget.position + offset, targetFollowStrength * Time.deltaTime);
    
        Vector3 cameraLerp = Vector3.Lerp(targetFollowPos, _desiredCamPos, mouseFollowStrength * Time.deltaTime);


        _mainCamera.transform.position = cameraLerp;
    }

    private void SpectateCamera()
    {
        if(_alivePlayers.Count < 1) return;
        if (!spectatedPlayer) spectatedPlayer = _alivePlayers.First().gameObject;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            spectatePlayerIndex--;
            if (spectatePlayerIndex < 0) spectatePlayerIndex = GameManager.Instance.alivePlayers.Count - 1;

            spectatedPlayer = GameManager.Instance.alivePlayers[spectatePlayerIndex].gameObject;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            spectatePlayerIndex++;
            if (spectatePlayerIndex > GameManager.Instance.alivePlayers.Count - 1) spectatePlayerIndex = 0;

            spectatedPlayer = GameManager.Instance.alivePlayers[spectatePlayerIndex].gameObject;
        }
        
        if(!spectatedPlayer) return;
        _mainCamera.transform.position = new Vector3(spectatedPlayer.transform.position.x,spectatedPlayer.transform.position.y,_mainCamera.transform.position.z);
    }
}
