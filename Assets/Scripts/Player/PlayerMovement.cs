 using System;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D),typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour
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

    [SerializeField] private Animator anim;
    private bool facingLeft = true;
    private Vector3 _initialScale;
    
    private Camera _camera;

    private PlayerStatus _playerStatus;
    private PhotonView _pv;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;
    
    void Start()
    {
        
        _rb = GetComponent<Rigidbody2D>();
        _pv = GetComponent<PhotonView>();
        _playerStatus = GetComponent<PlayerStatus>();
        _camera = Camera.main;
        _stamina = maxStamina;
        _initialScale = transform.localScale;

        if (!_pv.IsMine) staminaBarFill.transform.parent.gameObject.SetActive(false);

        if (_pv.IsMine && _camera) _camera.GetComponent<CameraMovement>().FollowTarget = transform;

        _desiredSpeed = _multipliedSpeed = walkSpeed;
        
        
    }

    private void Update()
    {
        if(!_pv.IsMine) return;
        if(!_camera) return;
        
        UpdateStamina();

        Animate();
        
        
        if (_input.x < 0)
            Flip(false);
        else if (_input.x > 0) 
            Flip(true);
        
        Foo();
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!_pv.IsMine) return;
        if(!_camera) return;

        if (!GameManager.Instance.PowerupPopup.gameObject.activeInHierarchy)
        {
            ApplyMovement();
        }

        //RotatePlayerToMouse();
    }

    private void UpdateMovementSpeed()
    {
        _desiredSpeed = _isSprinting && _stamina > 1 ? sprintSpeed : walkSpeed;
        _multipliedSpeed = _playerStatus.movementSpeedMultiplier * _desiredSpeed;
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        if(!_pv.IsMine) return;

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
        if(!_pv.IsMine) return;
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
    [HideInInspector] public Vector2 lastMovedirection;
    private bool _isMoving = false;

    private void Foo()
    {
        if (!_isMoving)
        {
            if (_input != Vector2.zero)
            {
                lastMovedirection = _input;
                _input = Vector2.zero;
            }
        }
        else
        {
            _input = _moveVector;
            _input.Normalize();
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        _isMoving = true;
        if (!_pv.IsMine) return;
        if (!GameManager.Instance.GameStarted) return;

        _moveVector = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            _isMoving = false;
        }
        
    }

    void Animate()
    {
        anim.SetFloat("MoveX", _input.x);
        anim.SetFloat("MoveY", _input.y);
        anim.SetFloat("MoveMagnitude", _input.magnitude);
        anim.SetFloat("LastMoveX", lastMovedirection.x);
        anim.SetFloat("LastMoveY", lastMovedirection.y);
    }

    public void Flip(bool flipToRight)
    {
        Vector3 scale = transform.localScale;
        if (flipToRight) 
            scale.x = _initialScale.x * 1f;
        else
            scale.x = _initialScale.x * -1f;
        
        transform.localScale = scale;

        facingLeft = !facingLeft;
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


    
    
    private void RotatePlayerToMouse()
    {
        Vector2 mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var direction = (mouseWorldPosition - _rb.position) * -1;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        playerGFX.transform.rotation = Quaternion.Euler(0f, 0f, angle);;
    }
}
