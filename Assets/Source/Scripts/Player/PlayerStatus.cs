using System;
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

        private float _currentHealth;

        public bool IsDead { get; private set; }

        public float CurrentHealth
        {
            get => _currentHealth > maxHealth ? maxHealth : _currentHealth;
            set => _currentHealth = value > maxHealth ? maxHealth : value;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            CurrentHealth = maxHealth;
            if (IsOwner)
            {
                GameManager.Instance.SRPC_PlayerJoined(this);
                healthBarFill.color = Color.green;
            }
        }


        [ServerRpc(RequireOwnership = false)]
        public void TakeDamage(float damage)
        {
            RPC_TakeDamage(damage);

        }

        [ObserversRpc]
        private void RPC_TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            UpdateHealthBar();
            
            if (CurrentHealth <= 0 && IsOwner) GameManager.Instance.isDead = true;
            if (CurrentHealth <= 0 && IsServerInitialized) Die();
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
            GameManager.Instance.SRPC_PlayerDied(this);
            Despawn();
            
            // GameManager.Instance.players.Remove(gameObject.GetPhotonView());
            GameManager.Instance.CheckForWinner();
        }
    }
}