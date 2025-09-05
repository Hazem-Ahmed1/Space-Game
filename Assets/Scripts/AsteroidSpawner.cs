using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    [System.Serializable]
    public class AsteroidType
    {
        public string tag;       // must match ObjectPool tag
        [Range(0f, 1f)] public float spawnWeight;
    }

    public AsteroidType[] asteroidTypes;
    public Transform player;
    public float spawnDistance = 50f;
    public float spawnInterval = 1.5f;
    public float pathWidth = 15f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAsteroid();
            timer = 0f;
        }
    }

    void SpawnAsteroid()
    {
        string chosenTag = ChooseAsteroidTag();

        Vector3 spawnPos = player.position + player.forward * spawnDistance;
        spawnPos.x += Random.Range(-pathWidth, pathWidth);
        spawnPos.y += Random.Range(-pathWidth, pathWidth);

        ObjectPool.Instance.SpawnFromPool(chosenTag, spawnPos, Random.rotation);
    }

    string ChooseAsteroidTag()
    {
        float totalWeight = 0f;
        foreach (var type in asteroidTypes)
            totalWeight += type.spawnWeight;

        float randomValue = Random.value * totalWeight;
        foreach (var type in asteroidTypes)
        {
            if (randomValue < type.spawnWeight)
                return type.tag;
            randomValue -= type.spawnWeight;
        }
        return asteroidTypes[0].tag;
    }
}
