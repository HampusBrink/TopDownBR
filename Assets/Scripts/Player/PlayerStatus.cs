using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour, IDamagable
{
    [SerializeField] private Image healthBarFill;

    [Header("Stats")] 
    public PlayerStatMultipliers playerStatMultipliers;
    public MovementStatMultipliers movementStatMultipliers;
    public WeaponStatMultipliers weaponStatMultipliers;
    

    private float _currentHealth;

    public bool IsDead { get; private set; }

    public float CurrentHealth
    {
        get => _currentHealth > playerStatMultipliers.maxHealth ? playerStatMultipliers.maxHealth : _currentHealth;
        set => _currentHealth = value > playerStatMultipliers.maxHealth ? playerStatMultipliers.maxHealth : value;
    }

    public PhotonView pv;

    [System.Serializable]
    public class PlayerStatMultipliers
    {
        public float maxHealth = 100f;
    }

    [System.Serializable]
    public class MovementStatMultipliers
    {
        public float movementSpeedMultiplier = 1.0f;
    }
    
    [System.Serializable]
    public class WeaponStatMultipliers
    {
        public float attackDamageMultiplier = 1.0f;
        public float attackRangeMultiplier = 1.0f;
        public float attackSpeedMultiplier = 1.0f;
    }
    
    public void Start()
    {
        _currentHealth = playerStatMultipliers.maxHealth;
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            healthBarFill.color = Color.green;
        }
    }

    public void TakeDamage(float damage, int viewID)
    {
        _currentHealth -= damage;
        UpdateHealthBar();
        if (CurrentHealth <= 0) Die(viewID);
    }

    [PunRPC]
    public void Heal(float healAmount)
    {
        _currentHealth += healAmount;
        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        float targetFillAmount = _currentHealth / playerStatMultipliers.maxHealth;
        healthBarFill.fillAmount = targetFillAmount;
    }

    void Die(int viewID)
    {
        if (PhotonView.Find(viewID).IsMine)
        {
            GameManager.Instance.isDead = true;
            GameManager.Instance.PlayerDied();
            PhotonNetwork.Destroy(gameObject);
        }

        if (!PhotonNetwork.IsMasterClient) return;
        GameManager.Instance._players.Remove(gameObject.GetPhotonView());
        GameManager.Instance.CheckForWinner();
    }

    [PunRPC]
    public void RPC_TakeDamage(int viewID, float damage)
    {
        var player = PhotonView.Find(viewID);
        if (!player.TryGetComponent(out IDamagable damagable)) return;
        damagable.TakeDamage(damage, viewID);
    }
}