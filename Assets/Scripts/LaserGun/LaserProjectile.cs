using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectile : MonoBehaviour
{
    public float lifeTime = 2f;
    private float timer;
    private Rigidbody rb;
    private LaserProjectilePool pool;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Ensure no gravity
        }
    }

    public void Init(LaserProjectilePool poolRef, Vector3 direction, float speed)
    {
        pool = poolRef;
        timer = lifeTime;

        if (rb != null)
        {
            rb.velocity = direction * speed;
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (pool != null)
                pool.ReturnToPool(gameObject);
            else
                Destroy(gameObject);
        }
    }
}
