using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour, IDamagable
{
    public float maxHealth = 100;
    [SerializeField] private Image healthBarFill;
    
    
    [HideInInspector] public float bonusDamagePercent;
    private float _currentHealth;

    public bool IsDead { get; private set; }

    public float CurrentHealth => _currentHealth > maxHealth ? maxHealth : _currentHealth;

    public void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        UpdateHealthBar();
        if (CurrentHealth <= 0) Die();
    }

    private void UpdateHealthBar()
    {
        float targetFillAmount = _currentHealth / maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
    }

    void Die()
    {
        IsDead = true;
        if(!PhotonNetwork.IsMasterClient) return;
        GameManager.Instance._players.Remove(gameObject.GetPhotonView());
        GameManager.Instance.CheckForWinner();
        PhotonNetwork.Destroy(gameObject);
    }
    
    [PunRPC]
    public void RPC_TakeDamage(int viewID,float damage)
    {
        var player = PhotonView.Find(viewID);
        if(!player.TryGetComponent(out IDamagable damagable)) return;
        damagable.TakeDamage(damage);
    }
}
