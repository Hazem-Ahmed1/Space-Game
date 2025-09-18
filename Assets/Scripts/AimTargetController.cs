using UnityEngine;

public class AimTargetController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private RectTransform crosshair; // UI crosshair
    [SerializeField] private LayerMask aimLayerMask;  // e.g., environment

    [SerializeField] private Transform aimTarget; // The target object for rig

    void Update()
    {
        // Get screen position of crosshair
        Vector2 screenPos = crosshair.position;

        Ray ray = playerCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimLayerMask))
        {
            // Move AimTarget to hit point
            aimTarget.position = hit.point;
        }
        else
        {
            // If nothing hit, aim far in front
            aimTarget.position = ray.origin + ray.direction * 100f;
        }
    }
}
