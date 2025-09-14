using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class BlackHoleCore : MonoBehaviour
{
    [Header("Core Settings")]
    [Tooltip("The initial visual size of the black hole.")]
    public float startScale = 10f;
    [Tooltip("How much the black hole grows when it consumes an object.")]
    public float growthPerObject = 1f;
    [Tooltip("The duration of the initial scale-up animation.")]
    public float scaleUpDuration = 2f;

    [Header("Attraction Settings")]
    [Tooltip("The radius within which objects are pulled towards the black hole.")]
    public float attractionRadius = 10f;
    [Tooltip("The force applied to rigidbodies.")]
    public float attractionForce = 200f;
    [Tooltip("Layers of objects that the black hole can affect.")]
    public LayerMask affectedLayers = ~0;

    [Header("Movement Settings")]
    [Tooltip("Whether the black hole should follow the player.")]
    public bool followsPlayer = true;
    [Tooltip("The speed at which the black hole follows the player.")]
    public float followSpeed = 5f;
    private Transform playerTransform;

    [Header("Destruction Settings")]
    [Tooltip("The radius at which objects are consumed.")]
    public float destroyRadius = 0.5f;

    private SphereCollider attractionSphere;

    void Awake()
    {
        attractionSphere = GetComponent<SphereCollider>();
        attractionSphere.isTrigger = true;

        // Find the player object by its tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void Start()
    {
        // Start the initial scale-up animation
        StartCoroutine(ScaleUpAnimation());
    }

    void FixedUpdate()
    {
        // If enabled, move the black hole towards the player
        if (followsPlayer && playerTransform != null)
        {
            Vector3 targetPosition = playerTransform.position;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.fixedDeltaTime);
        }

        // Attraction logic
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, attractionRadius, affectedLayers);

        foreach (Collider obj in objectsInRange)
        {
            if (obj.gameObject == this.gameObject)
            {
                continue;
            }

            IAttractable attractable = obj.GetComponent<IAttractable>();
            if (attractable != null)
            {
                attractable.Attract(transform.position, attractionForce, destroyRadius);
            }
        }
    }

    /// <summary>
    /// Coroutine to smoothly animate the black hole's scale at the start.
    /// </summary>
    private IEnumerator ScaleUpAnimation()
    {
        Vector3 startScaleVec = Vector3.one * 0.1f;
        Vector3 targetScaleVec = Vector3.one * startScale;
        float elapsedTime = 0f;

        while (elapsedTime < scaleUpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / scaleUpDuration;
            t = t * t * (3f - 2f * t);
            transform.localScale = Vector3.Lerp(startScaleVec, targetScaleVec, t);
            yield return null;
        }

        transform.localScale = targetScaleVec;
        UpdateAttractionRadius();
    }

    /// <summary>
    /// Grows the black hole and updates its attraction radius.
    /// </summary>
    public void Grow()
    {
        transform.localScale += Vector3.one * growthPerObject;
        UpdateAttractionRadius();
    }

    private void UpdateAttractionRadius()
    {
        attractionRadius = startScale * (transform.localScale.x / startScale);
        attractionSphere.radius = attractionRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destroyRadius);
    }
}