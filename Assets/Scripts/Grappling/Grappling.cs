using UnityEngine;
public class Grappling : MonoBehaviour
{
    [Header("References")]
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling Settings")]
    public float maxGrappleDistance = 25f;
    public float grappleDelayTime = 0.2f;
    public float overshootYAxis = 2f;

    [Header("Cooldown")]
    public float grapplingCd = 2f;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;
    public KeyCode LasereKey = KeyCode.Mouse0;

    private Vector3 grapplePoint;
    private bool grappling;

    // State machine reference
    private PlayerManager player;

    private void Start()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey))
            StartGrapple();

        if (Input.GetKeyUp(LasereKey))
        {
            player.ShootLaser();
            player.animator.SetTrigger("LeftHandShoot");
        }
        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    private void StartGrapple()
    {
        player.animator.SetTrigger("RightHandShoot");
        if (grapplingCdTimer > 0) return;

        grappling = true;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
    }

    private void ExecuteGrapple()
    {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        // Switch state → Grappling
        player._stateMachine.SwitchState(player.grapplingState);
        player.grapplingState.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1.5f);
    }

    public void StopGrapple()
    {
        grappling = false;
        grapplingCdTimer = grapplingCd;

        // return to Idle after grapple ends
        if (player._stateMachine.CurrentState == player.grapplingState)
        {
            player._stateMachine.SwitchState(player.idleState);
        }
    }

    public bool IsGrappling() => grappling;

    public Vector3 GetGrapplePoint() => grapplePoint;
}

