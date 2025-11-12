using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace VikingSiegeBreaker.Core
{
    /// <summary>
    /// Central game state manager - handles global state, run lifecycle, and scene transitions.
    /// Persists across scenes using DontDestroyOnLoad.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;

        [Header("Run Stats")]
        [SerializeField] private float currentDistance = 0f;
        [SerializeField] private int currentCoinsCollected = 0;
        [SerializeField] private int currentEnemiesKilled = 0;
        [SerializeField] private float currentRunTime = 0f;
        [SerializeField] private int currentReviveCount = 0;

        [Header("Settings")]
        [SerializeField] private int maxRevivesPerRun = 1;
        [SerializeField] private float distanceMultiplier = 1f; // For display (meters)

        // Events for other systems to listen to
        public event Action<GameState> OnGameStateChanged;
        public event Action OnRunStarted;
        public event Action<RunStats> OnRunEnded;
        public event Action OnPlayerRevived;

        // Properties
        public GameState CurrentState => currentState;
        public float CurrentDistance => currentDistance;
        public int CurrentCoinsCollected => currentCoinsCollected;
        public int CurrentEnemiesKilled => currentEnemiesKilled;
        public bool CanRevive => currentReviveCount < maxRevivesPerRun;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Debug.Log("[GameManager] Initialized");
        }

        private void Start()
        {
            // Load saved data on startup
            SaveSystem.Instance.LoadGame();

            // Check network on startup
            NetworkCheck.Instance.CheckConnection();
        }

        private void Update()
        {
            // Update run time during gameplay
            if (currentState == GameState.Playing)
            {
                currentRunTime += Time.deltaTime;
            }
        }

        #region State Management

        /// <summary>
        /// Changes the current game state and notifies listeners.
        /// </summary>
        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;

            Debug.Log($"[GameManager] State changed: {currentState} -> {newState}");
            currentState = newState;
            OnGameStateChanged?.Invoke(newState);

            // Handle state-specific logic
            HandleStateChange(newState);
        }

        private void HandleStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
            }
        }

        #endregion

        #region Run Lifecycle

        /// <summary>
        /// Starts a new run - resets stats and notifies listeners.
        /// </summary>
        public void StartRun()
        {
            Debug.Log("[GameManager] Starting new run");

            // Reset run stats
            currentDistance = 0f;
            currentCoinsCollected = 0;
            currentEnemiesKilled = 0;
            currentRunTime = 0f;
            currentReviveCount = 0;

            ChangeState(GameState.Playing);
            OnRunStarted?.Invoke();
        }

        /// <summary>
        /// Ends the current run and calculates final rewards.
        /// </summary>
        public void EndRun()
        {
            Debug.Log($"[GameManager] Run ended - Distance: {currentDistance:F2}m, Coins: {currentCoinsCollected}, Enemies: {currentEnemiesKilled}");

            ChangeState(GameState.GameOver);

            // Create run stats
            RunStats stats = new RunStats
            {
                distance = currentDistance,
                coinsCollected = currentCoinsCollected,
                enemiesKilled = currentEnemiesKilled,
                runTime = currentRunTime,
                reviveCount = currentReviveCount,
                bonusXP = CalculateBonusXP()
            };

            // Award currencies
            Systems.CurrencyManager.Instance.AddCoins(currentCoinsCollected);
            Systems.CurrencyManager.Instance.AddXP(stats.bonusXP);

            // Save progress
            SaveSystem.Instance.SaveGame();

            OnRunEnded?.Invoke(stats);
        }

        /// <summary>
        /// Revives the player mid-run (called after watching rewarded ad).
        /// </summary>
        public void RevivePlayer()
        {
            if (!CanRevive)
            {
                Debug.LogWarning("[GameManager] Cannot revive - max revives reached");
                return;
            }

            Debug.Log("[GameManager] Player revived");
            currentReviveCount++;
            ChangeState(GameState.Playing);
            OnPlayerRevived?.Invoke();
        }

        #endregion

        #region Run Stats Tracking

        /// <summary>
        /// Updates the distance traveled (call from MomentumSystem).
        /// </summary>
        public void UpdateDistance(float distance)
        {
            currentDistance = distance * distanceMultiplier;
        }

        /// <summary>
        /// Adds coins collected during the run.
        /// </summary>
        public void AddRunCoins(int amount)
        {
            currentCoinsCollected += amount;
        }

        /// <summary>
        /// Increments enemy kill count.
        /// </summary>
        public void AddEnemyKill()
        {
            currentEnemiesKilled++;
        }

        /// <summary>
        /// Calculates bonus XP based on performance.
        /// </summary>
        private int CalculateBonusXP()
        {
            // Base XP on distance and enemies killed
            float distanceXP = currentDistance * 0.5f;
            float enemyXP = currentEnemiesKilled * 2f;
            return Mathf.RoundToInt(distanceXP + enemyXP);
        }

        #endregion

        #region Scene Management

        /// <summary>
        /// Loads a scene by name.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Debug.Log($"[GameManager] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Loads the main menu.
        /// </summary>
        public void LoadMainMenu()
        {
            LoadScene("MainMenu");
            ChangeState(GameState.MainMenu);
        }

        /// <summary>
        /// Loads the gameplay scene.
        /// </summary>
        public void LoadGameplay()
        {
            LoadScene("Gameplay");
            // State will change to Playing when StartRun is called
        }

        /// <summary>
        /// Restarts the current run.
        /// </summary>
        public void RestartRun()
        {
            LoadGameplay();
        }

        #endregion

        #region Pause/Resume

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
        }

        /// <summary>
        /// Resumes the game from pause.
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }

        #endregion

        #region Application Lifecycle

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // Auto-save when app goes to background
                SaveSystem.Instance.SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            // Save on quit
            SaveSystem.Instance.SaveGame();
        }

        #endregion
    }

    /// <summary>
    /// Enum representing all possible game states.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    /// <summary>
    /// Data structure for end-of-run statistics.
    /// </summary>
    [Serializable]
    public struct RunStats
    {
        public float distance;
        public int coinsCollected;
        public int enemiesKilled;
        public float runTime;
        public int reviveCount;
        public int bonusXP;
    }
}
