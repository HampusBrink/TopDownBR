using System;
using FishNet.Object;
using MultiplayerBase.Scripts;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
    public class PlayerStatus : NetworkBehaviour, IDamagable
    {
        // Levels
        private int _playerLevel = 0;
        public int PlayerLevel => _playerLevel;
        private float _playerExp = 0;
        public float baseExpCap = 1000f;
        
        
        
        [SerializeField] private Image healthBarFill;
        public CapsuleCollider hitBox;
        [SerializeField] private PlayerCombat playerCombat;
        
        [Header("Generic Upgrades")] 
        public VitalUpgrades vitalUpgrades;
        public MovementUpgrades movementUpgrades;
        public CombatUpgrades combatUpgrades;

        [System.Serializable]
        public class VitalUpgrades
        {
            public float maxHealth = 100f;
        }

        [System.Serializable]
        public class MovementUpgrades
        {
            public float movementSpeedMultiplier = 1.0f;
        }
    
        [System.Serializable]
        public class CombatUpgrades
        {
            public float attackDamageMultiplier = 1.0f;
            public float attackRangeMultiplier = 1.0f;
            public float attackSpeedMultiplier = 1.0f;
        }
        
        

        private float _currentHealth;

        public bool IsDead { get; private set; }

        public float CurrentHealth
        {
            get => _currentHealth > vitalUpgrades.maxHealth ? vitalUpgrades.maxHealth : _currentHealth;
            set => _currentHealth = value > vitalUpgrades.maxHealth ? vitalUpgrades.maxHealth : value;
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();

            CurrentHealth = vitalUpgrades.maxHealth;
            if (IsOwner)
            {
                GameManager.Instance.SRPC_PlayerJoined(this);
                healthBarFill.color = Color.green;
            }
        }

        private void LevelUp()
        {
            _playerLevel++;
            if (_playerLevel % 5 == 0)
            {
                // weapon specific
            }
            else
            {
                // generic
                GameManager.Instance.upgradePopup.gameObject.SetActive(true);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                AddExp(1000f);
                Debug.Log("Current Level:" + _playerLevel);
                Debug.Log("Current Exp:" + _playerExp);
            }
        }

        public void AddExp(float amount)
        {
            _playerExp += amount;
            
            while (_playerExp >= GetExpCapForLevel(_playerLevel + 1))
            {
                _playerExp -= GetExpCapForLevel(_playerLevel + 1); 
                LevelUp();
            }
        }

        private float GetExpCapForLevel(int level)
        {
            return baseExpCap * Mathf.Pow(1.1f, level);
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
            float targetFillAmount = CurrentHealth / vitalUpgrades.maxHealth;
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