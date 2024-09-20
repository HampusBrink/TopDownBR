using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement Stats")]
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _jumpForce = 250;
    
    [Header("Variables")]
    [SerializeField] private float _groundDistance = 0.1f;
    [SerializeField] private float _feetDistance = 1.2f;
    [SerializeField] private Transform _gfx;
    
    [Header("Attack Stats")]
    [SerializeField] private Vector2 _attackHitBoxSize;
    [SerializeField] private Transform _attackTransform;
    [SerializeField] private int _damage = 100;
    [SerializeField] private Vector2 _hitAngleNormalized = new (1,1);

    
    private float _moveX;
    private Rigidbody2D _rb;
    private PhotonView _playerPv;
    private bool _isFacingRight;
    private bool _isInHitStun;

    public PhotonView PlayerPv => _playerPv;

    private void Awake()
    {
        _playerPv = GetComponent<PhotonView>();
        _rb = GetComponent<Rigidbody2D>();
        _hitAngleNormalized.x = -_hitAngleNormalized.x;
        //AddPlayerList();
    }
    private void AddPlayerList()
    {
        if(!_playerPv.IsMine) return;
        GameState.PlayerControllers.Add(this);
    }
    private void Update()
    {
        if(!_playerPv.IsMine) return;
        if (!_isInHitStun)
        {
            _rb.velocity = new Vector2(_moveX * _moveSpeed, _rb.velocity.y);
        }
    }
    private bool GroundCheck()
    {
        return Physics2D.OverlapCircle(transform.position - new Vector3(0,_feetDistance,0), _groundDistance);
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if(!_playerPv.IsMine) return;
        _moveX = context.ReadValue<Vector2>().x;
        if (!_isFacingRight && _moveX > 0f) 
            Flip();
        else if (_isFacingRight && _moveX < 0f)
            Flip();
    }

    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 localScale = _gfx.localScale;
        localScale.x *= -1f;
        _gfx.localScale = localScale;
        _hitAngleNormalized.x *= -1f;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(!_playerPv.IsMine) return;
        if(context.performed && GroundCheck())
            _rb.AddForce(new Vector2(0, _jumpForce));
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!_playerPv.IsMine) return;
        if (context.performed)
        {
            var hits = Physics2D.OverlapBoxAll(_attackTransform.position, _attackHitBoxSize,0);
            print(hits);
            for (var i = 0; i < hits.Length; i++)
            {
                if (hits[i].TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(_damage, _hitAngleNormalized.normalized);
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(_attackTransform.position,_attackHitBoxSize);
    }
    public void TakeDamage(int damage, Vector2 direction)
    {
        _playerPv.RPC(nameof(RPC_TakeDamage),RpcTarget.All,damage,direction);
    }

    [PunRPC]
    public void RPC_TakeDamage(int damage, Vector2 direction)
    {
        if(!_playerPv.IsMine) return;
        _isInHitStun = true;
        _rb.AddForce(direction * damage);
        StartCoroutine(CO_HitStunDuration());
    }

    IEnumerator CO_HitStunDuration()
    {
        yield return new WaitForSeconds(2);
        _isInHitStun = false;
    }
}