using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D),typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5;
    
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
    void Update()
    {
        if(!_pv.IsMine) return;
        if(!_camera) return;
        
        _rb.velocity = _moveVector * _moveSpeed;
        
        Vector2 mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        var direction = mouseWorldPosition - _rb.position;

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

        _rb.rotation = angle;
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
}
