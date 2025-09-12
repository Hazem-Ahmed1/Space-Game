using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Transform _cameraFollowPos;
    [SerializeField] private float _mouseSensitivity = 1f;
    [SerializeField] public float speed = 12f;


    [HideInInspector] public PlayerMovement _movement;
    [HideInInspector] public PlayerCameraController _cameraController;
    [HideInInspector] public PlayerStateMachine _stateMachine;
    public Animator animator { get; private set; }

    // Settings for GrapplingState
    public readonly float stopDistance = 1f;  // how close to stop at the grapple point
    public readonly float resetDelay = 5f;    // backup reset if something goes wrong

    // States
    public IdleState idleState;
    public FloatState floatState;
    public GrapplingState grapplingState;
    public FreezeState freezeState;


    // GunShoot Event

    public LaserGun LaserGun;
    private void Awake()
    {
        idleState = new IdleState(this);
        floatState = new FloatState(this);
        grapplingState = new GrapplingState(this);
        freezeState = new FreezeState(this);

        _controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        LaserGun = GetComponentInChildren<LaserGun>();
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
    }
    private void Update()
    {
        _movement.ReadInput();
        _movement.Move();

        _cameraController.UpdateCameraAxis();

        _stateMachine.Update();

        animator.SetFloat("hInput", _movement.Direction.x);
        animator.SetFloat("vInput", _movement.Direction.z);
    }

    private void LateUpdate()
    {
        _cameraController.ApplyRotation();
    }
}
