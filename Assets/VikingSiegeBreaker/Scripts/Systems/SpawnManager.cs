using UnityEngine;
using System.Collections.Generic;

namespace VikingSiegeBreaker.Systems
{
    /// <summary>
    /// Handles runtime spawning of enemies, obstacles, and pickups based on distance and difficulty curve.
    /// Uses object pooling for performance.
    /// </summary>
    public class SpawnManager : MonoBehaviour
    {
        public static SpawnManager Instance { get; private set; }

        [Header("Spawn Settings")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private float spawnDistance = 50f; // How far ahead to spawn
        [SerializeField] private float spawnInterval = 2f; // Seconds between spawns
        [SerializeField] private float despawnDistance = 30f; // Behind player

        [Header("Enemy Spawning")]
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float enemySpawnChance = 0.7f;
        [SerializeField] private int minEnemiesPerWave = 1;
        [SerializeField] private int maxEnemiesPerWave = 5;
        [SerializeField] private float enemySpawnHeight = 0f;
        [SerializeField] private float enemySpawnVariance = 3f;

        [Header("Obstacle Spawning")]
        [SerializeField] private GameObject[] obstaclePrefabs;
        [SerializeField] private float obstacleSpawnChance = 0.4f;
        [SerializeField] private float obstacleSpawnHeight = 0f;

        [Header("Pickup Spawning")]
        [SerializeField] private GameObject[] pickupPrefabs;
        [SerializeField] private float pickupSpawnChance = 0.3f;
        [SerializeField] private float pickupSpawnHeight = 2f;

        [Header("Difficulty Scaling")]
        [SerializeField] private AnimationCurve difficultyByDistance;
        [SerializeField] private float baseSpawnRate = 1f;
        [SerializeField] private float maxSpawnRate = 5f;

        [Header("Spawn Zones")]
        [SerializeField] private float groundLevel = 0f;
        [SerializeField] private float skyLevel = 5f;
        [SerializeField] private float minSpawnDistance = 5f;

        [Header("Debug")]
        [SerializeField] private bool showSpawnZones = true;

        private float spawnTimer = 0f;
        private float lastSpawnX = 0f;
        private List<GameObject> spawnedObjects = new List<GameObject>();

        // Object pools
        private Dictionary<string, Queue<GameObject>> objectPools = new Dictionary<string, Queue<GameObject>>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Find player if not assigned
            if (playerTransform == null)
            {
                var player = FindFirstObjectByType<Player.PlayerController>();
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }

            // Initialize difficulty curve if not set
            if (difficultyByDistance == null || difficultyByDistance.length == 0)
            {
                difficultyByDistance = AnimationCurve.Linear(0f, 0f, 1000f, 1f);
            }

            // Initialize pools
            InitializeObjectPools();

            lastSpawnX = playerTransform.position.x;
        }

        private void Update()
        {
            if (Core.GameManager.Instance.CurrentState != Core.GameState.Playing)
                return;

            if (playerTransform == null) return;

            // Spawn timer
            spawnTimer += Time.deltaTime;
            float currentSpawnRate = GetCurrentSpawnRate();

            if (spawnTimer >= spawnInterval / currentSpawnRate)
            {
                spawnTimer = 0f;
                SpawnWave();
            }

            // Cleanup distant objects
            CleanupDistantObjects();
        }

        #region Object Pooling

        private void InitializeObjectPools()
        {
            // Pre-instantiate pools for enemies
            foreach (var prefab in enemyPrefabs)
            {
                if (prefab != null)
                {
                    CreatePool(prefab.name, prefab, 10);
                }
            }

            // Pre-instantiate pools for obstacles
            foreach (var prefab in obstaclePrefabs)
            {
                if (prefab != null)
                {
                    CreatePool(prefab.name, prefab, 5);
                }
            }

            // Pre-instantiate pools for pickups
            foreach (var prefab in pickupPrefabs)
            {
                if (prefab != null)
                {
                    CreatePool(prefab.name, prefab, 10);
                }
            }
        }

        private void CreatePool(string poolName, GameObject prefab, int initialSize)
        {
            if (objectPools.ContainsKey(poolName)) return;

            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < initialSize; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }

            objectPools[poolName] = pool;
        }

        private GameObject GetPooledObject(string poolName, GameObject prefab)
        {
            if (!objectPools.ContainsKey(poolName))
            {
                CreatePool(poolName, prefab, 5);
            }

            Queue<GameObject> pool = objectPools[poolName];

            if (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                obj.SetActive(true);
                return obj;
            }

            // Pool empty, create new
            return Instantiate(prefab);
        }

