using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
public class FindSessions : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject _rooms;
    [SerializeField] private GameObject _roomButton;
    
    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ListRooms(roomList);
    }

    private void ListRooms(List<RoomInfo> roomList)
    {
        ClearRooms();
        foreach (var room in roomList)
        {
            var listedRoomButton = Instantiate(_roomButton, _rooms.transform);
            listedRoomButton.GetComponentInChildren<DisplayRoomInfo>().Init(room.Name, room.PlayerCount, room.MaxPlayers);
        }
    }

    private void ClearRooms()
    {
        foreach (var child in _rooms.GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.Equals(_rooms)) continue;
            Destroy(child.gameObject);
        }
    }
}