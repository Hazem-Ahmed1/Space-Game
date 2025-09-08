using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectilePool: MonoBehaviour
{
    public GameObject prefab;
    public int poolSize = 100;


    public List<GameObject> projectilePool;
    private void Start()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            projectilePool.Add(obj);
        }
    }
    public GameObject GetObject()
    {
        foreach (GameObject obj in projectilePool)
        {
            if (obj != null && !obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        GameObject newObject = Instantiate(prefab, transform);
        projectilePool.Add(newObject);
        return newObject;
    }
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

}
