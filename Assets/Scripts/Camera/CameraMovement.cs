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
    [SerializeField] private float targetFollowStrength = 9f;         // How strongly the camera should follow the FollowTarget
    public bool shouldFocus;
    public float focusMouseOffsetMultiplier = 2f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private AnimationCurve mouseOffsetCurve;
    
    public Transform FollowTarget;
    
    private Camera _mainCamera;
    private bool _isZooming = false;
    private GameObject spectatedPlayer;
    private int spectatePlayerIndex;
    private float mouseOffsetMultiplier = 1.0f;

    private List<PlayerStatus> _alivePlayers;

    public InputActionReference altAttack, mousePos;
    
    private void OnEnable()
    {
        altAttack.action.performed += InputAltAttack;
        altAttack.action.canceled += InputAltAttack;
        
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnAlivePlayersChanged -= UpdateAlivePlayerList;
    }

    private void InputAltAttack(InputAction.CallbackContext context)
    {
        shouldFocus = context.ReadValueAsButton();
    }

    void Start()
    {
        _mainCamera = Camera.main;
        _currentPos = transform.position;
        
        if (GameManager.Instance != null)
            GameManager.Instance.OnAlivePlayersChanged += UpdateAlivePlayerList;

    }

    
    
    private void UpdateAlivePlayerList(List<PlayerStatus> alivePlayers)
    {
        _alivePlayers = alivePlayers;
    }

    private void Update()
    {
        
        CheckFocus();
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

    private void CheckFocus()
    {
        if (shouldFocus)
        {
            _isZooming = true;
            mouseOffsetMultiplier = focusMouseOffsetMultiplier;
        }
        else
        {
            _isZooming = false;
            mouseOffsetMultiplier = 1.0f;
        }
    }

    public Vector3 forwardOffsetVector = Vector3.down;
    public Vector3 upOffsetVector = Vector3.forward;
    
    private Vector3 _currentMouseOffset = Vector3.zero;
    private Vector3 _currentPos = Vector3.zero;
    private void MoveCamera4()
    {
        Vector2 mousePos = ((Vector2)_mainCamera.ScreenToViewportPoint(this.mousePos.action.ReadValue<Vector2>()) - 0.5f * Vector2.one) * 2; // origin is bottom left
    
        //Vector3 mouseOffset = mousePos - FollowTarget.position;
        //mouseOffset.z = 0; // Lock the Z-axis movement
        //mouseOffset.z = 0; // Lock the Z-axis movement
        
        Vector3 targetPos = FollowTarget.position + offset;
        Vector3 mouseVector = mousePos.normalized * (mouseOffsetCurve.Evaluate(mousePos.magnitude) * mouseOffsetMultiplier);
        _currentMouseOffset = Vector3.Lerp(_currentMouseOffset, Quaternion.LookRotation(forwardOffsetVector, upOffsetVector) * mouseVector, Time.deltaTime * mouseFollowStrength);
            
        
        
        _currentPos = Vector3.Lerp(_currentPos, targetPos, Time.deltaTime * targetFollowStrength);

        _mainCamera.transform.position = _currentPos + _currentMouseOffset;
        
        //Debug.Log(mousePos);
        
        
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
