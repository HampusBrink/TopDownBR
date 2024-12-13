 using System;
 using FishNet.Object;
 using MultiplayerBase.Scripts;
 using Player;
 using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 22f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float gravityMultiplier = 0.4f;
    [SerializeField] private LayerMask groundLayer;
    
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
    private bool _grounded = true;
    private Vector3 _velocity = Vector3.zero;
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
    //private CharacterController _characterController;
    private Rigidbody _rb;
    private CapsuleCollider _col;
    public InputActionReference move, sprint, dodge;
    
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
        
        
        _stamina = maxStamina;

        if (!IsOwner) staminaBarFill.transform.parent.gameObject.SetActive(false);

        if(IsOwner) AssignComponents();
        

        _desiredSpeed = _multipliedSpeed = walkSpeed;
    }

    private void OnEnable()
    {
        move.action.performed += InputMove;
        move.action.canceled += InputMove;
        
        sprint.action.performed += InputSprint;
        sprint.action.canceled += InputSprint;
        
        dodge.action.started += InputDodge;
    }

    private void OnDisable()
    {
        move.action.performed -= InputMove;
        move.action.canceled -= InputMove;
        
        sprint.action.performed -= InputSprint;
        sprint.action.canceled -= InputSprint;
        
        dodge.action.started -= InputDodge;
    }

    #region Inputs

    public void InputMove(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
    }

    public void InputSprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.ReadValueAsButton();
    }

    public void InputDodge(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton())
            return;
        if (_moveInput != Vector2.zero && _canRoll && !_isRolling)
        {
            StartRoll();
        }
    }
    
    public bool GetDodgeInput()
    {
        return dodge.action.ReadValue<float>() != 0f;
    }

    #endregion

    private void Start()
    {
        if(!IsOffline) return;
        
        _stamina = maxStamina;
        
        _desiredSpeed = _multipliedSpeed = walkSpeed;
    }

    private void AssignComponents()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        _playerStatus = GetComponent<PlayerStatus>();
        _camera = Camera.main;
        if (_camera) _camera.GetComponent<CameraMovement>().FollowTarget = transform;
    }

    private void Update()
    {
        if(!GameManager.Instance.GameStarted) return;
        if(!IsOwner && !IsOffline) return;
        if(!_camera) return;
        
        
        
        if(!_isRolling)
            UpdateMoveDirection();
        
        HandleRollUpdate();
        
        Animate();
    }
    

    void FixedUpdate()
    {
        if(!GameManager.Instance.GameStarted) return;
        if(!IsOwner && !IsOffline) return;
        if(!_camera) return;

        HandleRollMovement();
        UpdateStamina();
        
        if (/*!GameManager.Instance.upgradePopup.gameObject.activeInHierarchy &&*/ !_isRolling)
        {
            ApplyMovement();
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

    private Vector2 _moveInput;
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
        if (_moveInput != Vector2.zero)
        {
            lastMovedirection = Vector2ToTurnDirection(_moveInput);
            isMoving = true;
        }
        else
            isMoving = false;
    
        currentMoveDirection = Vector2ToTurnDirection(_moveInput);
    }
    
    void Animate()
    {
        bodyAnim.SetFloat("MoveX", _moveInput.x);
        bodyAnim.SetFloat("MoveY", _moveInput.y);
        bodyAnim.SetFloat("MoveMagnitude", _moveInput.magnitude);
        bodyAnim.SetFloat("LastMoveX", TurnDirectionToVector2(lastMovedirection).x);
        bodyAnim.SetFloat("LastMoveY", TurnDirectionToVector2(lastMovedirection).y);
        
        
        legsAnim.SetFloat("MoveX", _moveInput.x);
        legsAnim.SetFloat("MoveY", _moveInput.y);
        legsAnim.SetFloat("MoveMagnitude", _moveInput.magnitude);
        legsAnim.SetFloat("LastMoveX", TurnDirectionToVector2(lastMovedirection).x);
        legsAnim.SetFloat("LastMoveY", TurnDirectionToVector2(lastMovedirection).y);
    }
    
    private void UpdateMovementSpeed()
    {
        _desiredSpeed = _isSprinting && _stamina > 1 ? sprintSpeed : walkSpeed;
        _multipliedSpeed = _playerStatus.GetMovementSpeedMultiplier() * _desiredSpeed;
    }

    private Vector3 Gravity()
    {
        return Physics.gravity * gravityMultiplier;
    }
    
    Vector3 GetMoveOnNormal(Vector3 move, Vector3 worldNormal)
    {
        Vector3 cross = Vector3.Cross(move, Vector3.up); // Cross product magic to get the vector we want to move on the normal.
        return Vector3.Cross(worldNormal, cross); // The move-input vector we'll use for moving on surfaces.
    }

    private void AdjustToGround()
    {
        Vector3 origin = transform.position - Vector3.up  * (_col.height / 2f) + _rayPadding * Vector3.up;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _rayPadding * 2f, groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);

            float groundDistance = (hit.distance - _rayPadding) * Mathf.Cos(angle);
            transform.position -= Vector3.up * groundDistance;
        }
    }

    private Vector3 _groundNormal = Vector3.up;

    private readonly float _rayPadding = 0.1f;
    private void RayCastGroundCheck()
    {
        Vector3 origin = transform.position - Vector3.up  * (_col.height / 2f) + _rayPadding * Vector3.up;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _rayPadding * 2f, groundLayer))
        {
            _grounded = true;
            _groundNormal = hit.normal;
        }
        else
        {
            _grounded = false;
            _groundNormal = Vector3.up;
        }
        //Debug.DrawRay(origin, Vector3.down * (_rayPadding * 2), Color.magenta);
        //Debug.DrawRay(origin, _groundNormal, Color.green);
    }

    
    private void ApplyMovement()
    {
        RayCastGroundCheck();
        AdjustToGround();
        UpdateMovementSpeed();
        
        Vector3 input = new Vector3(_moveInput.x, 0, _moveInput.y);
        Vector3 planeMove = GetMoveOnNormal(input, _groundNormal);
        _velocity = Vector3.MoveTowards(_velocity, planeMove * (_multipliedSpeed), acceleration);
        
        //Debug.DrawRay(transform.position, _velocity, Color.cyan);

        if (!_grounded)
        {
            _velocity += Gravity() + _rb.linearVelocity.y * Vector3.up;
        }

        _rb.linearVelocity = _velocity;
    }

    #region Dodge Roll
    
    private void StartRoll()
    {
        //Debug.Log("StartRoll");
        _isRolling = true;
        _canRoll = false;
        _rollTime = 0f;
        _rollDirection = _moveInput.normalized;
        _playerStatus.hitBox.gameObject.SetActive(false);
    
    
        // Optionally play roll animation
        // bodyAnim.SetTrigger("Roll");
    
        Invoke(nameof(ResetRollCooldown), rollCooldown);
    }
    
    private void EndRoll()
    {
        _isRolling = false;
        _playerStatus.hitBox.gameObject.SetActive(true);
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
            Vector3 rollMovement = new Vector3(_rollDirection.x, 0, _rollDirection.y) * (rollSpeed);
            _rb.linearVelocity = rollMovement;
        }
    }

    #endregion
    
}
