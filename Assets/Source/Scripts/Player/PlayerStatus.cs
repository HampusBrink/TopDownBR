using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using MultiplayerBase.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Source.Scripts.Player
{
    public class PlayerStatus : NetworkBehaviour, IDamagable
    {
        [SerializeField] private Image healthBarFill;

        [Header("Stats")] public float maxHealth = 100f;
        public float attackDamageMultiplier = 1.0f;
        public float attackSpeedMultiplier = 1.0f;
        public float weaponLengthMultiplier = 1.0f;
        public float movementSpeedMultiplier = 1.0f;

        [AllowMutableSyncType]
        private SyncVar<float> _currentHealth;

        public bool IsDead { get; private set; }

        private NetworkObject _no;

        public float CurrentHealth
        {
            get => _currentHealth.Value > maxHealth ? maxHealth : _currentHealth.Value;
            set => _currentHealth.Value = value > maxHealth ? maxHealth : value;
        }

        private void Awake()
        {
            _currentHealth.OnChange += OnHealthChange;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
        
            CurrentHealth = maxHealth;
            _no = GetComponent<NetworkObject>();
            if (IsOwner)
            {
                healthBarFill.color = Color.green;
            }
        }


        private void OnHealthChange(float prev, float next, bool asserver)
        {
            CurrentHealth = next;
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            UpdateHealthBar();
            if (CurrentHealth <= 0) Die();
        }

        public void Heal(float healAmount)
        {
            CurrentHealth += healAmount;
            UpdateHealthBar();
        }

        private void UpdateHealthBar()
        {
            float targetFillAmount = CurrentHealth / maxHealth;
            healthBarFill.fillAmount = targetFillAmount;
        }

        void Die()
        {
            if (_no.IsOwner)
            {
                GameManager.Instance.isDead = true;
                GameManager.Instance.PlayerDied();
                _no.Despawn();
            }

            if (!_no.IsServerInitialized) return;
            // GameManager.Instance.players.Remove(gameObject.GetPhotonView());
            GameManager.Instance.CheckForWinner();
        }
    }
}