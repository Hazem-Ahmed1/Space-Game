//using UnityEngine;
//using Cinemachine;
//using TreeEditor;


//public class ThirdPersonShooterController : MonoBehaviour
//{
//    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
//    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
//    public GameObject Deboug;

//    // Start is called before the first frame update
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //Vector3 mouseWorldPosition = Vector3.zero;
//        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
//        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
//        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
//        {
//            Deboug.transform.position = raycastHit.point;
//        }

//        //if (Input.GetKey(KeyCode.Mouse0))
//        //{
//        //    aimVirtualCamera.gameObject.SetActive(true);

//        //    Vector3 worldAimTarget = mouseWorldPosition;
//        //    worldAimTarget.y = transform.position.y;
//        //    Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

//        //    if (aimDirection != Vector3.zero)
//        //    {
//        //        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
//        //    }
//        //}
//        //else
//        //{
//        //    aimVirtualCamera.gameObject.SetActive(false);
//        //}
//    }
//}


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
