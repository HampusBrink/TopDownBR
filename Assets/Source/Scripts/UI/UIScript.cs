using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UIScript : MonoBehaviour
{
    public TMP_Text timerDisplay;
    public GameObject startGameObject;

    void Start()
    {
        timerDisplay.gameObject.SetActive(false);
        if(GameManager.Instance.photonView.IsMine) return;
        startGameObject.SetActive(false);
    }

    void Update()
    {
        var roundedValue = (int)GameManager.Instance.Timer;
        timerDisplay.text = roundedValue.ToString();
    }

    public void DisableSelf(GameObject self)
    {
        self.SetActive(false);
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("StartScene");
    }
}
