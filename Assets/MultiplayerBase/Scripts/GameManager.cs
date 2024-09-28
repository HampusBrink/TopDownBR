using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static System.String;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private List<Transform> _spawnPositions;
    [SerializeField] private UIScript _UI;
    [SerializeField] private TMP_Text _victoryRoyaleUI;

    [SerializeField] private int _countDownTime = 20;

    public static GameManager Instance { get; private set; }
    public PhotonView _localPlayer;
    public List<PhotonView> _players;

    public bool GameStarted { get; private set; }

    private bool _timerStarted;
    private float _timer;

    public float Timer => Mathf.Abs(_timer - _countDownTime);
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if(GameStarted) return;
        var player = PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPositions[0].position, quaternion.identity);
        _localPlayer = player.GetPhotonView();
        photonView.RPC(nameof(PlayerJoined),RpcTarget.MasterClient,player.GetPhotonView().ViewID);
    }

    void Update()
    {
        print(_players.Count);
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
        _players.Add(PhotonView.Find(playerID));
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

    public void CheckForWinner()
    {
        if (_players.Count == 1)
        {
            photonView.RPC(nameof(DisplayVictor),RpcTarget.All,_players[0].Controller.NickName);
            print(_players[0].Controller.NickName);
        }
    }

    [PunRPC]
    void DisplayVictor(string victorName)
    {
        if (victorName == Empty)
        {
            victorName = "Unknown Player";
        }
        _victoryRoyaleUI.text = victorName + " Wins!";
        _victoryRoyaleUI.gameObject.SetActive(true);
    }
}
