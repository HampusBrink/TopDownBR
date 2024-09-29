using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float mouseFollowStrength = 3f;
    
    [SerializeField] private float cameraMaxOffsetDistance = 3f;
    [SerializeField] private float cameraFocusMaxOffsetDistance = 6f;
    
    [SerializeField] private float playerFollowStrength = 9f;
    [SerializeField] private float maxMouseOffsetDistance = 5f; 
    [SerializeField] private float cameraZoom = -10f;
    
    [NonSerialized] public Transform FollowTarget;

    private float _desiredMouseFollowDistance;
    
    private Camera _mainCamera;
    private Vector3 _desiredCamPos = Vector2.zero;
    private bool _isZooming = false;
    private GameObject spectatedPlayer;
    private int spectatePlayerIndex;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if(!FollowTarget) return;

        //transform.position = new Vector3(FollowTarget.position.x,FollowTarget.position.y,transform.position.z);
        MoveCamera();
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
        
        _desiredMouseFollowDistance = _isZooming ? cameraFocusMaxOffsetDistance : cameraMaxOffsetDistance;
    }

    private void MoveCamera()
    {
        if(GameManager.Instance.isDead)
        {
            SpectateCamera();
            return;
        }
        Vector3 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = _mainCamera.transform.position.z;
    
        Vector3 mouseOffset = mousePos - FollowTarget.position;
        float mouseOffsetMagnitude = mouseOffset.magnitude;
    
        // Calculate the follow distance based on mouse offset
        float followDistance = Mathf.Lerp(0, _desiredMouseFollowDistance, 
            Mathf.Clamp01(mouseOffsetMagnitude / maxMouseOffsetDistance));
    
        // Clamp the mouse offset to the calculated follow distance
        if (mouseOffsetMagnitude > followDistance)
        {
            mouseOffset = mouseOffset.normalized * followDistance;
        }
    
        _desiredCamPos = FollowTarget.position + mouseOffset;
    
        Vector3 playerLerp = Vector3.Lerp(_mainCamera.transform.position, FollowTarget.position, playerFollowStrength * Time.deltaTime);
        Vector3 mouseLerp = Vector3.Lerp(playerLerp, _desiredCamPos, mouseFollowStrength * Time.deltaTime);
        mouseLerp.z = cameraZoom;

        _mainCamera.transform.position = mouseLerp;
    }

    private void SpectateCamera()
    {
        if(!GameManager.Instance.isDead) return;
        if (!spectatedPlayer) spectatedPlayer = GameManager.Instance._alivePlayers.First().gameObject;
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            repeat:
            spectatePlayerIndex--;
            if (spectatePlayerIndex < 0) spectatePlayerIndex = GameManager.Instance._alivePlayers.Count - 1;
            spectatedPlayer = GameManager.Instance._alivePlayers[spectatePlayerIndex].gameObject;
            
            if (GameManager.Instance._alivePlayers[spectatePlayerIndex] == null &&
                GameManager.Instance._alivePlayers.Count > 0)
            {
                GameManager.Instance._alivePlayers.RemoveAt(spectatePlayerIndex);
                goto repeat;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            repeat:
            spectatePlayerIndex++;
            if (spectatePlayerIndex >= GameManager.Instance._alivePlayers.Count - 1) spectatePlayerIndex = 0;
            spectatedPlayer = GameManager.Instance._alivePlayers[spectatePlayerIndex].gameObject;
            
            if (GameManager.Instance._alivePlayers[spectatePlayerIndex] == null &&
                GameManager.Instance._alivePlayers.Count > 0)
            {
                GameManager.Instance._alivePlayers.RemoveAt(spectatePlayerIndex);
                goto repeat;
            }
        }
        
        if(!spectatedPlayer) return;
        _mainCamera.transform.position = spectatedPlayer.transform.position;
    }
}
