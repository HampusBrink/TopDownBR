using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text name;
        [SerializeField] private RawImage profilePicture;

        public TMP_Text Name => name;
        public RawImage ProfilePicture => profilePicture;
    }
}