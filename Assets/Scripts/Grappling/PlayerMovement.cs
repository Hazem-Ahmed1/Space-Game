using UnityEngine;

public class PlayerMovement
{
    private readonly CharacterController _controller;
    private readonly float _speed;

    public Vector3 Direction { get;  set; }

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
        _controller.Move(Direction.normalized * _speed * Time.deltaTime);
    }
}

