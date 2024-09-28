using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(PhotonView))]
public class ZoneScript : MonoBehaviour
{
    public float zoneDecreaseIntervalTime = 60f;
    public int zoneDamagePerSecond = 5;
    public float zoneMoveSpeed = 0.01f;
    public Vector2 mapSize = new Vector2(100,100);
    
    private float _zoneTimer;
    private bool _decreaseZoneStart;
    private Vector2 _randomPoint;

    private float _zoneDamageTimer;
    private bool _isInSafeZone = true;

    private PhotonView _pv;
    private PhotonView _playerPv;
    
    void Start()
    {
        StartCoroutine(CO_SetActive());
        _pv = GetComponent<PhotonView>();
    }

    IEnumerator CO_SetActive()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    // Update is called once per frame 
    void Update()
    {
        if(!GameManager.Instance.GameStarted) return;
        if(!PhotonNetwork.IsMasterClient) return;
        _zoneTimer += Time.deltaTime;

        if (_zoneTimer > zoneDecreaseIntervalTime && !_decreaseZoneStart)
        {
            _decreaseZoneStart = true;
            _randomPoint = new Vector2(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));
            _zoneTimer = 0;
        }
        if(_decreaseZoneStart)
            _pv.RPC(nameof(RPC_DecreaseZone),RpcTarget.All,_randomPoint);
        
        if (!_isInSafeZone)
        {
            _zoneDamageTimer += Time.deltaTime;
            if(_zoneDamageTimer < 1) return;
            _zoneTimer = 0;
            if (_playerPv.TryGetComponent(out IDamagable damagable))
            {
                _playerPv.RPC(nameof(damagable.RPC_TakeDamage),RpcTarget.All,_playerPv.ViewID,zoneDamagePerSecond);
            }
        }
    }

    [PunRPC]
    void RPC_DecreaseZone(Vector2 randomPoint)
    {
        var oldPosition = transform.position;
        var oldScale = transform.localScale;
        
        transform.position = Vector3.Lerp(oldPosition, randomPoint,zoneMoveSpeed);
        transform.localScale -= oldScale * zoneMoveSpeed;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out PhotonView pv))
        {
            if(pv.IsMine)
            {
                _isInSafeZone = false;
                _playerPv = pv;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PhotonView pv))
        {
            if(pv.IsMine)
            {
                _isInSafeZone = true;
                _playerPv = pv;
            }
        }
    }
}
