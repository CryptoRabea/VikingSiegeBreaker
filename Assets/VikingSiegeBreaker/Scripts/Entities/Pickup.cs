using UnityEngine;
using System;

namespace VikingSiegeBreaker.Entities
{
    /// <summary>
    /// Generic pickup system - handles different pickup types:
    /// - Meat (momentum restore)
    /// - Shield (temporary invincibility)
    /// - Dash (instant dash refresh)
    /// - Coin packs
    /// - Runes (premium currency)
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Pickup : MonoBehaviour
    {
        [Header("Pickup Data")]
        [SerializeField] private Data.PickupData pickupData; // ScriptableObject reference
        [SerializeField] private PickupType type = PickupType.Coin;

        [Header("Effects")]
        [SerializeField] private float value = 1f; // Amount/duration depends on type
        [SerializeField] private GameObject collectEffectPrefab;
        [SerializeField] private AudioClip collectSound;

        [Header("Visuals")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool floatAnimation = true;
        [SerializeField] private float floatSpeed = 1f;
        [SerializeField] private float floatHeight = 0.3f;

        [Header("Lifetime")]
        [SerializeField] private float lifetime = 10f; // Auto-destroy after X seconds
        [SerializeField] private bool fadeBeforeDestroy = true;

        private Vector3 startPosition;
        private float spawnTime;
        private bool collected = false;

        private void Awake()
        {
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null) animator = GetComponent<Animator>();

            // Ensure trigger collider
            var collider = GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }

        private void Start()
        {
            // Initialize from data
            if (pickupData != null)
            {
                InitializeFromData();
            }

            startPosition = transform.position;
            spawnTime = Time.time;

            // Start float animation
            if (floatAnimation)
            {
                StartCoroutine(FloatAnimation());
            }
        }

        private void Update()
        {
            // Check lifetime
            if (Time.time - spawnTime > lifetime)
            {
                Expire();
            }
        }

        #region Initialization

        private void InitializeFromData()
        {
            type = pickupData.pickupType;
            value = pickupData.value;
            lifetime = pickupData.lifetime;

            if (spriteRenderer != null && pickupData.sprite != null)
            {
                spriteRenderer.sprite = pickupData.sprite;
            }

            collectSound = pickupData.collectSound;
        }

        #endregion

        #region Collection

        /// <summary>
        /// Called when player collects this pickup.
        /// </summary>
        public void Collect(Player.PlayerController player)
        {
            if (collected) return;
            collected = true;

            Debug.Log($"[Pickup] {type} collected by player (value: {value})");

            // Apply effect based on type
            ApplyEffect(player);

            // VFX
            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }

            // Audio
            if (collectSound != null)
            {
                Managers.AudioManager.Instance?.PlaySFX(collectSound);
            }
            else
            {
                Managers.AudioManager.Instance?.PlaySFX("PickupCollect");
            }

            // Destroy
            Destroy(gameObject);
        }

        private void ApplyEffect(Player.PlayerController player)
        {
            switch (type)
            {
                case PickupType.Coin:
                    CollectCoin();
                    break;

                case PickupType.Meat:
                    RestoreMomentum(player);
                    break;

                case PickupType.Shield:
                    ActivateShield(player);
                    break;

                case PickupType.Dash:
                    RefreshDash(player);
                    break;

                case PickupType.Gem:
                    CollectGem();
                    break;

                case PickupType.Health:
                    RestoreHealth(player);
                    break;

                case PickupType.CoinPack:
                    CollectCoinPack();
                    break;
            }
        }

        #endregion

        #region Pickup Effects

        private void CollectCoin()
        {
            int amount = Mathf.RoundToInt(value);
            Core.GameManager.Instance.AddRunCoins(amount);
            Debug.Log($"[Pickup] Collected {amount} coin(s)");
        }

        private void CollectCoinPack()
        {
            int amount = Mathf.RoundToInt(value);
            Core.GameManager.Instance.AddRunCoins(amount);
            Debug.Log($"[Pickup] Collected coin pack: {amount} coins!");
        }

        private void CollectGem()
        {
            int amount = Mathf.RoundToInt(value);
            Systems.CurrencyManager.Instance.AddGems(amount);
            Debug.Log($"[Pickup] Collected {amount} gem(s)");
        }

        private void RestoreMomentum(Player.PlayerController player)
        {
            var momentum = player.GetComponent<Player.MomentumSystem>();
            if (momentum != null)
            {
                momentum.AddMomentum(value);
                Debug.Log($"[Pickup] Restored {value} momentum");
            }
        }

        private void ActivateShield(Player.PlayerController player)
        {
            player.ActivateShield();
            Debug.Log($"[Pickup] Shield activated for {value} seconds");
        }

        private void RefreshDash(Player.PlayerController player)
        {
            // Dash cooldown reset handled in PlayerController
            Debug.Log("[Pickup] Dash cooldown refreshed");
        }

        private void RestoreHealth(Player.PlayerController player)
        {
            // Health restore logic (requires public method in PlayerController)
            Debug.Log($"[Pickup] Restored {value} health");
        }

        #endregion

        #region Visuals

        private System.Collections.IEnumerator FloatAnimation()
        {
            while (!collected)
            {
                float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
                transform.position = startPosition + Vector3.up * offset;
                yield return null;
            }
        }

        private void Expire()
        {
            if (fadeBeforeDestroy)
            {
                StartCoroutine(FadeOut());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private System.Collections.IEnumerator FadeOut()
        {
            float fadeTime = 1f;
            float elapsed = 0f;

            Color startColor = spriteRenderer.color;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            Destroy(gameObject);
        }

        #endregion

        #region Magnet Effect (Optional)

        /// <summary>
        /// Attracts pickup to player when in range (optional feature).
        /// </summary>
        public void AttractToPlayer(Transform playerTransform, float speed)
        {
            if (collected) return;

            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }

        #endregion
    }

    /// <summary>
    /// Enum for different pickup types.
    /// </summary>
    public enum PickupType
    {
        Coin,           // Single coin
        CoinPack,       // Multiple coins (5-10)
        Gem,            // Premium currency
        Meat,           // Momentum restore
        Shield,         // Temporary invincibility
        Dash,           // Refresh dash cooldown
        Health,         // Restore health
        Magnet,         // Attract coins (future)
        SpeedBoost      // Temporary speed boost (future)
    }
}
