using UnityEngine;
using System;

namespace VikingSiegeBreaker.Player
{
    /// <summary>
    /// Central momentum/speed system that drives the core gameplay loop.
    /// Handles: launch velocity, decay over time, enemy hit penalties, pickups, and GameOver trigger.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class MomentumSystem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private PlayerController playerController;

        [Header("Momentum Settings")]
        [SerializeField] private float maxMomentum = 100f;
        [SerializeField] private float currentMomentum = 0f;
        [SerializeField] private float momentumDecayRate = 2f; // Units per second
        [SerializeField] private float minMomentumThreshold = 1f; // GameOver below this

        [Header("Launch Settings")]
        [SerializeField] private float baseLaunchPower = 50f;
        [SerializeField] private float launchAngle = 45f;

        [Header("Collision Penalties")]
        [SerializeField] private float enemyHitPenalty = 5f;
        [SerializeField] private float wallHitPenalty = 3f;
        [SerializeField] private float groundHitPenalty = 1f;

        [Header("Distance Tracking")]
        [SerializeField] private float totalDistance = 0f;
        [SerializeField] private Vector2 lastPosition;

        [Header("UI Feedback")]
        [SerializeField] private Color highSpeedColor = Color.green;
        [SerializeField] private Color mediumSpeedColor = Color.yellow;
        [SerializeField] private Color lowSpeedColor = Color.red;

        // Events
        public event Action<float> OnMomentumChanged; // normalized 0-1
        public event Action<float> OnSpeedChanged; // actual speed
        public event Action OnMomentumDepleted;

        // Properties
        public float CurrentMomentum => currentMomentum;
        public float MaxMomentum => maxMomentum;
        public float NormalizedMomentum => currentMomentum / maxMomentum;
        public float CurrentSpeed => rb.linearVelocity.magnitude;
        public float TotalDistance => totalDistance;
        public bool IsMoving => currentMomentum > minMomentumThreshold;

        private void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (playerController == null) playerController = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Apply upgrades to momentum parameters
            ApplyUpgrades();

