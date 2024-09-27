using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D),typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour, IDamagable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 5f;

    [SerializeField] private GameObject playerGFX;

    private Vector2 _velocity = Vector2.zero;
    private Camera _camera;
    
    private PhotonView _pv;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _pv = GetComponent<PhotonView>();
        _camera = Camera.main;

        if (_pv.IsMine && _camera) _camera.GetComponent<CameraMovement>()._followTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!_pv.IsMine) return;
        if(!_camera) return;
        
        ApplyMovement();
        
        RotatePlayerToMouse();
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
        if (_moveVector != Vector2.zero)
        {
            _velocity = Vector2.MoveTowards(_velocity, _moveVector * moveSpeed, acceleration * Time.fixedDeltaTime);
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
        var direction = mouseWorldPosition - _rb.position;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;
        playerGFX.transform.rotation = Quaternion.Euler(0f, 0f, angle);;
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
