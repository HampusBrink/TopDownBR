using System.Collections.Generic;
using System.Linq;
using MultiplayerBase.Scripts;
using Photon.Pun;
using Photon.Realtime;
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
    public PowerupPopup PowerupPopup;

    [SerializeField] private int _countDownTime = 20;

    public static GameManager Instance { get; private set; }
    [HideInInspector] public PhotonView _localPlayer;
    public bool isDead;
    public List<PhotonView> _players;
    public List<PlayerMovement> _alivePlayers;

    public bool GameStarted { get; private set; }
    
    //remove later
    public bool IsTestScene;

    private bool _timerStarted;
    private float _timer;

    public float Timer => Mathf.Abs(_timer - _countDownTime);
    
    void Awake()
    {
        if (IsTestScene) GameStarted = true;
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

    public void PlayerDied()
    {
        _alivePlayers = FindObjectsOfType<PlayerMovement>().ToList();
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            CheckQueue();
        }
    }

    private void CheckQueue()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(MainMenuManagerScript.QUEUE_PROP_KEY, out object queueObj))
        {
            string[] queue = queueObj as string[];
            if (queue != null && queue.Length > 0)
            {
                string nextPlayerId = queue[0];
                // Remove the player from the queue
                queue = queue.Skip(1).ToArray();
                ExitGames.Client.Photon.Hashtable queueProperty = new ExitGames.Client.Photon.Hashtable();
                queueProperty[MainMenuManagerScript.QUEUE_PROP_KEY] = queue;
                PhotonNetwork.CurrentRoom.SetCustomProperties(queueProperty);

                // Invite the next player
                PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(new string[] { "OpenForNext" });
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
        }
    }
}
