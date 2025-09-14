using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAiming : MonoBehaviour
{
    [Header("Rigging References")]
    public Rig aimRig;
    public Transform aimTarget;

    [Header("Aiming Settings")]
    public float aimDistance = 50f;
    public LayerMask aimLayerMask; // The layers the raycast can hit

    private Animator animator;
    private Camera mainCamera;

    // A flag to control aiming behavior
    private bool isAiming = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        // Hide the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Toggle aiming on right mouse button click
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = !isAiming;
        }

        // Smoothly blend the rig's weight
        float targetWeight = isAiming ? 1f : 0f;
        aimRig.weight = Mathf.Lerp(aimRig.weight, targetWeight, Time.deltaTime * 10f);

        // Update the aim target position using a raycast from the screen center
        Vector3 aimPoint = mainCamera.transform.position + mainCamera.transform.forward * aimDistance;
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, aimDistance, aimLayerMask))
        {
            aimPoint = hit.point;
        }

        // Set the position of the aim target object
        aimTarget.position = aimPoint;

        // You would also set an animator parameter here, if needed, for base aiming animations.
        //animator.SetBool("IsAiming", isAiming);

        // Example for firing logic
        if (isAiming && Input.GetMouseButtonDown(0))
        {
            // FireGun logic goes here
            // animator.SetTrigger("Fire");
        }
    }
}