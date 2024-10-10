using System.Collections;
using FishNet.Object;
using MultiplayerBase.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class ZoneScript : NetworkBehaviour
{
    public float zoneDecreaseIntervalTime = 60f;
    public float zoneDamagePerSecond = 5f;
    public float zoneMoveSpeed = 0.01f;
    public Vector4 mapSize = new Vector4(-50,10,-10,100);
    
    private float _zoneTimer;
    private bool _decreaseZoneStart;
    private Vector2 _randomPoint;

    private float _zoneDamageTimer;
    private bool _isInSafeZone = true;
    
    private NetworkObject _playerNo;

    void Start()
    {
        StartCoroutine(CO_SetActive());
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
        UpdateZoneDamage();
        
        if(!GameManager.Instance.GameStarted) return;
        if(!IsServerInitialized) return;
        _zoneTimer += Time.deltaTime;

        if (_zoneTimer > zoneDecreaseIntervalTime && !_decreaseZoneStart)
        {
            _decreaseZoneStart = true;
            _randomPoint = new Vector2(Random.Range(mapSize.x, mapSize.y), Random.Range(mapSize.z, mapSize.w));
            _zoneTimer = 0;
        }
        if(_decreaseZoneStart) RPC_DecreaseZone(_randomPoint);
            
    }

    [ServerRpc(RequireOwnership = false)]
    void RPC_DecreaseZone(Vector2 randomPoint)
    {
        var oldPosition = transform.position;
        var oldScale = transform.localScale;
        
        transform.position = Vector3.Lerp(oldPosition, randomPoint,zoneMoveSpeed);
        transform.localScale -= oldScale * zoneMoveSpeed;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out NetworkObject no))
        {
            if(no.IsOwner)
            {
                _isInSafeZone = false;
                _playerNo = no;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out NetworkObject no))
        {
            if(no.IsOwner)
            {
                _isInSafeZone = true;
                _playerNo = no;
            }
        }
    }

    void UpdateZoneDamage()
    {
        if (!_playerNo) return;
        if (_isInSafeZone) return;
        
        _zoneDamageTimer += Time.deltaTime;
        if(_zoneDamageTimer < 1) return;
        _zoneDamageTimer = 0;
        if (_playerNo.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(zoneDamagePerSecond);
        }
    }
}
