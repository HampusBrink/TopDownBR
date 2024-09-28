using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D),typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour
{
    
    
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 22f;
    [SerializeField] private float acceleration = 5f;

    [SerializeField] private GameObject playerGFX;

    private Vector2 _velocity = Vector2.zero;
    private float _desiredSpeed;
    private bool _isSprinting = false;
    
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

        if (_pv.IsMine && _camera) _camera.GetComponent<CameraMovement>().FollowTarget = transform;

        _desiredSpeed = walkSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!_pv.IsMine) return;
        if(!_camera) return;
        
        ApplyMovement();
        
        RotatePlayerToMouse();
    }

    private void UpdateMovementSpeed()
    {
        _desiredSpeed *= _playerStatus.movementSpeedMultiplier;
    }
    
    public void OnSprint(InputAction.CallbackContext context)
    {
        UpdateMovementSpeed();
        
        if (context.performed)
        {
            _isSprinting = true;
        }
        else if (context.canceled)
        {
            _isSprinting = false;
        }

        _desiredSpeed = _isSprinting ? sprintSpeed : walkSpeed;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if(!_pv.IsMine) return;
        if(!GameManager.Instance.GameStarted) return;
        if (!context.performed)
        {
            _moveVector = Vector2.zero;
            return;
        }

        _moveVector = context.ReadValue<Vector2>();
    }

    private void ApplyMovement()
    {
        UpdateMovementSpeed();
        if (_moveVector != Vector2.zero)
        {
            _velocity = Vector2.MoveTowards(_velocity, _moveVector * _desiredSpeed, acceleration * Time.fixedDeltaTime);
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
