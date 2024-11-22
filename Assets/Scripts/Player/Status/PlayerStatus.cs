using System;
using FishNet.Object;
using MultiplayerBase.Scripts;
using NetworkRelated;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

namespace Player
{
    public class PlayerStatus : AdvancedNetworkBehaviour, IDamagable
    {
        // Levels
        [Header("Experience / Levels")] [SerializeField]
        AnimationCurve experienceCurve;

        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI experienceText;
        [SerializeField] Image experienceBarFill;
        [SerializeField] private int debugExpAddAmount = 10;

        int _currentLevel = 1, _totalExperience;
        int _previousLevelsExperience, _nextLevelsExperience;

        [Header("Other")] [SerializeField] private Image healthBarFill;
        public CapsuleCollider hitBox;
        [SerializeField] private PlayerCombat playerCombat;

        [Header("Generic Upgrades")] public VitalUpgrades vitalUpgrades;
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
                GameManager.Instance.localPlayer = this;
            }
        }

        private void Start()
        {
            InitializeLevel();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L)) // add exp
            {
                AddExperience(debugExpAddAmount);
            }
        }

        // Experience / Levels
        private void InitializeLevel()
        {
            _currentLevel = 1;
            _totalExperience = 0;
            UpdateLevel();
            UpdateLevelUI();
        }

        public void AddExperience(int amount)
        {
            if (_currentLevel >= experienceCurve.keys[experienceCurve.length - 1].time)
                return; // Stop adding experience if already at max level

            _totalExperience += amount;
            CheckForLevelUp();
            UpdateLevelUI();
        }

        private void CheckForLevelUp()
        {
            // Get the max level from the curve
            int maxLevel = (int)experienceCurve.keys[experienceCurve.length - 1].time;

            // Check if we're already at the max level
            if (_currentLevel >= maxLevel)
            {
                _currentLevel = maxLevel;
                _totalExperience = _nextLevelsExperience;
                UpdateLevel();
                UpdateLevelUI();
                return;
            }

            // Level up if experience exceeds the threshold for the current level
            while (_totalExperience >= _nextLevelsExperience)
            {
                _currentLevel++;

                // Stop leveling up if we've reached max level
                if (_currentLevel >= maxLevel)
                {
                    _currentLevel = maxLevel;
                    _totalExperience = _nextLevelsExperience;
                    UpdateLevel();
                    UpdateLevelUI();
                    return;
                }

                UpdateLevel();
            }
        }

        private void UpdateLevel()
        {
            _previousLevelsExperience = (int)experienceCurve.Evaluate(_currentLevel);

            // Get the max level from the curve
            int maxLevel = (int)experienceCurve.keys[experienceCurve.length - 1].time;

            // Check if we are at the max level
            if (_currentLevel < maxLevel)
            {
                _nextLevelsExperience = (int)experienceCurve.Evaluate(_currentLevel + 1);
            }
            else
            {
                // Cap experience at max level
                _nextLevelsExperience = _previousLevelsExperience;
            }
        }

        private void UpdateLevelUI()
        {
            // Get the max level from the curve
            int maxLevel = (int)experienceCurve.keys[experienceCurve.length - 1].time;

            // Check if we're at max level
            if (_currentLevel >= maxLevel)
            {
                levelText.text = _currentLevel.ToString();
                experienceText.text = "Max Level";
                experienceBarFill.fillAmount = 1f;
                return;
            }

            int start = _totalExperience - _previousLevelsExperience;
            int end = _nextLevelsExperience - _previousLevelsExperience;
            levelText.text = _currentLevel.ToString();
            experienceText.text = $"{start} / {end} exp";
            experienceBarFill.fillAmount = (float)start / end;
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