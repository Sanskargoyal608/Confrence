using UnityEngine;
using Fusion;

public class VRLocomotionControllers : NetworkBehaviour
{
    public float moveSpeed = 2f;
    public Transform xrOrigin; // Reference to the XR Origin transform (child of VR_Player)

    // Simple joystick input variables (modify according to your input system)
    private Vector2 moveInput;

    void Update()
    {
        if (!HasInputAuthority) return;

        // Example using old Input API (you may replace this with your XR input method):
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        moveInput = new Vector2(horizontal, vertical);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasInputAuthority || xrOrigin == null) return;

        // Calculate movement in local space
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Runner.DeltaTime;
        xrOrigin.Translate(movement, Space.Self);

        // Optionally, you might update the networked avatar's position here,
        // if your XR Origin is not a child of the root, then copy its transform to the root.
        // For example:
        // transform.position = xrOrigin.position;
        // transform.rotation = xrOrigin.rotation;
    }
}
