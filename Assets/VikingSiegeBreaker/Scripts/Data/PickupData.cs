using UnityEngine;

namespace VikingSiegeBreaker.Data
{
    /// <summary>
    /// ScriptableObject defining pickup properties.
    /// Create instances via: Assets > Create > VikingSiegeBreaker > Data > Pickup
    /// </summary>
    [CreateAssetMenu(fileName = "NewPickup", menuName = "VikingSiegeBreaker/Data/Pickup", order = 3)]
    public class PickupData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Display name")]
        public string pickupName = "Meat";

        [Tooltip("Pickup type/category")]
        public Entities.PickupType pickupType = Entities.PickupType.Meat;

        [Tooltip("Description for UI")]
        [TextArea(2, 3)]
        public string description = "Restores momentum.";

        [Header("Visual")]
        public Sprite sprite;
        public Color glowColor = Color.yellow;
        public float scale = 1f;

        [Header("Effect")]
        [Tooltip("Effect value (meaning depends on type):\n" +
                 "- Coin: amount of coins\n" +
                 "- Meat: momentum restore amount\n" +
                 "- Shield: duration in seconds\n" +
                 "- etc.")]
        public float value = 20f;

        [Header("Spawn Settings")]
        [Tooltip("Rarity weight (higher = more common)")]
        [Range(0.1f, 10f)]
        public float spawnWeight = 1f;

        [Tooltip("Can spawn from enemy drops")]
        public bool canDropFromEnemies = true;

        [Tooltip("Can spawn in world randomly")]
        public bool canSpawnInWorld = true;

        [Header("Behavior")]
        [Tooltip("Lifetime before auto-despawn (seconds)")]
        public float lifetime = 10f;

        [Tooltip("Auto-collect radius (0 = requires collision)")]
        public float magnetRadius = 0f;

        [Tooltip("Bounce on spawn")]
        public bool bounceOnSpawn = true;

        [Header("VFX/SFX")]
        public GameObject collectEffectPrefab;
        public AudioClip collectSound;
        public ParticleSystem trailEffect;

        /// <summary>
        /// Gets display text for this pickup.
        /// Example: "Meat (+20 momentum)"
        /// </summary>
        public string GetDisplayText()
        {
            string valueText = GetValueText();
            return $"{pickupName} ({valueText})";
        }

        /// <summary>
        /// Gets formatted value text based on type.
        /// </summary>
        private string GetValueText()
        {
            switch (pickupType)
            {
                case Entities.PickupType.Coin:
                    return $"+{value:F0} coins";

                case Entities.PickupType.CoinPack:
                    return $"+{value:F0} coins";

                case Entities.PickupType.Gem:
                    return $"+{value:F0} gems";

                case Entities.PickupType.Meat:
                    return $"+{value:F0} momentum";

                case Entities.PickupType.Shield:
                    return $"{value:F1}s shield";

                case Entities.PickupType.Dash:
                    return "Dash refresh";

                case Entities.PickupType.Health:
                    return $"+{value:F0} HP";

                default:
                    return $"+{value:F0}";
            }
        }
    }
}
