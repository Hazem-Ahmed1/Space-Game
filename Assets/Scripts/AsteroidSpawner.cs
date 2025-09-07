//using UnityEngine;

//public class AsteroidSpawner : MonoBehaviour
//{
//    [System.Serializable]
//    public class AsteroidType
//    {
//        public string tag;       // must match ObjectPool tag
//        [Range(0f, 1f)] public float spawnWeight;
//    }

//    public AsteroidType[] asteroidTypes;
//    public Transform player;
//    public float spawnDistance = 50f;
//    public float spawnInterval = 1.5f;
//    public float pathWidth = 15f;

//    private float timer;

//    void Update()
//    {
//        timer += Time.deltaTime;
//        if (timer >= spawnInterval)
//        {
//            SpawnAsteroid();
//            timer = 0f;
//        }
//    }

//    void SpawnAsteroid()
//    {
//        string chosenTag = ChooseAsteroidTag();

//        Vector3 spawnPos = player.position + player.forward * spawnDistance;
//        spawnPos.x += Random.Range(-pathWidth, pathWidth);
//        spawnPos.y += Random.Range(-pathWidth, pathWidth);

//        ObjectPool.Instance.SpawnFromPool(chosenTag, spawnPos, Random.rotation);
//    }

//    string ChooseAsteroidTag()
//    {
//        float totalWeight = 0f;
//        foreach (var type in asteroidTypes)
//            totalWeight += type.spawnWeight;

//        float randomValue = Random.value * totalWeight;
//        foreach (var type in asteroidTypes)
//        {
//            if (randomValue < type.spawnWeight)
//                return type.tag;
//            randomValue -= type.spawnWeight;
//        }
//        return asteroidTypes[0].tag;
//    }
//}


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

    public AsteroidType[] asteroidTypes;
    public Transform player;
    public float spawnDistance = 50f;
    public float cullDistance = 600f; // المسافة التي تختفي عندها الكويكبات (600 وحدة)
    public float spawnInterval = 1.5f;
    public float pathWidth = 15f;
    public int preSpawnCount = 200; // عدد الكويكبات المُسبقة التوليد
    public float worldRadius = 300f; // نصف قطر المنطقة المحيطة باللاعب

    private float timer;
    private List<Transform> activeAsteroids = new List<Transform>();
    //private bool hasInitialized = false;

    void Start()
    {
        // إنشاء كويكبات مسبقة حول اللاعب في بداية اللعبة فقط
        PreSpawnAsteroids();
        //hasInitialized = true;
    }

    void Update()
    {
        // الاستمرار في إنتاج كويكبات جديدة أمام اللاعب فقط
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnAsteroidInFront();
            timer = 0f;
        }

        // إزالة الكويكبات البعيدة جداً فقط (600 وحدة)
        CullVeryDistantAsteroids();
    }

    void PreSpawnAsteroids()
    {
        // إنشاء كويكبات في دائرة كبيرة حول اللاعب
        for (int i = 0; i < preSpawnCount; i++)
        {
            // توزيع عشوائي في دائرة حول اللاعب
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection.y = Random.Range(-worldRadius * 0.3f, worldRadius * 0.3f); // تقليل الارتفاع

            Vector3 spawnPos = player.position + randomDirection.normalized * Random.Range(20f, worldRadius);

            string chosenTag = ChooseAsteroidTag();

            GameObject asteroid = ObjectPool.Instance.SpawnFromPool(chosenTag, spawnPos, Random.rotation);
            if (asteroid != null)
            {
                activeAsteroids.Add(asteroid.transform);
            }
        }

        // إضافة كويكبات إضافية أمام اللاعب
        for (int i = 0; i < 50; i++)
        {
            Vector3 forwardPos = player.position + player.forward * Random.Range(10f, spawnDistance * 2f);
            forwardPos.x += Random.Range(-pathWidth * 2f, pathWidth * 2f);
            forwardPos.y += Random.Range(-pathWidth * 2f, pathWidth * 2f);

            string chosenTag = ChooseAsteroidTag();
            GameObject asteroid = ObjectPool.Instance.SpawnFromPool(chosenTag, forwardPos, Random.rotation);
            if (asteroid != null)
            {
                activeAsteroids.Add(asteroid.transform);
            }
        }
    }

    void SpawnAsteroidInFront()
    {
        // إنشاء كويكب أمام اللاعب فقط (السلوك الأصلي)
        string chosenTag = ChooseAsteroidTag();
        Vector3 spawnPos = player.position + player.forward * spawnDistance;
        spawnPos.x += Random.Range(-pathWidth, pathWidth);
        spawnPos.y += Random.Range(-pathWidth, pathWidth);

        GameObject asteroid = ObjectPool.Instance.SpawnFromPool(chosenTag, spawnPos, Random.rotation);
        if (asteroid != null)
        {
            activeAsteroids.Add(asteroid.transform);
        }
    }

    void CullVeryDistantAsteroids()
    {
        // إزالة الكويكبات البعيدة جداً فقط (600 وحدة)
        for (int i = activeAsteroids.Count - 1; i >= 0; i--)
        {
            if (activeAsteroids[i] == null)
            {
                activeAsteroids.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(player.position, activeAsteroids[i].position);

            // إزالة الكويكبات البعيدة جداً فقط
            if (distance > cullDistance)
            {
                GameObject asteroidObj = activeAsteroids[i].gameObject;
                activeAsteroids.RemoveAt(i);

                // إرجاع الكويكب إلى الـ pool
                if (asteroidObj.activeInHierarchy)
                {
                    asteroidObj.SetActive(false);
                }
            }
        }
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

    // دالة لإعادة إنشاء الكويكبات عند الحاجة
    public void RefreshAsteroidField()
    {
        // مسح الكويكبات الحالية
        foreach (Transform asteroid in activeAsteroids)
        {
            if (asteroid != null && asteroid.gameObject.activeInHierarchy)
            {
                asteroid.gameObject.SetActive(false);
            }
        }
        activeAsteroids.Clear();

        // إعادة إنشاء المجال
        PreSpawnAsteroids();
    }

    // للمعلومات والتشخيص
    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            // رسم منطقة الإنشاء المسبق
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(player.position, worldRadius);

            // رسم منطقة الإزالة
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, cullDistance);

            // رسم منطقة الإنشاء الأمامية
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(player.position + player.forward * spawnDistance,
                               new Vector3(pathWidth * 2f, pathWidth * 2f, 5f));
        }
    }
}