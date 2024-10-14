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
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 22f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] public float maxStamina = 100f;
    [SerializeField] public float staminaDrain = 2f;
    

    [SerializeField] private GameObject playerGFX;
    [SerializeField] private Image staminaBarFill;

    private Vector2 _velocity = Vector2.zero;
    private float _desiredSpeed;
    private float _multipliedSpeed;
    private bool _isSprinting = false;
    private float _stamina;

    [SerializeField] private Animator bodyAnim;
    [SerializeField] private Animator legsAnim;
    private bool facingLeft = true;
    private Vector3 _initialScale;
    
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
        _initialScale = transform.localScale;

        if (!IsOwner) staminaBarFill.transform.parent.gameObject.SetActive(false);

        if (IsOwner && _camera) _camera.GetComponent<CameraMovement>().FollowTarget = transform;

        _desiredSpeed = _multipliedSpeed = walkSpeed;
    }

    private void Update()
    {
        if(!IsOwner) return;
        if(!_camera) return;
        
        UpdateStamina();

        Animate();
        
        UpdateMoveDirection();
    }
    

    void FixedUpdate()
    {
        if(!IsOwner) return;
        if(!_camera) return;

        if (!GameManager.Instance.powerupPopup.gameObject.activeInHierarchy)
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
        if(!IsOwner) return;

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
        if(!IsOwner) return;
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
        isMoving = true;
        if (!IsOwner) return;
        if (!GameManager.Instance.GameStarted) return;

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

    
}