            lastPosition = transform.position;
        }

        private void FixedUpdate()
        {
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Playing)
                return;

            // Update momentum based on velocity
            UpdateMomentumFromVelocity();

            // Apply decay
            ApplyMomentumDecay();

            // Track distance
            UpdateDistance();

            // Check for game over
            CheckGameOver();
        }

        #region Launch

        /// <summary>
        /// Launches the player with the given power multiplier (0-1 from catapult charge).
        /// </summary>
        public void Launch(float chargeMultiplier)
        {
            chargeMultiplier = Mathf.Clamp01(chargeMultiplier);

            // Calculate launch force
            float launchPower = baseLaunchPower * chargeMultiplier;
            Vector2 launchDirection = Quaternion.Euler(0, 0, launchAngle) * Vector2.right;
            Vector2 launchForce = launchDirection * launchPower;

            // Apply force
            rb.linearVelocity = Vector2.zero; // Reset velocity
            rb.AddForce(launchForce, ForceMode2D.Impulse);

            // Set initial momentum
            currentMomentum = maxMomentum * chargeMultiplier;

            Debug.Log($"[MomentumSystem] Launched with power {launchPower:F2} (charge: {chargeMultiplier:F2})");
            OnMomentumChanged?.Invoke(NormalizedMomentum);
        }

        #endregion

        #region Momentum Management

        /// <summary>
        /// Updates momentum based on current velocity (momentum = speed).
        /// </summary>
        private void UpdateMomentumFromVelocity()
        {
            float speed = rb.linearVelocity.magnitude;

            // Momentum is directly tied to speed
            currentMomentum = Mathf.Min(speed, maxMomentum);

            OnSpeedChanged?.Invoke(speed);
        }

        /// <summary>
        /// Applies passive momentum decay over time.
        /// </summary>
        private void ApplyMomentumDecay()
        {
            if (currentMomentum <= 0f) return;

            // Decay momentum
            float decay = momentumDecayRate * Time.fixedDeltaTime;
            currentMomentum = Mathf.Max(0f, currentMomentum - decay);

            // Reduce velocity proportionally
            if (rb.linearVelocity.magnitude > minMomentumThreshold)
            {
                rb.linearVelocity *= (1f - (decay / maxMomentum));
            }

            OnMomentumChanged?.Invoke(NormalizedMomentum);
        }

        /// <summary>
        /// Adds momentum (from pickups or abilities).
        /// </summary>
        public void AddMomentum(float amount)
        {
            currentMomentum = Mathf.Min(currentMomentum + amount, maxMomentum);

            // Also boost velocity
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * currentMomentum;
            }

            Debug.Log($"[MomentumSystem] Added {amount} momentum (now: {currentMomentum:F2})");
            OnMomentumChanged?.Invoke(NormalizedMomentum);
        }

        /// <summary>
        /// Reduces momentum (from collisions or penalties).
        /// </summary>
        public void ReduceMomentum(float amount)
        {
            currentMomentum = Mathf.Max(0f, currentMomentum - amount);

            // Also reduce velocity
            if (rb.linearVelocity.magnitude > 0.1f)
            {
                rb.linearVelocity *= (1f - (amount / maxMomentum));
            }

            Debug.Log($"[MomentumSystem] Lost {amount} momentum (now: {currentMomentum:F2})");
            OnMomentumChanged?.Invoke(NormalizedMomentum);
        }

        #endregion

        #region Collision Handling

        /// <summary>
        /// Called when hitting an enemy.
        /// </summary>
        public void OnEnemyHit(Entities.Enemy enemy)
        {
            float penalty = enemyHitPenalty;

            // Tougher enemies cause more penalty
            if (enemy != null)
            {
                penalty *= enemy.MomentumPenaltyMultiplier;
            }

            ReduceMomentum(penalty);
        }

        /// <summary>
        /// Called when hitting a wall/structure.
        /// </summary>
        public void OnWallHit()
        {
            ReduceMomentum(wallHitPenalty);
        }

        /// <summary>
        /// Called when hitting the ground.
        /// </summary>
        public void OnGroundHit()
        {
            ReduceMomentum(groundHitPenalty);
        }

        #endregion

        #region Distance Tracking

        /// <summary>
        /// Updates total distance traveled.
        /// </summary>
        private void UpdateDistance()
        {
            Vector2 currentPos = transform.position;
            float frameDist = Vector2.Distance(lastPosition, currentPos);

            // Only count forward movement
            if (currentPos.x > lastPosition.x)
            {
                totalDistance += frameDist;
                Core.GameManager.Instance.UpdateDistance(totalDistance);
            }

            lastPosition = currentPos;
        }

        #endregion

        #region Game Over

        /// <summary>
        /// Checks if momentum has depleted and triggers GameOver.
        /// </summary>
        private void CheckGameOver()
        {
            if (currentMomentum <= minMomentumThreshold && rb.linearVelocity.magnitude < 0.5f)
            {
                TriggerGameOver();
            }
        }

        private void TriggerGameOver()
        {
            Debug.Log("[MomentumSystem] Momentum depleted - Game Over");
            OnMomentumDepleted?.Invoke();
            Core.GameManager.Instance.EndRun();
        }

        #endregion

        #region Upgrades

        /// <summary>
        /// Applies upgrade effects from UpgradeManager.
        /// </summary>
        private void ApplyUpgrades()
        {
            var upgradeManager = Systems.UpgradeManager.Instance;
            if (upgradeManager == null) return;

            // Launch Power
            float launchBonus = upgradeManager.GetUpgradeValue("LaunchPower");
            baseLaunchPower += launchBonus;

            // Momentum Decay Reduction
            float decayReduction = upgradeManager.GetUpgradeValue("MomentumDecay");
            momentumDecayRate = Mathf.Max(0.5f, momentumDecayRate - decayReduction);

            Debug.Log($"[MomentumSystem] Upgrades applied - Launch: {baseLaunchPower:F2}, Decay: {momentumDecayRate:F2}");
        }

        #endregion

        #region Utility

        /// <summary>
        /// Gets the current speed color for UI feedback.
        /// </summary>
        public Color GetSpeedColor()
        {
            float normalized = NormalizedMomentum;
            if (normalized > 0.6f) return highSpeedColor;
            if (normalized > 0.3f) return mediumSpeedColor;
            return lowSpeedColor;
        }

        /// <summary>
        /// Resets momentum and distance (for revives).
        /// </summary>
        public void ResetForRevive()
        {
            currentMomentum = maxMomentum * 0.5f; // 50% momentum on revive
            rb.linearVelocity = Vector2.right * currentMomentum;
            Debug.Log("[MomentumSystem] Reset for revive");
        }

        #endregion

        #region Debug

        private void OnGUI()
        {
            if (!Debug.isDebugBuild) return;

            GUILayout.BeginArea(new Rect(10, 100, 300, 200));
            GUILayout.Label($"Momentum: {currentMomentum:F2} / {maxMomentum:F2}");
            GUILayout.Label($"Speed: {CurrentSpeed:F2} m/s");
            GUILayout.Label($"Distance: {totalDistance:F2} m");
            GUILayout.Label($"Velocity: {rb.linearVelocity}");
            GUILayout.EndArea();
        }

        #endregion
    }
}
