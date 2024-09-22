using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _playerPrefab;
    void Start()
    {
        PhotonNetwork.Instantiate(_playerPrefab.name, new Vector3(0, 0, 0), quaternion.identity);
    }
}
