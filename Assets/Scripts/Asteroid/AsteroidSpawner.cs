using UnityEngine;
using System.Collections.Generic;

public class AsteroidSpawner : MonoBehaviour
{
    [System.Serializable]
    public class AsteroidType
    {
        public string tag;       // must match ObjectPool tag
        [Range(0f, 1f)] public float spawnWeight;
    }

    [Header("Asteroid Types")]
    public AsteroidType[] asteroidTypes;

    [Header("References")]
    public Transform player;

    [Header("Road Generation (Z Direction)")]
    public float roadLength = 200f;          // How far ahead to generate asteroids
    public float roadBehindDistance = 50f;   // How far behind player to keep asteroids
    public float roadWidth = 30f;            // Width of the asteroid road
    public float roadHeight = 20f;           // Height variation of the road

    [Header("Asteroid Density")]
    public float asteroidDensity = 2f;       // Asteroids per unit distance
    public float minAsteroidSpacing = 5f;    // Minimum distance between asteroids

    [Header("Spawning Control")]
    public float spawnInterval = 0.5f;       // How often to check for new spawns
    public int maxAsteroidsPerFrame = 5;     // Max asteroids to spawn per frame

    [Header("Culling")]
    public float cullDistance = 300f;        // Distance behind player to remove asteroids

