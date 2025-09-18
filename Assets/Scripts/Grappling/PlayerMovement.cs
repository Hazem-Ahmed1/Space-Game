using UnityEngine;

public class PlayerMovement
{
    private readonly CharacterController _controller;
    private readonly float _speed;

    public Vector3 Direction { get; set; }
    public Vector3 ExternalForce { get; set; } // New property for external forces

    public PlayerMovement(CharacterController controller, float speed)
    {
        _controller = controller;
        _speed = speed;
    }

    public void ReadInput()
    {
        float hz = Input.GetAxis("Horizontal");
        float vt = Input.GetAxis("Vertical");
        Direction = _controller.transform.right * hz + _controller.transform.forward * vt;
    }

    public void Move()
    {
        // Add the external force to the player's normal direction
        Vector3 totalDirection = Direction.normalized * _speed + ExternalForce;
        _controller.Move(totalDirection * Time.deltaTime);
    }
}