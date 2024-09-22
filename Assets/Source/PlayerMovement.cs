using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D),typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5;
    
    private PhotonView _pv;
    private Vector2 _moveVector;
    private Rigidbody2D _rb;
    private Camera _camera;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _pv = GetComponent<PhotonView>();
        _camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = _moveVector * _moveSpeed;
        
        // Get the mouse position in screen space
        Vector3 mouseScreenPosition = Input.mousePosition;

        // Convert the mouse position from screen space to world space using the Camera
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // Since this is a 2D top-down perspective, set z position to 0
        mouseWorldPosition.z = 0;

        // Get the direction from the object to the mouse
        Vector3 direction = mouseWorldPosition - transform.position;

        // Calculate the angle in degrees between the object's up direction and the direction to the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply the rotation to the object, adjusting it to face the mouse
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Correct for the 
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            _moveVector = Vector2.zero;
            return;
        }

        _moveVector = context.ReadValue<Vector2>();
    }
}
