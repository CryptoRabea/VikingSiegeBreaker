using UnityEngine;

namespace VikingSiegeBreaker.Data
{
    /// <summary>
    /// ScriptableObject defining enemy stats and behavior.
    /// Create instances via: Assets > Create > VikingSiegeBreaker > Data > Enemy
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "VikingSiegeBreaker/Data/Enemy", order = 2)]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name")]
        public string enemyName = "Soldier";

        [Tooltip("Enemy tier (1=common, 2=uncommon, 3=rare, 4=epic, 5=legendary)")]
        [Range(1, 5)]
        public int tier = 1;

        [Tooltip("Description for bestiary")]
        [TextArea(2, 3)]
        public string description = "A basic soldier defending the castle.";

        [Header("Visual")]
        public Sprite sprite;
        public RuntimeAnimatorController animatorController;
        public Color tintColor = Color.white;

        [Header("Base Stats")]
        [Tooltip("Base health points")]
        public float baseHealth = 50f;

        [Tooltip("Damage dealt on contact with player")]
        public float contactDamage = 10f;

        [Tooltip("How much this enemy slows player momentum (multiplier)")]
        [Range(0.5f, 3f)]
        public float momentumPenalty = 1f;

        [Header("Physics")]
        [Tooltip("Mass (affects knockback resistance)")]
        [Range(0.1f, 10f)]
        public float mass = 1f;

        [Tooltip("Resistance to knockback (higher = less knockback)")]
        [Range(0.5f, 5f)]
        public float knockbackResistance = 1f;

        [Header("Rewards")]
        [Tooltip("Coins dropped on death")]
        public int coinReward = 5;

        [Tooltip("XP awarded on death")]
        public int xpReward = 2;

        [Tooltip("Chance to drop a pickup (0-1)")]
        [Range(0f, 1f)]
        public float dropChance = 0.2f;

        [Header("Behavior")]
        [Tooltip("Enemy type/behavior pattern")]
        public EnemyType enemyType = EnemyType.Stationary;

        [Tooltip("Detection range (for active enemies)")]
        public float detectionRange = 10f;

        [Tooltip("Movement speed (for mobile enemies)")]
        public float moveSpeed = 2f;

        [Header("VFX")]
        public GameObject hitEffectPrefab;
        public GameObject deathEffectPrefab;
        public AudioClip hitSound;
        public AudioClip deathSound;

        /// <summary>
        /// Calculates scaled stats for a given distance.
        /// </summary>
        public EnemyStats GetStatsForDistance(float distance)
        {
            float healthScale = 1f + (distance / 100f) * 0.05f; // +5% per 100m
            float damageScale = 1f + (distance / 100f) * 0.03f; // +3% per 100m
            float rewardScale = 1f + (distance / 100f) * 0.1f;  // +10% per 100m

            return new EnemyStats
            {
                health = baseHealth * healthScale,
                damage = contactDamage * damageScale,
                coinReward = Mathf.RoundToInt(coinReward * rewardScale),
                xpReward = Mathf.RoundToInt(xpReward * rewardScale)
            };
        }

        /// <summary>
        /// Calculates tier-scaled stats.
        /// </summary>
        public EnemyStats GetStatsForTier(int tierLevel)
        {
            float tierMultiplier = 1f + (tierLevel - 1) * 0.5f; // +50% per tier

            return new EnemyStats
            {
                health = baseHealth * tierMultiplier,
                damage = contactDamage * tierMultiplier,
                coinReward = Mathf.RoundToInt(coinReward * tierMultiplier),
                xpReward = Mathf.RoundToInt(xpReward * tierMultiplier)
            };
        }
    }

    /// <summary>
    /// Struct for calculated enemy stats.
    /// </summary>
    [System.Serializable]
    public struct EnemyStats
    {
        public float health;
        public float damage;
        public int coinReward;
        public int xpReward;
    }

    /// <summary>
    /// Enemy behavior types.
    /// </summary>
    public enum EnemyType
    {
        Stationary,     // Doesn't move, defends position
        Patrolling,     // Moves back and forth
        Chasing,        // Chases player when in range
        Flying,         // Flies in air
        Ranged,         // Shoots projectiles
        Tank            // High HP, slow, blocks path
    }
}
