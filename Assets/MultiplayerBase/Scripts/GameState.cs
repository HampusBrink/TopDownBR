using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public abstract class GameState : MonoBehaviourPunCallbacks
{
    private static List<Player> _players;

    private static List<PlayerController> _playerControllers;
    public static List<Player> Players => _players;

    public static List<PlayerController> PlayerControllers => _playerControllers;

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        _players.Add(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player oldPlayer)
    {
        _players.Remove(oldPlayer);
    }
}