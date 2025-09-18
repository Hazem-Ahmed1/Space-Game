using System.Collections;
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
    [SerializeField] private float shootCooldown = 0.5f;

    [Header("Aiming Constraints")]
    [SerializeField] private MultiAimConstraint handLeftAim;    // hand L looks at target
    [SerializeField] private MultiAimConstraint handRightAim;    // hand R looks at target
    [SerializeField] private TwoBoneIKConstraint leftHandIK;  // Left hand on gun
    [SerializeField] private TwoBoneIKConstraint rightHandIK; // Right hand on grapple

    private float handLeftAimVel;
    private float handRightAimVel;
    private float leftHandVel;
    private float rightHandVel;

    // Flags for managing state
    private bool _canShoot = true;
    private bool _isShooting = false;

    // A reference to a rig layer if you have one
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
    public FireLaserState fireLaserState;

    public LaserGun LaserGun;

    private Camera mainCam;

    private void Awake()
    {
        idleState = new IdleState(this);
        floatState = new FloatState(this);
        grapplingState = new GrapplingState(this);
        freezeState = new FreezeState(this);
        deadState = new DeadState(this);
        fireLaserState = new FireLaserState(this);

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
        if (_canShoot)
        {
            StartCoroutine(ShootLaserRoutine());
        }
    }

    private IEnumerator ShootLaserRoutine()
    {
        _canShoot = false;
        _isShooting = true;

        // Play shoot animation
        //animator.SetTrigger("Shoot");

        // Give a small moment for the IK to snap into place before firing the bullet
        yield return new WaitForSeconds(0.05f);

        // Perform the shooting action
        LaserGun.Shoot();

        // Wait for shoot cooldown / animation duration
        yield return new WaitForSeconds(shootCooldown);

        _isShooting = false; // Deactivate shooting flag to blend IK back
        _canShoot = true; // Allow shooting again
    }

    private void Update()
    {
        _movement.ReadInput();

        HandleBlackHoleAttraction();
        HandleAiming(); // This method updates the aimTarget position
        HandleShootingInput();

        _movement.Move();
        _cameraController.UpdateCameraAxis();
        _stateMachine.Update();

        animator.SetFloat("hInput", _movement.Direction.x);
        animator.SetFloat("vInput", _movement.Direction.z);

        HandleRigWeights();
    }

    private void LateUpdate()
    {
        _cameraController.ApplyRotation();
    }

    private void HandleShootingInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShootLaser();
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

    private void HandleRigWeights()
    {
        // Shooting IK: This rig will activate only for a brief moment to align the weapon
        float targetShootWeight = _isShooting ? 1f : 0f;
        handLeftAim.weight = Mathf.SmoothDamp(handLeftAim.weight, targetShootWeight, ref handLeftAimVel, 0.05f); // Fast snap
        leftHandIK.weight = Mathf.SmoothDamp(leftHandIK.weight, targetShootWeight, ref leftHandVel, 0.05f); // Fast snap

        // Right hand for grappling (unaffected by shooting)
        bool grappling = _stateMachine.CurrentState == grapplingState || Input.GetKeyDown(KeyCode.Mouse1);
        float targetGrappleWeight = grappling ? 1f : 0f;
        Debug.Log(_stateMachine.CurrentState);
        handRightAim.weight = Mathf.SmoothDamp(handRightAim.weight, targetGrappleWeight, ref handRightAimVel, 0.05f);
        rightHandIK.weight = Mathf.SmoothDamp(rightHandIK.weight, targetGrappleWeight, ref rightHandVel, 0.05f);
    }
}