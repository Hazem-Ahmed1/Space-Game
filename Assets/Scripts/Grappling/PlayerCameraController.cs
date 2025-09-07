//using UnityEngine;

//public class PlayerCameraController
//{
//    private readonly Transform _cameraFollowPos;
//    private readonly Transform _playerTransform;
//    private readonly float _sensitivity;

//    private float XAxis, YAxis;

//    public PlayerCameraController(Transform cameraFollowPos, Transform playerTransform, float sensitivity)
//    {
//        _cameraFollowPos = cameraFollowPos;
//        _playerTransform = playerTransform;
//        _sensitivity = sensitivity;
//    }

//    public void UpdateCameraAxis()
//    {
//        XAxis += Input.GetAxisRaw("Mouse X") * _sensitivity;
//        YAxis -= Input.GetAxisRaw("Mouse Y") * _sensitivity;
//        YAxis = Mathf.Clamp(YAxis, -90f, 90f);
//    }

//    public void ApplyRotation()
//    {
//        _playerTransform.rotation = Quaternion.Euler(0f, XAxis, 0f);
//        _cameraFollowPos.localRotation = Quaternion.Euler(YAxis, 0f, 0f);
//    }
//}



using UnityEngine;
using Cinemachine;

public class PlayerCameraController
{
    private readonly Transform _cameraFollowPos;
    private readonly Transform _playerTransform;
    private readonly float _sensitivity;

    private float XAxis, YAxis;

    // Cinemachine
    private CinemachineVirtualCamera _virtualCam;
    private float normalFOV = 70f;
    private float zoomFOV = 60f;
    private float zoomSpeed = 5f;
    private float targetFOV;

    public PlayerCameraController(Transform cameraFollowPos, Transform playerTransform, float sensitivity)
    {
        _cameraFollowPos = cameraFollowPos;
        _playerTransform = playerTransform;
        _sensitivity = sensitivity;

        _virtualCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        if (_virtualCam != null)
        {
            normalFOV = _virtualCam.m_Lens.FieldOfView;
            targetFOV = normalFOV;
        }
    }

    public void UpdateCameraAxis()
    {
        XAxis += Input.GetAxisRaw("Mouse X") * _sensitivity;
        YAxis -= Input.GetAxisRaw("Mouse Y") * _sensitivity;
        YAxis = Mathf.Clamp(YAxis, -90f, 90f);

        // Smooth zoom effect
        if (_virtualCam != null)
        {
            float currentFOV = _virtualCam.m_Lens.FieldOfView;
            _virtualCam.m_Lens.FieldOfView = Mathf.Lerp(
                currentFOV,
                targetFOV,
                Time.deltaTime * zoomSpeed
            );
        }
    }

    public void ApplyRotation()
    {
        _playerTransform.rotation = Quaternion.Euler(0f, XAxis, 0f);
        _cameraFollowPos.localRotation = Quaternion.Euler(YAxis, 0f, 0f);
    }

    // Called by states
    public void ZoomIn()
    {
        targetFOV = zoomFOV;
    }

    public void ZoomOut()
    {
        targetFOV = normalFOV;
    }
}
