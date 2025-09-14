using UnityEngine;
using System.Collections.Generic;

public class AsteroidRoadGenerator : MonoBehaviour
{
    [System.Serializable]
    public class AsteroidType
    {
        public string tag;       // must match ObjectPool tag
        [Range(0f, 1f)] public float spawnWeight;
    }

    [Header("Asteroid Types")]
    public AsteroidType[] asteroidTypes;

    [Header("Road Generation")]
    public float roadWidth = 30f;
    public float roadHeight = 20f;
    public float asteroidDensity = 2f;
    public float minAsteroidSpacing = 5f;
    public int maxAsteroidsPerFrame = 5;
    public AsteroidFieldManager fieldManager;

    // We will use this list to keep track of active asteroids to check for spacing
    private List<Transform> activeAsteroids = new List<Transform>();

    /// <summary>
    /// Generates asteroids in a specific Z-range.
    /// </summary>
    public void GenerateRoad(Transform player, float startZ, float endZ)
    {
        int asteroidsSpawned = 0;
        float distance = endZ - startZ;
        int totalAsteroidsNeeded = Mathf.RoundToInt(distance * asteroidDensity);

        for (int i = 0; i < totalAsteroidsNeeded && asteroidsSpawned < maxAsteroidsPerFrame; i++)
        {
            float zPos = Random.Range(startZ, endZ);
            if (IsTooCloseToExistingAsteroid(zPos))
            {
                continue;
            }

            float xPos = player.position.x + Random.Range(-roadWidth * 0.5f, roadWidth * 0.5f);
            float yPos = player.position.y + Random.Range(-roadHeight * 0.5f, roadHeight * 0.5f);

            Vector3 spawnPos = new Vector3(xPos, yPos, zPos);

            string chosenTag = ChooseAsteroidTag();
            GameObject asteroid = ObjectPool.Instance.SpawnFromPool(chosenTag, spawnPos, Random.rotation);

            if (asteroid != null)
            {
                // Register the new asteroid
                activeAsteroids.Add(asteroid.transform);
                asteroidsSpawned++;
            }
        }
    }

    /// <summary>
    /// Checks if a potential spawn position is too close to an existing asteroid.
    /// </summary>
    private bool IsTooCloseToExistingAsteroid(float zPos)
    {
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

    /// <summary>
    /// Chooses an asteroid type based on spawn weights.
    /// </summary>
    private string ChooseAsteroidTag()
    {
        if (asteroidTypes == null || asteroidTypes.Length == 0)
        {
            Debug.LogWarning("No asteroid types defined!");
            return "Asteroid";
        }

        float totalWeight = 0f;
        foreach (var type in asteroidTypes)
            totalWeight += type.spawnWeight;

        if (totalWeight <= 0f) return asteroidTypes[0].tag;

        float randomValue = Random.value * totalWeight;
        foreach (var type in asteroidTypes)
        {
            if (randomValue < type.spawnWeight)
                return type.tag;
            randomValue -= type.spawnWeight;
        }
        return asteroidTypes[0].tag;
    }

    /// <summary>
    /// Unregisters an asteroid, typically called when it is consumed or culled.
    /// </summary>
    public void UnregisterAsteroid(Transform asteroid)
    {
        if (activeAsteroids.Contains(asteroid))
        {
            activeAsteroids.Remove(asteroid);
        }
    }

    /// <summary>
    /// Clears the entire list of active asteroids.
    /// </summary>
    public void ClearActiveAsteroids()
    {
        activeAsteroids.Clear();
    }

    void OnDrawGizmosSelected()
    {
        // The player's transform is needed to correctly position the Gizmos relative to the player.
        // Assuming the AsteroidFieldManager has a player reference, and this script is on the same GameObject.
        AsteroidFieldManager fieldManager = GetComponentInParent<AsteroidFieldManager>();
        if (fieldManager == null || fieldManager.player == null)
        {
            Debug.LogWarning("Player or AsteroidFieldManager reference not found. Gizmos will not draw.");
            return;
        }

        Vector3 playerPos = fieldManager.player.position;

        // Visualize the road generation bounds (the area where new asteroids are spawned).
        Gizmos.color = Color.green;
        Vector3 roadCenter = new Vector3(playerPos.x, playerPos.y, playerPos.z + fieldManager.roadLength / 2);
        Vector3 roadSize = new Vector3(roadWidth, roadHeight, fieldManager.roadLength);
        Gizmos.DrawWireCube(roadCenter, roadSize);

        // Visualize the road behind the player (the area that's not culled yet).
        Gizmos.color = Color.yellow;
        Vector3 behindCenter = new Vector3(playerPos.x, playerPos.y, playerPos.z - fieldManager.roadBehindDistance / 2);
        Vector3 behindSize = new Vector3(roadWidth, roadHeight, fieldManager.roadBehindDistance);
        Gizmos.DrawWireCube(behindCenter, behindSize);

        // Visualize the cull distance threshold (asteroids behind this line are removed).
        Gizmos.color = Color.red;
        Vector3 cullPosition = new Vector3(playerPos.x, playerPos.y, playerPos.z - fieldManager.cullDistance);
        Gizmos.DrawLine(cullPosition + Vector3.left * roadWidth, cullPosition + Vector3.right * roadWidth);
        Gizmos.DrawLine(cullPosition + Vector3.up * roadHeight, cullPosition + Vector3.down * roadHeight);

        // Draw the player's position as a reference point.
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(playerPos, 1f);
    }
}