using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour, IDamagable
{
    public float maxHealth = 100;
    [SerializeField] private Image healthBarFill;

    [Header("Stats")]
    public float attackDamageMultiplier = 1.0f;
    public float attackSpeedMultiplier = 1.0f;
    public float weaponLengthMultiplier = 1.0f;
    
    public float movementSpeedMultiplier = 1.0f;
    
    [HideInInspector] public float bonusDamagePercent;
    private float _currentHealth;

    public bool IsDead { get; private set; }

    public float CurrentHealth => _currentHealth > maxHealth ? maxHealth : _currentHealth;

    public void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(float damage, int viewID)
    {
        _currentHealth -= damage;
        UpdateHealthBar();
        if (CurrentHealth <= 0) Die(viewID);
    }

    private void UpdateHealthBar()
    {
        float targetFillAmount = _currentHealth / maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
    }

    void Die(int viewID)
    {
        IsDead = true;
        if (PhotonView.Find(viewID).IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
        
        if(!PhotonNetwork.IsMasterClient) return;
        GameManager.Instance._players.Remove(gameObject.GetPhotonView());
        GameManager.Instance.CheckForWinner();
    }
    
    [PunRPC]
    public void RPC_TakeDamage(int viewID,float damage)
    {
        var player = PhotonView.Find(viewID);
        if(!player.TryGetComponent(out IDamagable damagable)) return;
        damagable.TakeDamage(damage,viewID);
    }
}
