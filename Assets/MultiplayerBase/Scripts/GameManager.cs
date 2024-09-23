using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private List<Transform> _spawnPositions;
    [SerializeField] private UIScript _UI;

    [SerializeField] private int _countDownTime = 20;

    public static GameManager Instance { get; private set; }

    public bool GameStarted { get; private set; }

    private bool _timerStarted;
    private float _timer;

    public float Timer => Mathf.Abs(_timer - _countDownTime);
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        var player = PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPositions[0].position, quaternion.identity);
        photonView.RPC(nameof(PlayerJoined),RpcTarget.MasterClient,player.GetPhotonView().ViewID);
    }

    void Update()
    {
        if (_timerStarted)
        {
            _timer += Time.deltaTime;

            if (_timer <= _countDownTime) return;
            _timer = 0;
            _timerStarted = false;
            GameStarted = true; 
            
            _UI.timerDisplay.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    void PlayerJoined(int playerID)
    {
        photonView.RPC(nameof(MovePlayer),RpcTarget.All,playerID,_spawnPositions[0].position);
        _spawnPositions.RemoveAt(0);
    }

    [PunRPC]
    void MovePlayer(int playerID,Vector3 newPos)
    {
        var player = PhotonView.Find(playerID);
        player.transform.position = newPos;
    }

    [PunRPC]
    public void StartGame()
    {
        _timerStarted = true;
        _UI.timerDisplay.gameObject.SetActive(true);
    }
    
    public void OnStartGameClicked()
    {
        photonView.RPC(nameof(StartGame),RpcTarget.All);
        _UI.startGameObject.SetActive(false);
    }
}
