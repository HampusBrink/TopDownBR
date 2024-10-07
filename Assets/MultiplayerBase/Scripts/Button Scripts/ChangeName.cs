using TMPro;
using UnityEngine;
public class ChangeName : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameInputField;
    
    public void ChangePlayerName()
    {
        //PhotonNetwork.NickName = _nameInputField.text;
    }
}