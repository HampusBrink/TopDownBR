using System;
using FishNet.Managing;
using NetworkRelated.Steam;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField] private InputActionReference inputActionReference;
    [SerializeField] private GameObject content;

    private void OnEnable()
    {
        inputActionReference.action.performed += EscapePressed;
    }

    private void EscapePressed(InputAction.CallbackContext obj)
    {
        content.SetActive(!content.activeSelf);
    }

    private void OnDisable()
    {
        inputActionReference.action.performed -= EscapePressed;
    }

    public void OnBackToGamePressed()
    {
        content.SetActive(false);
    }
    
    public void OnOptionsPressed()
    {
        //TODO: Add Options
    }
    
    public void OnLeaveGamePressed()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(SteamManager.Instance.CurrentLobbyID));
        
        SteamManager.Instance.FishySteamworks.StopConnection(true);
        
        SteamManager.Instance.FishySteamworks.StopConnection(false);
        SceneManager.LoadScene("MainMenu");
    }
    
    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
