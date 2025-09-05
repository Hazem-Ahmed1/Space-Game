using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class PlayerCamV : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    public CinemachineVirtualCamera virtualCam; // Drag your Virtual Camera here in the Inspector

    [Header("Sensitivity")]
    public float sensX = 300f;
    public float sensY = 300f;
    public float multiplier = 1f;

    [Header("FOV Settings")]
    public bool useFluentFov = true;
    public float minMovementSpeed = 5f;
    public float maxMovementSpeed = 20f;
    public float minFov = 60f;
    public float maxFov = 90f;

    private CinemachinePOV pov;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (virtualCam != null)
        {
            pov = virtualCam.GetCinemachineComponent<CinemachinePOV>();

            if (pov != null)
            {
                // Set Cinemachine POV sensitivity
                pov.m_HorizontalAxis.m_MaxSpeed = sensX * multiplier;
                pov.m_VerticalAxis.m_MaxSpeed = sensY * multiplier;
            }
        }
    }

    private void Update()
    {
        if (useFluentFov) HandleFov();
    }

    private void HandleFov()
    {
        if (rb == null || virtualCam == null) return;

        float moveSpeedDif = maxMovementSpeed - minMovementSpeed;
        float fovDif = maxFov - minFov;

        float rbFlatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z).magnitude;
        float currMoveSpeedOvershoot = rbFlatVel - minMovementSpeed;
        float currMoveSpeedProgress = currMoveSpeedOvershoot / moveSpeedDif;

        float targetFov = Mathf.Clamp((currMoveSpeedProgress * fovDif) + minFov, minFov, maxFov);

        float currFov = virtualCam.m_Lens.FieldOfView;
        float lerpedFov = Mathf.Lerp(currFov, targetFov, Time.deltaTime * 5f);

        virtualCam.m_Lens.FieldOfView = lerpedFov;
    }

    public void DoFov(float endValue)
    {
        if (virtualCam == null) return;
        DOTween.To(() => virtualCam.m_Lens.FieldOfView, x => virtualCam.m_Lens.FieldOfView = x, endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        // If you want tilt, apply it to the Virtual Camera itself
        if (virtualCam != null)
        {
            virtualCam.transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
        }
    }
}
