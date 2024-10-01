 using System;
using UnityEngine;
using Photon.Pun;
using Unity.Mathematics;
using UnityEditor.Timeline;
using UnityEngine.InputSystem;
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

        if (!_pv.IsMine) staminaBarFill.transform.parent.gameObject.SetActive(false);

        if (_pv.IsMine && _camera) _camera.GetComponent<CameraMovement>().FollowTarget = transform;

        _desiredSpeed = _multipliedSpeed = walkSpeed;
        
        
    }

    private void Update()
    {
        if(!_pv.IsMine) return;
        if(!_camera) return;
        
        UpdateStamina();

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(GetTurnDirection());
        }

        Animate();
        
        
        if (input.x < 0 && !facingLeft || input.x > 0 && facingLeft)
        {
            Flip();
        }
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

    private Vector2 input;
    private Vector2 lastMovedirection;
    private bool isMoving = false;

    private void Foo()
    {
        if (!isMoving)
        {
            if (input != Vector2.zero)
            {
                lastMovedirection = input;
                input = Vector2.zero;
            }
        }
        else
        {
            input = _moveVector;
            input.Normalize();
        }
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        isMoving = true;
        if (!_pv.IsMine) return;
        if (!GameManager.Instance.GameStarted) return;

        _moveVector = context.ReadValue<Vector2>();

        if (context.canceled)
        {
            isMoving = false;
        }
        
    }

    void Animate()
    {
        anim.SetFloat("MoveX", input.x);
        anim.SetFloat("MoveY", input.y);
        anim.SetFloat("MoveMagnitude", input.magnitude);
        anim.SetFloat("LastMoveX", lastMovedirection.x);
        anim.SetFloat("LastMoveY", lastMovedirection.y);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
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


    enum TurnDirection
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

    private TurnDirection GetTurnDirection()
    {
        Vector2 mouseScreenPosition = Input.mousePosition;
        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 normalizedDirection = new Vector2(
            (mouseScreenPosition.x - screenCenter.x) / Screen.width,
            (mouseScreenPosition.y - screenCenter.y) / Screen.height
        );
    
        var angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg + 90;
        angle = (angle + 360) % 360;

        int directionIndex = Mathf.RoundToInt(angle / 45f) % 8;
        return (TurnDirection)directionIndex;
    }
    
    private void RotatePlayerToMouse()
    {
        Vector2 mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var direction = (mouseWorldPosition - _rb.position) * -1;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        playerGFX.transform.rotation = Quaternion.Euler(0f, 0f, angle);;
    }
}
