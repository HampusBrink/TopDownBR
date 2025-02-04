using FishNet.Connection;
using FishNet.Object;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using NetworkRelated.Steam;
using Steamworks;
using UI;
using UnityEngine;

public class PlayerDisplayManager : NetworkBehaviour
{
    
    [SerializeField] private PlayerDisplay _playerDisplayItem;
    [SerializeField] private Transform _playerDisplayParent;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;


    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkManager.ClientManager.OnRemoteConnectionState += RemoteConnectionStarted;
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
        UpdatePlayersInLobby();
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t param)
    {
        UpdatePlayersInLobby();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        NetworkManager.ClientManager.OnRemoteConnectionState -= RemoteConnectionStarted;
    }

    private void RemoteConnectionStarted(RemoteConnectionStateArgs state)
    {
        if (state.ConnectionState == RemoteConnectionState.Started)
        {
            UpdatePlayersInLobby();
        }
    }

    void UpdatePlayersInLobby()
    {
        int lobbyMembers = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(SteamManager.Instance.CurrentLobbyID));
        
        _playerDisplayParent.DestroyChildren();
        
        for (int i = 0; i < lobbyMembers; i++)
        {
            var lobbyMember = SteamMatchmaking.GetLobbyMemberByIndex(new CSteamID(SteamManager.Instance.CurrentLobbyID),i);
            
            var playerDisplay = Instantiate(_playerDisplayItem, _playerDisplayParent);
            
            playerDisplay.Name.text = SteamFriends.GetFriendPersonaName(lobbyMember);
            
            var imageID = SteamFriends.GetLargeFriendAvatar(lobbyMember);
            
            playerDisplay.ProfilePicture.texture = GetSteamImageAsTexture(imageID);
        }
    }

    private Texture2D GetSteamImageAsTexture(int imageID)
    {
        Texture2D texture = null;

        if (SteamUtils.GetImageSize(imageID, out uint width, out uint height))
        {
            byte[] image = new byte[width * height * 4];

            if (SteamUtils.GetImageRGBA(imageID, image, (int)(width * height * 4)))
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        return texture;
    }

}
