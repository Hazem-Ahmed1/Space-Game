using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;
    // Inside your ObjectPool.cs script
    public Dictionary<string, List<GameObject>> ActiveObjects { get; private set; } // Make it a public property
    //private Dictionary<string, List<GameObject>> activeObjects; // Track active objects

    void Awake()
    {
        // Singleton pattern with proper cleanup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //void Start()
    //{
    //    poolDictionary = new Dictionary<string, Queue<GameObject>>();
    //    activeObjects = new Dictionary<string, List<GameObject>>();

    //    foreach (Pool pool in pools)
    //    {
    //        Queue<GameObject> objectPool = new Queue<GameObject>();
    //        List<GameObject> activeList = new List<GameObject>();

    //        for (int i = 0; i < pool.size; i++)
    //        {
    //            GameObject obj = Instantiate(pool.prefab);
    //            obj.SetActive(false);
    //            obj.name = pool.prefab.name + "_" + i; // Better naming for debugging
    //            objectPool.Enqueue(obj);
    //        }

    //        poolDictionary.Add(pool.tag, objectPool);
    //        activeObjects.Add(pool.tag, activeList);
    //    }
    //}
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        ActiveObjects = new Dictionary<string, List<GameObject>>(); // Use the new public property

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            List<GameObject> activeList = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.name = pool.prefab.name + "_" + i;
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
            ActiveObjects.Add(pool.tag, activeList); // Add to the public property
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];

        // Check if pool is empty
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool for tag " + tag + " is empty! Consider increasing pool size.");
            return null;
        }

        GameObject objectToSpawn = pool.Dequeue();

        // Make sure the object isn't already active
        if (objectToSpawn.activeInHierarchy)
        {
            Debug.LogWarning("Trying to spawn an already active object: " + objectToSpawn.name);
            // Try to find an inactive object
            objectToSpawn = FindInactiveObject(tag);
            if (objectToSpawn == null)
            {
                Debug.LogWarning("No inactive objects available in pool: " + tag);
                return null;
            }
        }

        // Set position and rotation before activating
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // Track as active object
        ActiveObjects[tag].Add(objectToSpawn);

        // Reset object state if it has a reset interface
        IResetable resetable = objectToSpawn.GetComponent<IResetable>();
        if (resetable != null)
        {
            resetable.ResetForReuse();
        }

        return objectToSpawn;
    }

    public void ReturnToPool(GameObject obj, string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return;
        }

        // Remove from active list
        if (ActiveObjects[tag].Contains(obj))
        {
            ActiveObjects[tag].Remove(obj);
        }

        // Deactivate and return to pool
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    public void ReturnToPool(GameObject obj)
    {
        // Try to find which pool this object belongs to
        string foundTag = null;
        foreach (var kvp in ActiveObjects)
        {
            if (kvp.Value.Contains(obj))
            {
                foundTag = kvp.Key;
                break;
            }
        }

        if (foundTag != null)
        {
            ReturnToPool(obj, foundTag);
        }
        else
        {
            Debug.LogWarning("Object " + obj.name + " not found in any active pool!");
        }
    }

    private GameObject FindInactiveObject(string tag)
    {
        // Search through all objects in the pool to find an inactive one
        Queue<GameObject> pool = poolDictionary[tag];
        int originalCount = pool.Count;

        for (int i = 0; i < originalCount; i++)
        {
            GameObject obj = pool.Dequeue();
            if (!obj.activeInHierarchy)
            {
                return obj; // Found inactive object
            }
            pool.Enqueue(obj); // Put back in queue
        }

        return null; // No inactive objects found
    }

    // Utility methods for debugging and management
    public int GetActiveCount(string tag)
    {
        if (ActiveObjects.ContainsKey(tag))
        {
            return ActiveObjects[tag].Count;
        }
        return 0;
    }

    public int GetInactiveCount(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            return poolDictionary[tag].Count;
        }
        return 0;
    }

    public void ReturnAllToPool(string tag)
    {
        if (!ActiveObjects.ContainsKey(tag)) return;

        List<GameObject> activeList = new List<GameObject>(ActiveObjects[tag]);
        foreach (GameObject obj in activeList)
        {
            ReturnToPool(obj, tag);
        }
    }

    public void ReturnAllToPool()
    {
        foreach (string tag in ActiveObjects.Keys)
        {
            ReturnAllToPool(tag);
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Debug information
    //void OnGUI()
    //{
    //    if (Application.isPlaying && poolDictionary != null)
    //    {
    //        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
    //        GUILayout.Label("Object Pool Status:");

    //        foreach (var pool in pools)
    //        {
    //            int active = GetActiveCount(pool.tag);
    //            int inactive = GetInactiveCount(pool.tag);
    //            GUILayout.Label($"{pool.tag}: Active({active}) Inactive({inactive}) Total({pool.size})");
    //        }

    //        GUILayout.EndArea();
    //    }
    //}
}