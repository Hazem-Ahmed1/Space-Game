//using UnityEngine;

//public class GrapplingHook : MonoBehaviour
//{
//    [Header("Aiming Settings")]
//    public float maxGrappleDistance = 100f;
//    public LayerMask grappleableLayers;
//    public GameObject grappleIndicatorPrefab; // Optional: a prefab to show the aim point
//    public Transform indicatorParent; // Optional: a parent for the indicator

//    private Camera mainCamera;
//    private GameObject grappleIndicatorInstance;
//    private PlayerManager playerManager;

//    void Start()
//    {
//        mainCamera = Camera.main;
//        playerManager = GetComponent<PlayerManager>();

//        // Instantiate the visual indicator
//        if (grappleIndicatorPrefab != null && indicatorParent != null)
//        {
//            grappleIndicatorInstance = Instantiate(grappleIndicatorPrefab, indicatorParent);
//            grappleIndicatorInstance.SetActive(false);
//        }
//    }

//    void Update()
//    {
//        // Aiming logic
//        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

//        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleableLayers))
//        {
//            // Show the grapple indicator on the hit point
//            if (grappleIndicatorInstance != null)
//            {
//                grappleIndicatorInstance.transform.position = hit.point;
//                grappleIndicatorInstance.SetActive(true);
//            }

//            // Fire the grappling hook on button press
//            if (Input.GetMouseButtonDown(0))
//            {
//                // Switch to the GrapplingState and pass the hit point as the target
//                if (playerManager._stateMachine.CurrentState != playerManager.grapplingState)
//                {
//                    playerManager._stateMachine.SwitchState(playerManager.grapplingState);
//                    playerManager.grapplingState.StartGrapple(hit.point);
//                }
//            }
//        }
//        else
//        {
//            // Hide the indicator if not aiming at a grappleable object
//            if (grappleIndicatorInstance != null)
//            {
//                grappleIndicatorInstance.SetActive(false);
//            }
//        }
//    }
//}