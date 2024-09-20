using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuManagerScript : MonoBehaviour
{
    public void LoadFoundSessionsScene()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public void LoadHostedSessionScene()
    {
        SceneManager.LoadScene("HostScene");
    }
}