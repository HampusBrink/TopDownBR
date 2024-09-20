using UnityEngine;
public class CameraScript : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    private void Awake()
    {
        gameObject.SetActive(_playerController.PlayerPv.IsMine);
    }
}