using UnityEngine;
using System.Collections.Generic;

public class AsteroidFieldManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public AsteroidRoadGenerator roadGenerator;
    public ObjectPool objectPool;

    [Header("Road Settings")]
    public float roadLength = 200f;
    public float roadBehindDistance = 50f;
    public float roadOffset = 50f; // New variable for the initial offset

    [Header("Culling Settings")]
    public float cullDistance = 300f;

    private float lastSpawnedZ;

    void Start()
    {
        if (player == null || roadGenerator == null || objectPool == null)
        {
            Debug.LogError("Required references not set on AsteroidFieldManager!");
            return;
        }

        // The initial Z-position for spawning is set to be the player's position
        // plus the designated road offset.
        lastSpawnedZ = player.position.z + roadOffset;

        GenerateInitialRoad();
    }

    void Update()
    {
        if (player == null) return;

        ExtendRoadForward();
        CullDistantAsteroids();
    }

    private void GenerateInitialRoad()
    {
        float startZ = lastSpawnedZ;
        float endZ = lastSpawnedZ + roadLength;
        roadGenerator.GenerateRoad(player, startZ, endZ);
        lastSpawnedZ = endZ;
    }

    private void ExtendRoadForward()
    {
        float playerZ = player.position.z;
        float targetZ = playerZ + roadLength;

        // The condition for extending the road has been updated.
        // It now triggers when the player is a certain distance past the start of the last spawned segment.
        // This ensures a new segment is generated precisely when the old one is about to be "used up."
        if (playerZ > (lastSpawnedZ - roadLength - roadOffset))
        {
            float generateFromZ = lastSpawnedZ;
            float generateToZ = lastSpawnedZ + roadLength;

            roadGenerator.GenerateRoad(player, generateFromZ, generateToZ);
            lastSpawnedZ = generateToZ;
        }
    }

    private void CullDistantAsteroids()
    {
        float cullThreshold = player.position.z - cullDistance;

        foreach (var activeListEntry in objectPool.ActiveObjects)
        {
            string tag = activeListEntry.Key;
            List<GameObject> activeList = activeListEntry.Value;

            for (int i = activeList.Count - 1; i >= 0; i--)
            {
                GameObject obj = activeList[i];
                if (obj != null && obj.transform.position.z < cullThreshold)
                {
                    objectPool.ReturnToPool(obj, tag);
                    roadGenerator.UnregisterAsteroid(obj.transform);
                }
            }
        }
    }
}