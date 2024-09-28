using Photon.Pun;
using UnityEngine;

public class PlayerStatus : MonoBehaviour, IDamagable
{
    public float maxHealth = 100;

    [HideInInspector] public float bonusDamagePercent;
    private float _currentHealth;

    public float CurrentHealth => _currentHealth > maxHealth ? maxHealth : _currentHealth;

    public void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        if (CurrentHealth <= 0) Die();
    }

    void Die()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        GameManager.Instance._players.Remove(gameObject.GetPhotonView());
        PhotonNetwork.Destroy(gameObject);
        GameManager.Instance.CheckForWinner();
    }
    
    [PunRPC]
    public void RPC_TakeDamage(int viewID,float damage)
    {
        var player = PhotonView.Find(viewID);
        if(!player.TryGetComponent(out IDamagable damagable)) return;
        damagable.TakeDamage(damage);
    }
}
