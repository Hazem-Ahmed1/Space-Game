using UnityEngine;

public class LaserGun : MonoBehaviour
{
    [Header("Setup")]
    public Transform firePoint;
    public LaserProjectilePool laserPool;
    public AudioClip shootSound;
    public float laserSpeed = 50f;
    public float maxDistance = 100f;  // how far the crosshair can aim

    private AudioSource audioSource;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Called by Animation Event
    public void Shoot()
    {
        if (laserPool != null && firePoint != null)
        {
            // Get world target position from crosshair (center of screen)
            Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                targetPoint = hit.point; // hit something
            }
            else
            {
                targetPoint = ray.GetPoint(maxDistance); // shoot straight ahead
            }

            // Calculate direction
            Vector3 dir = (targetPoint - firePoint.position).normalized;

            // Get a laser from pool
            GameObject laserObj = laserPool.GetFromPool();
            laserObj.transform.position = firePoint.position;
            laserObj.transform.rotation = Quaternion.LookRotation(dir);

            LaserProjectile laser = laserObj.GetComponent<LaserProjectile>();
            if (laser != null)
            {
                laser.Init(laserPool, dir, laserSpeed);
            }
        }

        if (shootSound != null)
            audioSource.PlayOneShot(shootSound);
    }
}
