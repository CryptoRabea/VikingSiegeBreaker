using UnityEngine;
using System;

namespace VikingSiegeBreaker.Entities
{
    /// <summary>
    /// Generic enemy behavior - handles HP, damage, knockback, rewards, and tier scaling.
    /// Enemy difficulty scales with distance traveled.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Enemy : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("Enemy Data")]
        [SerializeField] private Data.EnemyData enemyData; // ScriptableObject reference

        [Header("Stats (Runtime)")]
        [SerializeField] private float maxHealth = 50f;
        [SerializeField] private float currentHealth = 50f;
        [SerializeField] private float contactDamage = 10f;
        [SerializeField] private float momentumPenaltyMultiplier = 1f;
        [SerializeField] private int tier = 1;

        [Header("Rewards")]
        [SerializeField] private int coinReward = 5;
        [SerializeField] private int xpReward = 2;
        [SerializeField] private float dropChance = 0.2f; // 20% chance to drop pickup

        [Header("Knockback")]
        [SerializeField] private float knockbackResistance = 1f;
        [SerializeField] private bool canBeKnockedBack = true;

        [Header("VFX")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private GameObject damageNumberPrefab;

        // Properties
        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float NormalizedHealth => currentHealth / maxHealth;
        public float MomentumPenaltyMultiplier => momentumPenaltyMultiplier;
        public int Tier => tier;

        // Events
        public event Action<Enemy> OnDeath;

        private bool isDead = false;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // Initialize from data
            if (enemyData != null)
            {
                InitializeFromData();
            }

            // Scale based on distance (difficulty curve)
            ScaleWithDistance();
        }

        #region Initialization

        /// <summary>
        /// Initializes stats from ScriptableObject data.
        /// </summary>
        private void InitializeFromData()
        {
            maxHealth = enemyData.baseHealth;
            currentHealth = maxHealth;
            contactDamage = enemyData.contactDamage;
            momentumPenaltyMultiplier = enemyData.momentumPenalty;
            knockbackResistance = enemyData.knockbackResistance;
            coinReward = enemyData.coinReward;
            xpReward = enemyData.xpReward;
            dropChance = enemyData.dropChance;
            tier = enemyData.tier;
        }

        /// <summary>
        /// Scales enemy stats based on current game distance (difficulty curve).
        /// </summary>
        private void ScaleWithDistance()
        {
            float distance = Core.GameManager.Instance.CurrentDistance;

            // Scale health: +5% per 100m
            float healthScale = 1f + (distance / 100f) * 0.05f;
            maxHealth *= healthScale;
            currentHealth = maxHealth;

            // Scale damage: +3% per 100m
            float damageScale = 1f + (distance / 100f) * 0.03f;
            contactDamage *= damageScale;

            // Scale rewards: +10% per 100m
            float rewardScale = 1f + (distance / 100f) * 0.1f;
            coinReward = Mathf.RoundToInt(coinReward * rewardScale);
            xpReward = Mathf.RoundToInt(xpReward * rewardScale);

            Debug.Log($"[Enemy] Scaled for distance {distance:F2}m - HP: {maxHealth}, DMG: {contactDamage}");
        }

        /// <summary>
        /// Sets enemy tier manually (for spawn system).
        /// </summary>
        public void SetTier(int newTier)
        {
            tier = newTier;
            float tierMultiplier = 1f + (tier - 1) * 0.5f; // 50% increase per tier

            maxHealth *= tierMultiplier;
            currentHealth = maxHealth;
            contactDamage *= tierMultiplier;
            coinReward = Mathf.RoundToInt(coinReward * tierMultiplier);
        }

        #endregion

        #region Combat

        /// <summary>
        /// Takes damage from the player.
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;

            // Spawn damage number
            SpawnDamageNumber(damage);

            // VFX
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            // Animation
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }

            // Flash red
            StartCoroutine(DamageFlash());

            Debug.Log($"[Enemy] Took {damage} damage ({currentHealth}/{maxHealth})");

            // Check death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator DamageFlash()
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
        }

        /// <summary>
        /// Gets the contact damage this enemy deals.
        /// </summary>
        public float GetContactDamage()
        {
            return contactDamage;
        }

        #endregion

        #region Knockback

        /// <summary>
        /// Applies knockback force to this enemy.
        /// </summary>
        public void ApplyKnockback(Vector2 direction, float force)
        {
            if (!canBeKnockedBack) return;

            float adjustedForce = force / knockbackResistance;
            rb.AddForce(direction * adjustedForce, ForceMode2D.Impulse);
        }

        #endregion

        #region Death & Rewards

        private void Die()
        {
            if (isDead) return;
            isDead = true;

            Debug.Log($"[Enemy] Died - Rewarding {coinReward} coins, {xpReward} XP");

            // Death VFX
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            // Animation
            if (animator != null)
            {
                animator.SetTrigger("Death");
            }

            // Award rewards
            GiveRewards();

            // Chance to drop pickup
            RollForDrop();

            // Increment kill count
            Core.GameManager.Instance.AddEnemyKill();

            // Audio
            Managers.AudioManager.Instance?.PlaySFX("EnemyDeath");

            // Notify listeners
            OnDeath?.Invoke(this);

            // Destroy after delay (for death animation)
            Destroy(gameObject, 1f);
        }

        private void GiveRewards()
        {
            // Add coins to run total
            Core.GameManager.Instance.AddRunCoins(coinReward);

            // XP is awarded at end of run, but track it
            // (Could add immediate XP here if desired)
        }

        private void RollForDrop()
        {
            if (UnityEngine.Random.value < dropChance)
            {
                // Spawn a random pickup
                GameObject[] pickupPrefabs = Resources.LoadAll<GameObject>("Prefabs/Pickups");
                if (pickupPrefabs.Length > 0)
                {
                    GameObject pickup = pickupPrefabs[UnityEngine.Random.Range(0, pickupPrefabs.Length)];
                    Instantiate(pickup, transform.position, Quaternion.identity);
                    Debug.Log("[Enemy] Dropped pickup!");
                }
            }
        }

        #endregion

        #region Damage Numbers

        private void SpawnDamageNumber(float damage)
        {
            if (damageNumberPrefab == null) return;

            GameObject damageNum = Instantiate(damageNumberPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            var textMesh = damageNum.GetComponentInChildren<TMPro.TextMeshPro>();
            if (textMesh != null)
            {
                textMesh.text = Mathf.RoundToInt(damage).ToString();
            }

            Destroy(damageNum, 1f);
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            // Draw health bar in editor
            Vector3 healthBarPos = transform.position + Vector3.up * 1.5f;
            float barWidth = 1f;
            float healthPercent = isDead ? 0f : NormalizedHealth;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(healthBarPos, healthBarPos + Vector3.right * barWidth);

            Gizmos.color = Color.green;
            Gizmos.DrawLine(healthBarPos, healthBarPos + Vector3.right * barWidth * healthPercent);
        }

        #endregion
    }
}
