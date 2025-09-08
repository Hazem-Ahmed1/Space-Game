using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SocialPlatforms;



public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject proj;
    [SerializeField] private bool useProjectileGravity = true;
    [SerializeField] private bool isAuto = true;
    [SerializeField] private bool useHitscan = true;
    [SerializeField] private float rangeProjectileSpeed = 100f;
    [SerializeField] private float hitscanRange = 100f;
    [SerializeField] private LayerMask hitsacenLayer;
    [SerializeField] private float fireRate = .25f;
    private bool canShoot = true;
    private float shootTimer;
    [SerializeField] private string gunName;
    private GameObject ammoPool;
    private ProjectilePool projectilePool;
    [SerializeField] private int ammoCount = 15;
    private int maxGunAmmo;
    private int curGunAmmo;
    [SerializeField] private float reloadTime = 2f;
    [SerializeField] private float hitScanDamage = 25f;
    private bool canReload = false;
    private bool isReloadind = false;
    private float rTimer;


    private void Awake()
    {
        maxGunAmmo = ammoCount;
        curGunAmmo = maxGunAmmo;
        ammoPool = new GameObject();
        projectilePool = ammoPool.AddComponent<ProjectilePool>();
        projectilePool.prefab = proj;
        projectilePool.poolSize = ammoCount;
        projectilePool.gameObject.name = gunName + "ammoPool";
    }
    void Update()
    {

        PlayerAmmo.currentMaxAmmo = maxGunAmmo;
        PlayerAmmo.currentAmmo = curGunAmmo;
        
        if (canShoot == false)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer > fireRate)
            {
                canShoot = true;
                shootTimer = 0;
            }
        }

        if (isReloadind == true)
        {
            rTimer += Time.deltaTime;
            if (rTimer > reloadTime)
            {
                isReloadind = false;
                curGunAmmo = maxGunAmmo;
                rTimer = 0;
            }

        }

        if (canReload == true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                isReloadind = true;
                canReload = false;
            }
        }
        if (curGunAmmo > 0)
        {
            ShootGun();
        }
        else if (curGunAmmo < maxGunAmmo)
        {
            canReload = true;
        }
    }
    private void ShootGun()
    {



        if (isAuto)
        {
            if (canShoot && Input.GetKey(KeyCode.Space))
            {
                if (useHitscan)
                {
                    FireHitscan();
                }
                else
                {
                    CreateProjectile();
                }
                canShoot = false;

            }

        }

        else
        {
            if (canShoot && Input.GetKeyDown(KeyCode.Space))
            {
                if (useHitscan)
                {
                    FireHitscan();
                }
                else
                {
                    CreateProjectile();
                }
                canShoot = false;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * hitscanRange);
    }



    public void CreateProjectile()
    {

        proj = projectilePool.GetObject();
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        proj.transform.position = transform.position;
        proj.transform.rotation = transform.rotation;
        if (useProjectileGravity)
        {
            rb.useGravity = true;
        }
        else
        {
            rb.useGravity = false;
        }
        rb.AddForce(gameObject.transform.forward * rangeProjectileSpeed, ForceMode.Force);
        curGunAmmo--;
    }

    private void FireHitscan()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, hitscanRange, hitsacenLayer))
        {
            ObstcaleHealth eh = hit.collider.GetComponent<ObstcaleHealth>();
            if (eh != null)
            {
                eh.TakeDamage(hitScanDamage);
            }
        }
        curGunAmmo--;
    }


}
