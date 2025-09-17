//using UnityEngine;

//public class PlayerManager : MonoBehaviour
//{
//    [SerializeField] private CharacterController _controller;
//    [SerializeField] private Transform _cameraFollowPos;
//    [SerializeField] private float _mouseSensitivity = 1f;
//    [SerializeField] public float speed = 12f;

//    [HideInInspector] public PlayerMovement _movement;
//    [HideInInspector] public PlayerCameraController _cameraController;
//    [HideInInspector] public PlayerStateMachine _stateMachine;
//    public Animator animator { get; private set; }

//    // New component references
//    private AttractableCharacterController _attractableController;
//    private BlackHoleCore _blackHole;

//    // Settings for GrapplingState
//    public readonly float stopDistance = 1f; // how close to stop at the grapple point
//    public readonly float resetDelay = 5f; // backup reset if something goes wrong

//    // States
//    public IdleState idleState;
//    public FloatState floatState;
//    public GrapplingState grapplingState;
//    public FreezeState freezeState;
//    public DeadState deadState;

//    // GunShoot Event
//    public LaserGun LaserGun;

//    private void Awake()
//    {
//        idleState = new IdleState(this);
//        floatState = new FloatState(this);
//        grapplingState = new GrapplingState(this);
//        freezeState = new FreezeState(this);
//        deadState = new DeadState(this);

//        _controller = GetComponent<CharacterController>();
//        animator = GetComponent<Animator>();

//        LaserGun = GetComponentInChildren<LaserGun>();
//        _attractableController = GetComponent<AttractableCharacterController>(); // Get the attractable component
//        _blackHole = FindObjectOfType<BlackHoleCore>(); // Find the black hole in the scene
//    }

//    private void Start()
//    {
//        _movement = new PlayerMovement(_controller, speed);
//        _cameraController = new PlayerCameraController(_cameraFollowPos, transform, _mouseSensitivity);
//        _stateMachine = new PlayerStateMachine(this);

//        _stateMachine.SwitchState(idleState);

//        Cursor.lockState = CursorLockMode.Locked;
//        Cursor.visible = false;
//    }

//    public void ShootLaser()
//    {
//        LaserGun.Shoot();
//        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10));
//    }

//    private void Update()
//    {
//        _movement.ReadInput();

//        // Handle black hole attraction
//        if (_blackHole != null && _attractableController != null)
//        {
//            float distanceToBlackHole = Vector3.Distance(transform.position, _blackHole.transform.position);

//            if (distanceToBlackHole <= _blackHole.attractionRadius)
//            {
//                // Calculate the attraction force from the AttractableCharacterController
//                Vector3 attractionForce = _attractableController.GetAttractionForce(_blackHole.transform.position, _blackHole.attractionForce);
//                _movement.ExternalForce = attractionForce;
//            }
//            else
//            {
//                // Reset the external force when outside the attraction radius
//                _movement.ExternalForce = Vector3.zero;
//            }
//        }
//        else
//        {
//            _movement.ExternalForce = Vector3.zero;
//        }

//        _movement.Move();

//        _cameraController.UpdateCameraAxis();

//        _stateMachine.Update();

//        animator.SetFloat("hInput", _movement.Direction.x);
//        animator.SetFloat("vInput", _movement.Direction.z);
//    }

//    private void LateUpdate()
//    {
//        _cameraController.ApplyRotation();
//    }
//}

using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _cameraFollowPos;
    [SerializeField] private float _mouseSensitivity = 1f;
    [SerializeField] public float speed = 12f;

    [Header("Aiming")]
    [SerializeField] private Transform aimTarget; // Empty GameObject used for IK
    [SerializeField] private LayerMask aimLayerMask; // What the crosshair can hit
    [SerializeField] private float aimSmoothSpeed = 10f;

    [Header("Rig")]
    [SerializeField] private Rig aimRig; // Reference to your rig layer
    private float rigWeightVelocity; // for smooth damping

    [HideInInspector] public PlayerMovement _movement;
    [HideInInspector] public PlayerCameraController _cameraController;
    [HideInInspector] public PlayerStateMachine _stateMachine;
    // Settings for GrapplingState
    public readonly float stopDistance = 1f; // how close to stop at the grapple point
    public readonly float resetDelay = 5f; // backup reset if something goes wrong

    public Animator animator { get; private set; }

    private AttractableCharacterController _attractableController;
    private BlackHoleCore _blackHole;

    // States
    public IdleState idleState;
    public FloatState floatState;
    public GrapplingState grapplingState;
    public FreezeState freezeState;
    public DeadState deadState;

    public LaserGun LaserGun;

    private Camera mainCam;

    private void Awake()
    {
        idleState = new IdleState(this);
        floatState = new FloatState(this);
        grapplingState = new GrapplingState(this);
        freezeState = new FreezeState(this);
        deadState = new DeadState(this);

        _controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        LaserGun = GetComponentInChildren<LaserGun>();
        _attractableController = GetComponent<AttractableCharacterController>();
        _blackHole = FindObjectOfType<BlackHoleCore>();

        mainCam = Camera.main;
    }

    private void Start()
    {
        _movement = new PlayerMovement(_controller, speed);
        _cameraController = new PlayerCameraController(_cameraFollowPos, transform, _mouseSensitivity);
        _stateMachine = new PlayerStateMachine(this);

        _stateMachine.SwitchState(idleState);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ShootLaser()
    {
        LaserGun.Shoot();
        //animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10));
    }

    private void Update()
    {
        _movement.ReadInput();

        HandleBlackHoleAttraction();
        HandleAiming();

        _movement.Move();
        _cameraController.UpdateCameraAxis();
        _stateMachine.Update();

        animator.SetFloat("hInput", _movement.Direction.x);
        animator.SetFloat("vInput", _movement.Direction.z);

        HandleRigWeight();
    }

    private void LateUpdate()
    {
        {
            _cameraController.ApplyRotation();
        }
    }

    private void HandleBlackHoleAttraction()
    {
        if (_blackHole != null && _attractableController != null)
        {
            float distanceToBlackHole = Vector3.Distance(transform.position, _blackHole.transform.position);

            if (distanceToBlackHole <= _blackHole.attractionRadius)
            {
                Vector3 attractionForce = _attractableController.GetAttractionForce(_blackHole.transform.position, _blackHole.attractionForce);
                _movement.ExternalForce = attractionForce;
            }
            else
            {
                _movement.ExternalForce = Vector3.zero;
            }
        }
        else
        {
            _movement.ExternalForce = Vector3.zero;
        }
    }

    private void HandleAiming()
    {
        if (aimTarget == null) return;

        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimLayerMask))
        {
            aimTarget.position = Vector3.Lerp(aimTarget.position, hit.point, Time.deltaTime * aimSmoothSpeed);
        }
        else
        {
            Vector3 farPoint = ray.origin + ray.direction * 100f;
            aimTarget.position = Vector3.Lerp(aimTarget.position, farPoint, Time.deltaTime * aimSmoothSpeed);
        }
    }

    private void HandleRigWeight()
    {
        if (aimRig == null) return;

        // Player is aiming or shooting
        bool aiming = Input.GetMouseButton(1) || Input.GetMouseButton(0);

        float targetWeight = aiming ? 1f : 0f;

        // Smooth transition (damping)
        aimRig.weight = Mathf.SmoothDamp(aimRig.weight, targetWeight, ref rigWeightVelocity, 0.1f);
    }
}
