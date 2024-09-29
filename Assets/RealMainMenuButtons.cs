using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RealMainMenuButtons : MonoBehaviour
{
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
    
    
}
