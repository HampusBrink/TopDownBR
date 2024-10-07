using FishNet.Object;
using MultiplayerBase.Scripts;
using TMPro;
using UnityEngine;

namespace Source.Scripts.UI
{
    public class UIScript : NetworkBehaviour
    {
        public TMP_Text timerDisplay;
        public GameObject startGameObject;
    
    
        public override void OnStartServer()
        {
            base.OnStartServer();
        
            startGameObject.SetActive(true);
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
        
        }
    }
}
