using UnityEngine;
using System.Collections.Generic;

public class ShoeSpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Road Generation (Z Direction)")]
    public float roadLength = 200f;          // قد إيه يولد قدام اللاعب
    public float roadBehindDistance = 50f;   // قد إيه يولد ورا اللاعب
    public float roadWidth = 20f;            // المسافة على محور X
    public float roadHeight = 10f;           // المسافة على محور Y

    [Header("PowerUp Density")]
    public float powerUpDensity = 0.02f;      
    public float minSpacing = 40f;           // أقل مسافة بين باور أب والتاني

    [Header("Spawning Control")]
    public float spawnInterval = 2f;         // كل كام ثانية يحاول يولد
    public int maxPowerUpsPerFrame = 1;      // يولد كام باور أب في المرة

    [Header("Culling")]
    public float cullDistance = 300f;        // يشيل الباور أب البعيد

    private float timer;
    private List<Transform> activePowerUps = new List<Transform>();
    private float lastSpawnedZ = 0f;

    [Header("Pooling")]
    public string powerUpTag = "PowerUp";    // نفس التاج اللي في ObjectPool

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not set in PowerUpSpawner!");
            return;
        }

        lastSpawnedZ = player.position.z;
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

        CullDistantPowerUps();
    }

    void GenerateInitialRoad()
    {
        float startZ = player.position.z - roadBehindDistance;
        float endZ = player.position.z + roadLength;

        GeneratePowerUpsInZRange(startZ, endZ);
        lastSpawnedZ = endZ;
    }

    void ExtendRoadForward()
    {
        float playerZ = player.position.z;
        float targetZ = playerZ + roadLength;

        if (targetZ > lastSpawnedZ)
        {
            float generateFromZ = lastSpawnedZ;
            float generateToZ = targetZ;

            GeneratePowerUpsInZRange(generateFromZ, generateToZ);
            lastSpawnedZ = generateToZ;
        }
    }

    void GeneratePowerUpsInZRange(float startZ, float endZ)
    {
        int spawned = 0;
        float distance = endZ - startZ;
        int totalNeeded = Mathf.RoundToInt(distance * powerUpDensity);

        for (int i = 0; i < totalNeeded && spawned < maxPowerUpsPerFrame; i++)
        {
            float zPos = Random.Range(startZ, endZ);

            if (IsTooClose(zPos)) continue;

            float xPos = player.position.x + Random.Range(-roadWidth * 0.5f, roadWidth * 0.5f);
            float yPos = player.position.y + Random.Range(-roadHeight * 0.5f, roadHeight * 0.5f);

            Vector3 spawnPos = new Vector3(xPos, yPos, zPos);

            GameObject powerUp = ObjectPool.Instance.SpawnFromPool(powerUpTag, spawnPos, Quaternion.identity);

            if (powerUp != null)
            {
                activePowerUps.Add(powerUp.transform);
                spawned++;
            }
        }
    }

    bool IsTooClose(float zPos)
    {
        foreach (Transform p in activePowerUps)
        {
            if (p != null && p.gameObject.activeInHierarchy)
            {
                float distance = Mathf.Abs(p.position.z - zPos);
                if (distance < minSpacing) return true;
            }
        }
        return false;
    }

    void CullDistantPowerUps()
    {
        float playerZ = player.position.z;

        for (int i = activePowerUps.Count - 1; i >= 0; i--)
        {
            Transform p = activePowerUps[i];
            if (p == null)
            {
                activePowerUps.RemoveAt(i);
                continue;
            }

            if (p.position.z < playerZ - cullDistance)
            {
                GameObject obj = p.gameObject;
                activePowerUps.RemoveAt(i);

                if (obj.activeInHierarchy)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}