        private void ReturnToPool(string poolName, GameObject obj)
        {
            obj.SetActive(false);

            if (objectPools.ContainsKey(poolName))
            {
                objectPools[poolName].Enqueue(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        #endregion

        #region Spawning

        private void SpawnWave()
        {
            float spawnX = playerTransform.position.x + spawnDistance;

            // Don't spawn too close to previous spawn
            if (spawnX - lastSpawnX < minSpawnDistance)
            {
                return;
            }

            lastSpawnX = spawnX;

            // Spawn enemies
            if (Random.value < enemySpawnChance)
            {
                SpawnEnemies(spawnX);
            }

            // Spawn obstacles
            if (Random.value < obstacleSpawnChance)
            {
                SpawnObstacle(spawnX);
            }

            // Spawn pickups
            if (Random.value < pickupSpawnChance)
            {
                SpawnPickup(spawnX);
            }
        }

        private void SpawnEnemies(float spawnX)
        {
            if (enemyPrefabs.Length == 0) return;

            int enemyCount = Random.Range(minEnemiesPerWave, maxEnemiesPerWave + 1);

            // Scale enemy count with difficulty
            float difficulty = GetDifficultyAtDistance(Core.GameManager.Instance.CurrentDistance);
            enemyCount = Mathf.RoundToInt(enemyCount * (1f + difficulty));

            for (int i = 0; i < enemyCount; i++)
            {
                // Random enemy prefab
                GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                // Random spawn position
                float spawnY = enemySpawnHeight + Random.Range(-enemySpawnVariance, enemySpawnVariance);
                float offsetX = Random.Range(-2f, 2f);
                Vector3 spawnPos = new Vector3(spawnX + offsetX, spawnY, 0f);

                // Spawn (use pooling)
                GameObject enemy = GetPooledObject(enemyPrefab.name, enemyPrefab);
                enemy.transform.position = spawnPos;

                // Set tier based on distance
                var enemyScript = enemy.GetComponent<Entities.Enemy>();
                if (enemyScript != null)
                {
                    int tier = GetTierForDistance(Core.GameManager.Instance.CurrentDistance);
                    enemyScript.SetTier(tier);
                }

                spawnedObjects.Add(enemy);
            }

            Debug.Log($"[SpawnManager] Spawned {enemyCount} enemies at x={spawnX}");
        }

        private void SpawnObstacle(float spawnX)
        {
            if (obstaclePrefabs.Length == 0) return;

            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            Vector3 spawnPos = new Vector3(spawnX, obstacleSpawnHeight, 0f);

            GameObject obstacle = GetPooledObject(obstaclePrefab.name, obstaclePrefab);
            obstacle.transform.position = spawnPos;

            spawnedObjects.Add(obstacle);
        }

        private void SpawnPickup(float spawnX)
        {
            if (pickupPrefabs.Length == 0) return;

            GameObject pickupPrefab = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
            Vector3 spawnPos = new Vector3(spawnX, pickupSpawnHeight, 0f);

            GameObject pickup = GetPooledObject(pickupPrefab.name, pickupPrefab);
            pickup.transform.position = spawnPos;

            spawnedObjects.Add(pickup);
        }

        #endregion

        #region Difficulty Scaling

        private float GetCurrentSpawnRate()
        {
            float distance = Core.GameManager.Instance.CurrentDistance;
            float difficulty = GetDifficultyAtDistance(distance);
            return Mathf.Lerp(baseSpawnRate, maxSpawnRate, difficulty);
        }

        private float GetDifficultyAtDistance(float distance)
        {
            return difficultyByDistance.Evaluate(distance);
        }

        private int GetTierForDistance(float distance)
        {
            // Tier increases every 200m
            return Mathf.FloorToInt(distance / 200f) + 1;
        }

        #endregion

        #region Cleanup

        private void CleanupDistantObjects()
        {
            if (playerTransform == null) return;

            float cleanupX = playerTransform.position.x - despawnDistance;

            for (int i = spawnedObjects.Count - 1; i >= 0; i--)
            {
                GameObject obj = spawnedObjects[i];

                if (obj == null)
                {
                    spawnedObjects.RemoveAt(i);
                    continue;
                }

                if (obj.transform.position.x < cleanupX)
                {
                    // Return to pool or destroy
                    string poolName = obj.name.Replace("(Clone)", "").Trim();
                    ReturnToPool(poolName, obj);
                    spawnedObjects.RemoveAt(i);
                }
            }
        }

        public void ClearAllSpawned()
        {
            foreach (var obj in spawnedObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            spawnedObjects.Clear();
        }

        #endregion

        #region Debug

        private void OnDrawGizmos()
        {
            if (!showSpawnZones || playerTransform == null) return;

            // Draw spawn zone
            Gizmos.color = Color.green;
            Vector3 spawnPos = new Vector3(playerTransform.position.x + spawnDistance, 0f, 0f);
            Gizmos.DrawLine(spawnPos + Vector3.up * 10f, spawnPos + Vector3.down * 10f);

            // Draw despawn zone
            Gizmos.color = Color.red;
            Vector3 despawnPos = new Vector3(playerTransform.position.x - despawnDistance, 0f, 0f);
            Gizmos.DrawLine(despawnPos + Vector3.up * 10f, despawnPos + Vector3.down * 10f);
        }

        #endregion
    }
}
