using MultiplayerBase.Scripts;
using TMPro;
using UnityEngine;

namespace Source.Scripts.UI
{
    public class UIScript : MonoBehaviour
    {
        public TMP_Text timerDisplay;
        public GameObject startGameObject;

        private void Start()
        {
            if(GameManager.Instance.IsServerInitialized) startGameObject.SetActive(true);
        }

        void Update()
        {
            if(GameManager.Instance.IsServerInitialized) startGameObject.SetActive(true);
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