    private float timer;
    private List<Transform> activeAsteroids = new List<Transform>();
    private float lastSpawnedZ = 0f;         // Track the furthest Z position spawned

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in AsteroidSpawner!");
            return;
        }

        // Initialize the last spawned Z position
        lastSpawnedZ = player.position.z;

        // Generate initial road
        GenerateInitialRoad();
    }

    void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            ExtendRoadForward();
            timer = 0f;
        }

        CullDistantAsteroids();
    }

    void GenerateInitialRoad()
    {
        // Generate asteroids from behind player to ahead of player
        float startZ = player.position.z - roadBehindDistance;
        float endZ = player.position.z + roadLength;

        GenerateAsteroidsInZRange(startZ, endZ);
        lastSpawnedZ = endZ;
    }

    void ExtendRoadForward()
    {
        // Calculate how far ahead we need to generate
        float playerZ = player.position.z;
        float targetZ = playerZ + roadLength;

        // Only generate if we need to extend the road
        if (targetZ > lastSpawnedZ)
        {
            float generateFromZ = lastSpawnedZ;
            float generateToZ = targetZ;

            GenerateAsteroidsInZRange(generateFromZ, generateToZ);
            lastSpawnedZ = generateToZ;
        }
    }

    void GenerateAsteroidsInZRange(float startZ, float endZ)
    {
        int asteroidsSpawned = 0;
        float distance = endZ - startZ;
        int totalAsteroidsNeeded = Mathf.RoundToInt(distance * asteroidDensity);

        for (int i = 0; i < totalAsteroidsNeeded && asteroidsSpawned < maxAsteroidsPerFrame; i++)
        {
            // Random Z position within the range
            float zPos = Random.Range(startZ, endZ);

            // Check if this position is too close to existing asteroids
            if (IsTooCloseToExistingAsteroid(zPos))
                continue;

            // Random X and Y positions within road bounds
            float xPos = player.position.x + Random.Range(-roadWidth * 0.5f, roadWidth * 0.5f);
            float yPos = player.position.y + Random.Range(-roadHeight * 0.5f, roadHeight * 0.5f);

            Vector3 spawnPos = new Vector3(xPos, yPos, zPos);

            string chosenTag = ChooseAsteroidTag();
            GameObject asteroid = ObjectPool.Instance.SpawnFromPool(chosenTag, spawnPos, Random.rotation);

            if (asteroid != null)
            {
                activeAsteroids.Add(asteroid.transform);
                asteroidsSpawned++;
            }
        }
    }

    bool IsTooCloseToExistingAsteroid(float zPos)
    {
        // Check if any active asteroid is too close in Z direction
        foreach (Transform asteroid in activeAsteroids)
        {
            if (asteroid != null && asteroid.gameObject.activeInHierarchy)
            {
                float distance = Mathf.Abs(asteroid.position.z - zPos);
                if (distance < minAsteroidSpacing)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void CullDistantAsteroids()
    {
        float playerZ = player.position.z;

        for (int i = activeAsteroids.Count - 1; i >= 0; i--)
        {
            if (activeAsteroids[i] == null)
            {
                activeAsteroids.RemoveAt(i);
                continue;
            }

            Transform asteroid = activeAsteroids[i];

            // Remove asteroids that are too far behind the player
            if (asteroid.position.z < playerZ - cullDistance)
            {
                GameObject asteroidObj = asteroid.gameObject;
                activeAsteroids.RemoveAt(i);

                if (asteroidObj.activeInHierarchy)
                {
                    asteroidObj.SetActive(false);
                }
            }
        }
    }

    string ChooseAsteroidTag()
    {
        if (asteroidTypes == null || asteroidTypes.Length == 0)
        {
            Debug.LogWarning("No asteroid types defined!");
            return "Asteroid"; // Default tag
        }

        float totalWeight = 0f;
        foreach (var type in asteroidTypes)
            totalWeight += type.spawnWeight;

        if (totalWeight <= 0f)
        {
            return asteroidTypes[0].tag;
        }

        float randomValue = Random.value * totalWeight;
        foreach (var type in asteroidTypes)
        {
            if (randomValue < type.spawnWeight)
                return type.tag;
            randomValue -= type.spawnWeight;
        }
        return asteroidTypes[0].tag;
    }

    public void RefreshAsteroidField()
    {
        // Clear all existing asteroids
        foreach (Transform asteroid in activeAsteroids)
        {
            if (asteroid != null && asteroid.gameObject.activeInHierarchy)
            {
                asteroid.gameObject.SetActive(false);
            }
        }
        activeAsteroids.Clear();

        // Reset spawn position
        lastSpawnedZ = player.position.z;

        // Generate new field
        GenerateInitialRoad();
    }

    public void UnregisterAsteroid(Transform asteroid)
    {
        if (activeAsteroids.Contains(asteroid))
        {
            activeAsteroids.Remove(asteroid);
        }
    }

    // Method to spawn asteroids in a specific area (useful for debugging or special events)
    public void SpawnAsteroidsInArea(Vector3 center, Vector3 size, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-size.x * 0.5f, size.x * 0.5f),
                Random.Range(-size.y * 0.5f, size.y * 0.5f),
                Random.Range(-size.z * 0.5f, size.z * 0.5f)
            );

            Vector3 spawnPos = center + randomOffset;
            string chosenTag = ChooseAsteroidTag();

            GameObject asteroid = ObjectPool.Instance.SpawnFromPool(chosenTag, spawnPos, Random.rotation);
            if (asteroid != null)
            {
                activeAsteroids.Add(asteroid.transform);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        Vector3 playerPos = player.position;

        // Draw road bounds
        Gizmos.color = Color.green;
        Vector3 roadCenter = new Vector3(playerPos.x, playerPos.y, playerPos.z + (roadLength - roadBehindDistance) * 0.5f);
        Vector3 roadSize = new Vector3(roadWidth, roadHeight, roadLength + roadBehindDistance);
        Gizmos.DrawWireCube(roadCenter, roadSize);

        // Draw safe cull zone (asteroids won't be culled here)
        Gizmos.color = Color.yellow;
        Vector3 safeZoneCenter = new Vector3(playerPos.x, playerPos.y, playerPos.z - cullDistance * 0.5f);
        Vector3 safeZoneSize = new Vector3(roadWidth * 2f, roadHeight * 2f, cullDistance);
        Gizmos.DrawWireCube(safeZoneCenter, safeZoneSize);

        // Draw cull distance sphere
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPos, cullDistance);

        // Draw spawn area ahead
        Gizmos.color = Color.blue;
        Vector3 spawnCenter = new Vector3(playerPos.x, playerPos.y, playerPos.z + roadLength * 0.5f);
        Vector3 spawnSize = new Vector3(roadWidth, roadHeight, roadLength);
        Gizmos.DrawWireCube(spawnCenter, spawnSize);

        // Draw player forward direction
        Gizmos.color = Color.white;
        Gizmos.DrawRay(playerPos, Vector3.forward * roadLength);
    }
}