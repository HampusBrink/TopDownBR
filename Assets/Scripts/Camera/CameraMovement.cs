using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MultiplayerBase.Scripts;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float mouseFollowStrength = 3f;          // How strongly the camera should adjust after the mouse
    
    [SerializeField] private float cameraMaxOffsetDistance = 3f;      // How far away the camera can possibly go from the FollowTarget
    [SerializeField] private float cameraFocusMaxOffsetDistance = 6f; // How far away the camera can possibly go from the FollowTarget while focusing
    
    [SerializeField] private float targetFollowStrength = 9f;         // How strongly the camera should follow the FollowTarget
    [SerializeField] private float maxMouseOffsetDistance = 5f;       // How far away the mouse can be from the target until the target to camera distance stops
    [SerializeField] private float cameraZOffset = -10f;
    
    [NonSerialized] public Transform FollowTarget;

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
        if(GameManager.Instance.isDead)
        {
            SpectateCamera();
            return;
        }
        if(!FollowTarget) return;

        //transform.position = new Vector3(FollowTarget.position.x,FollowTarget.position.y,transform.position.z);
        MoveCamera3();
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

    private void MoveCamera3()
    {
        // Get the mouse position in world space
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    
        // Calculate the mouse offset from the FollowTarget (player)
        Vector3 mouseOffset = mousePos - FollowTarget.position;
        mouseOffset.z = 0; // Lock the Z-axis movement

        // Get the magnitude (distance) of the mouse offset
        float mouseOffsetMagnitude = mouseOffset.magnitude;

        // Step 1: If the mouse is within maxMouseOffsetDistance, interpolate
        if (mouseOffsetMagnitude <= maxMouseOffsetDistance)
        {
            // Interpolate towards the mouse but clamp to _desiredMaxCameraOffsetDistance
            float t = mouseOffsetMagnitude / maxMouseOffsetDistance;

            // Calculate the target position with _desiredMaxCameraOffsetDistance limit
            Vector3 desiredOffset = mouseOffset.normalized * Mathf.Min(mouseOffsetMagnitude, _desiredMaxCameraOffsetDistance);
            _desiredCamPos = Vector3.Lerp(FollowTarget.position, FollowTarget.position + desiredOffset, t);
        }
        else
        {
            // Step 2: Clamp when the mouse exceeds maxMouseOffsetDistance
            Vector3 clampedOffset = mouseOffset.normalized * _desiredMaxCameraOffsetDistance;
            _desiredCamPos = FollowTarget.position + clampedOffset;
        }

        // Step 3: Smoothly move the camera towards the desired position with target follow strength
        Vector3 targetFollowPos = Vector3.Lerp(_mainCamera.transform.position, FollowTarget.position, targetFollowStrength * Time.deltaTime);
    
        // Blend between the target follow position and the desired camera position based on mouse movement
        Vector3 cameraLerp = Vector3.Lerp(targetFollowPos, _desiredCamPos, mouseFollowStrength * Time.deltaTime);

        // Maintain fixed z position
        cameraLerp.z = cameraZOffset;

        // Apply the new camera position
        _mainCamera.transform.position = cameraLerp;
    }
    
    private void MoveCamera2()
    {
        // Get the mouse position in world space
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
    
        // Calculate the mouse offset from the FollowTarget
        Vector3 mouseOffset = mousePos - FollowTarget.position;
    
        // Maintain the z-offset for the camera
        mouseOffset.z = 0;

        // Get the magnitude (distance) of the mouse offset
        float mouseOffsetMagnitude = mouseOffset.magnitude;

        Debug.Log(mouseOffsetMagnitude);
        
        // Step 1: If the mouse is within maxMouseOffsetDistance, interpolate
        if (mouseOffsetMagnitude <= maxMouseOffsetDistance)
        {
            
            // Calculate interpolation factor (t) based on how far the mouse is within the offset distance
            float t = mouseOffsetMagnitude / maxMouseOffsetDistance;

            // Interpolate the camera between the FollowTarget and the mouse position
            _desiredCamPos = Vector3.Lerp(FollowTarget.position, mouseOffset.normalized * _desiredMaxCameraOffsetDistance, t);
        }
        else
        {
            // Step 2: Clamp when the mouse exceeds maxMouseOffsetDistance
            // Clamp the mouseOffset to the _desiredMaxCameraOffsetDistance without pushing it further
            Vector3 clampedOffset = mouseOffset.normalized * _desiredMaxCameraOffsetDistance;
            clampedOffset.z = 0;    
            
            
            // Set the desired camera position as FollowTarget + the clamped offset
            _desiredCamPos = FollowTarget.position + clampedOffset;
        }

        // Smoothly move the camera towards the desired position
        Vector3 cameraLerp = Vector3.Lerp(_mainCamera.transform.position, _desiredCamPos, mouseFollowStrength * Time.deltaTime);
    
        // Set the camera's z position to maintain the correct depth
        cameraLerp.z = cameraZOffset;
    
        // Apply the new camera position
        _mainCamera.transform.position = cameraLerp;
    }

    
    private void MoveCamera()
    {
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = _mainCamera.transform.position.z;
    
        Vector3 mouseOffset = mousePos - FollowTarget.position;
        float mouseOffsetMagnitude = mouseOffset.magnitude;
    
        // Calculate the follow distance based on mouse offset
        float followDistance = Mathf.Lerp(0, _desiredMaxCameraOffsetDistance, 
            Mathf.Clamp01(mouseOffsetMagnitude / maxMouseOffsetDistance));
    
        // Clamp the mouse offset to the calculated follow distance
        if (mouseOffsetMagnitude > followDistance)
        {
            mouseOffset = mouseOffset.normalized * followDistance;
        }
    
        _desiredCamPos = FollowTarget.position + mouseOffset;
    
        Vector3 playerLerp = Vector3.Lerp(_mainCamera.transform.position, FollowTarget.position, targetFollowStrength * Time.deltaTime);
        Vector3 mouseLerp = Vector3.Lerp(playerLerp, _desiredCamPos, mouseFollowStrength * Time.deltaTime);
        mouseLerp.z = cameraZOffset;

        _mainCamera.transform.position = mouseLerp;
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
