using TMPro;
using UnityEngine;
public class DisplayRoomInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _playerAmount;

    /// <summary>
    /// Initializes the room button's text fields
    /// </summary>
    /// <param name="roomName"> name of the room</param>
    /// <param name="playerAmount"> amount of players currently in the room</param>
    /// <param name="maxPlayers"> max players allowed to join the room</param>
    public void Init(string roomName, int playerAmount, int maxPlayers)
    {
        _roomName.text = roomName;
        _playerAmount.text = playerAmount + "/" + maxPlayers;
    }
}
