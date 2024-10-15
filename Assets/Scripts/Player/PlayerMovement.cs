 using System;
 using FishNet.Object;
 using MultiplayerBase.Scripts;
 using Player;
 using UnityEngine;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 22f;
    [SerializeField] private float acceleration = 5f;
    
    [Header("Stamina")]
    [SerializeField] public float maxStamina = 100f;
    [SerializeField] public float staminaDrain = 2f;
    
    [Header("DodgeRoll")]
    [SerializeField] private float rollSpeed = 15f;
    [SerializeField] private float rollDuration = 0.3f;
    [SerializeField] private float rollCooldown = 1f;
    
    [Header("Other")]
    [SerializeField] private GameObject playerGFX;
    [SerializeField] private Image staminaBarFill;

    // Movement private fields
    private Vector2 _velocity = Vector2.zero;
    private float _desiredSpeed;
    private float _multipliedSpeed;
    private bool _isSprinting = false;
    private float _stamina;
    
    // DodgeRoll private fields
    private bool _isRolling = false;
    private bool _canRoll = true;
    private float _rollTime = 0f;
    private Vector2 _rollDirection;

    [SerializeField] public Animator bodyAnim;
    [SerializeField] public Animator legsAnim;
    
    // Components
    private Camera _camera;
    private PlayerStatus _playerStatus;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;
    
    public enum TurnDirection
    {
        Down = 0,
        DownRight = 1,
        Right = 2,
        UpRight = 3,
        Up = 4,
        UpLeft = 5,
        Left = 6,
        DownLeft = 7
    }
    
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        _rb = GetComponent<Rigidbody2D>();
        _playerStatus = GetComponent<PlayerStatus>();
        _camera = Camera.main;
        _stamina = maxStamina;

        if (!IsOwner) staminaBarFill.transform.parent.gameObject.SetActive(false);

        if (IsOwner && _camera) _camera.GetComponent<CameraMovement>().FollowTarget = transform;

        _desiredSpeed = _multipliedSpeed = walkSpeed;
    }

    private void Start()
    {
        if(!IsOffline) return;
        
        AssignComponents();
        
        _stamina = maxStamina;
        
        _desiredSpeed = _multipliedSpeed = walkSpeed;
    }

    private void AssignComponents()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerStatus = GetComponent<PlayerStatus>();
        _camera = Camera.main;
        if (_camera) _camera.GetComponent<CameraMovement>().FollowTarget = transform;
    }

    private void Update()
    {
        if(!IsOwner && !IsOffline) return;
        if(!_camera) return;
        
        if(!_isRolling)
            UpdateMoveDirection();
        
        HandleRollUpdate();
        
        UpdateStamina();
        Animate();
        
    }
    

    void FixedUpdate()
    {
        if(!IsOwner && !IsOffline) return;
        if(!_camera) return;

        HandleRollMovement();
        
        if (!GameManager.Instance.powerupPopup.gameObject.activeInHierarchy && !_isRolling)
        {
            ApplyMovement();
        }
    }

    private void UpdateMovementSpeed()
    {
        _desiredSpeed = _isSprinting && _stamina > 1 ? sprintSpeed : walkSpeed;
        _multipliedSpeed = _playerStatus.movementStatMultipliers.movementSpeedMultiplier * _desiredSpeed;
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if(!IsOwner && !IsOffline) return;

        UpdateMovementSpeed();
        
        if (context.performed)
        {
            _isSprinting = true;
        }
        else if (context.canceled)
        {
            _isSprinting = false;
        }
    }

    private void UpdateStamina()
    {
        if(!IsOwner && !IsOffline) return;
        if (_isSprinting)
        {
            _stamina = Mathf.Clamp(_stamina - staminaDrain * Time.deltaTime, 0, maxStamina);
        }
        else
        {
            _stamina = Mathf.Clamp(_stamina + (staminaDrain * 1.5f) * Time.deltaTime, 0, maxStamina);
        }
        UpdateStaminaBar();
    }
    
    private void UpdateStaminaBar()
    {
        float targetFillAmount = _stamina / maxStamina;
        staminaBarFill.fillAmount = targetFillAmount;
    }

    private Vector2 _input;
    [HideInInspector] public TurnDirection lastMovedirection;
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public TurnDirection currentTurnDirection = TurnDirection.Down;
    [HideInInspector] public TurnDirection currentMoveDirection = TurnDirection.Down;

    private readonly Vector2[] _vector2TurnDirections =
    {
        new(0f, -1f), // Down
        new(1f, -1f), // DownRight
        new(1f, 0f), // Right
        new(1f, 1f), // UpRight
        new(0f, 1f), // Up
        new(-1f, 1f), // UpLeft
        new(-1f, 0f), // Left
        new(-1f, -1f) // DownLeft
    };
    public Vector2 TurnDirectionToVector2(TurnDirection turnDirection)
    {
        return _vector2TurnDirections[(int)turnDirection];
    }
    
    public TurnDirection Vector2ToTurnDirection(Vector2 vector)
    {
        vector = vector.normalized; // Normalize the input vector

        for (int i = 0; i < _vector2TurnDirections.Length; i++)
        {
            // Normalize the stored direction vector to ensure diagonal movement is detected
            Vector2 direction = _vector2TurnDirections[i].normalized;

            if (Vector2.Distance(vector, direction) < 0.1f) // Adjust tolerance as needed
            {
                return (TurnDirection)i;
            }
        }

        return lastMovedirection;
        //throw new ArgumentException("Vector2 does not match any TurnDirection.");
    }

   

    public void SetTurnDirection(TurnDirection turnDirection)
    {
        currentTurnDirection = turnDirection;
        lastMovedirection = turnDirection;
    }
    
    private void UpdateMoveDirection()
    {
        if (!isMoving)
        {
            if (_input != Vector2.zero)
            {
                lastMovedirection = Vector2ToTurnDirection(_input);
                _input = Vector2.zero;
            }
        }
        else
        {
            _input = _moveVector;
            _input.Normalize();
        }

        currentMoveDirection = Vector2ToTurnDirection(_input);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        // moved isMoving = true from here
        if (!IsOwner && !IsOffline) return;
        if (!GameManager.Instance.GameStarted) return;

        isMoving = true;
        _moveVector = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            isMoving = false;
        }
        
    }
    void Animate()
    {
        bodyAnim.SetFloat("MoveX", _input.x);
        bodyAnim.SetFloat("MoveY", _input.y);
        bodyAnim.SetFloat("MoveMagnitude", _input.magnitude);
        bodyAnim.SetFloat("LastMoveX", TurnDirectionToVector2(lastMovedirection).x);
        bodyAnim.SetFloat("LastMoveY", TurnDirectionToVector2(lastMovedirection).y);
        
        
        legsAnim.SetFloat("MoveX", _input.x);
        legsAnim.SetFloat("MoveY", _input.y);
        legsAnim.SetFloat("MoveMagnitude", _input.magnitude);
        legsAnim.SetFloat("LastMoveX", TurnDirectionToVector2(lastMovedirection).x);
        legsAnim.SetFloat("LastMoveY", TurnDirectionToVector2(lastMovedirection).y);
    }

    private void ApplyMovement()
    {
        UpdateMovementSpeed();
        //print(_moveVector);
        if (_moveVector != Vector2.zero)
        {
            _velocity = Vector2.MoveTowards(_velocity, _moveVector * _multipliedSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            _velocity = Vector2.MoveTowards(_velocity, Vector2.zero, acceleration * Time.fixedDeltaTime);
        }

        _rb.velocity = _velocity;
    }

    #region Dodge Roll

    public void OnDodgeRoll(InputAction.CallbackContext context)
    {
        if (!IsOwner && !IsOffline) return;
        if (!GameManager.Instance.GameStarted) return;
        
        if (context.performed && _moveVector != Vector2.zero && _canRoll && !_isRolling)
        {
            StartRoll();
        }
    }
    
    private void StartRoll()
    {
        _isRolling = true;
        _canRoll = false;
        _rollTime = 0f;
        _rollDirection = _moveVector.normalized;
        _playerStatus.hitBox.enabled = false;
    
        // Make player invulnerable
        //_playerStatus.isInvulnerable = true;
    
        // Optionally play roll animation
        // bodyAnim.SetTrigger("Roll");
    
        Invoke(nameof(ResetRollCooldown), rollCooldown);
    }
    
    private void EndRoll()
    {
        _isRolling = false;
        _playerStatus.hitBox.enabled = true;
        //_playerStatus.isInvulnerable = false;
    }
    
    private void ResetRollCooldown()
    {
        _canRoll = true;
    }
    
    private void HandleRollUpdate()
    {
        if (_isRolling)
        {
            _rollTime += Time.deltaTime;
            if (_rollTime >= rollDuration)
            {
                EndRoll();
            }
        }
    }
    
    private void HandleRollMovement()
    {
        if (_isRolling)
        {
            _rb.velocity = _rollDirection * rollSpeed;
        }
    }

    #endregion
    
}
