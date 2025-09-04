using UnityEngine;

public class YPlayerManager : MonoBehaviour
{
    // movement variables----
    [SerializeField] private CharacterController _controller;
    public float speed = 12f;
    [HideInInspector] public Vector3 dir;
    private float _hzInput, _vtInput;

    // camera rotate with player variables----
    public float XAxis, YAxis;
    [SerializeField] private Transform _cameraFollowPos;
    [SerializeField] private float _mouseSensitivity = 1f; // tweak as you like

    // state machine / animator----
    public MovementBaseState currentState;
    public IdleState idleState = new IdleState();
    public FloatState floatState = new FloatState();
    [HideInInspector] public Animator animator;

    //---------OnLoad Methods---------
    private void Start()
    {
        if (!_controller) _controller = GetComponent<CharacterController>();
        if (!animator) animator = GetComponent<Animator>();
        SwitchState(idleState);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ReadInput();
        Move();
        UpdateCameraAxis();
        currentState.UpdateState(this);
        animator.SetFloat("hInput", _hzInput);
        animator.SetFloat("vInput", _vtInput);
    }

    void LateUpdate()
    {
        RotateCameraWithPlayer();
    }

    // movement methods----
    void ReadInput()
    {
        _hzInput = Input.GetAxis("Horizontal");
        _vtInput = Input.GetAxis("Vertical");
        dir = transform.right * _hzInput + transform.forward * _vtInput;
    }

    void Move()
    {
        _controller.Move(dir.normalized * speed * Time.deltaTime);
    }

    // camera rotate part methods----
    void UpdateCameraAxis()
    {
        XAxis += Input.GetAxisRaw("Mouse X") * _mouseSensitivity;
        YAxis -= Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
        YAxis = Mathf.Clamp(YAxis, -90f, 90f);
    }

    void RotateCameraWithPlayer()
    {
        // horizontal yaw on the player
        transform.rotation = Quaternion.Euler(0f, XAxis, 0f);

        // vertical pitch on the camera pivot
        _cameraFollowPos.localRotation = Quaternion.Euler(YAxis, 0f, 0f);
    }

    // state machine control-------
    public void SwitchState(MovementBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }
}
