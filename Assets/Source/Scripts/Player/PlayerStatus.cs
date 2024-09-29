using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour, IDamagable
{
    [SerializeField] private Image healthBarFill;

    [Header("Stats")] 
    public float maxHealth = 100f;
    public float attackDamageMultiplier = 1.0f;
    public float attackSpeedMultiplier = 1.0f;
    public float weaponLengthMultiplier = 1.0f;
    public float movementSpeedMultiplier = 1.0f;

    private float _currentHealth;

    public bool IsDead { get; private set; }

    public float CurrentHealth
    {
        get => _currentHealth > maxHealth ? maxHealth : _currentHealth;
        set => _currentHealth = value > maxHealth ? maxHealth : value;
    }

    public PhotonView pv;

    public void Start()
    {
        _currentHealth = maxHealth;
        pv = GetComponent<PhotonView>();
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
        float targetFillAmount = _currentHealth / maxHealth;
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