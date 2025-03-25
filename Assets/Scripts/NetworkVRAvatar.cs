using UnityEngine;
using Fusion;
using TMPro;
using static Unity.Collections.Unicode;

public class NetworkVRAvatar : NetworkBehaviour
{
    [Header("VR Tracking References")]
    public Transform head;           // Visual head (e.g., from XR rig)
    public Transform leftHand;       // Visual left hand model
    public Transform rightHand;      // Visual right hand model

    [Header("Local Tracker References")]
    public Transform headTracker;    // Local VR headset transform (from XR Origin)
    public Transform leftController; // Local left controller transform
    public Transform rightController;// Local right controller transform

    [Header("UI")]
    public TMP_Text playerNameText; // Reference to the TextMeshPro in the NameUI canvas

    public override void FixedUpdateNetwork()
    {
        // Only update for local player input
        if (HasInputAuthority)
        {
            // Update the head and hand positions based on local VR tracking
            if (headTracker != null)
            {
                head.position = headTracker.position;
                head.rotation = headTracker.rotation;
            }
            if (leftController != null)
            {
                leftHand.position = leftController.position;
                leftHand.rotation = leftController.rotation;
            }
            if (rightController != null)
            {
                rightHand.position = rightController.position;
                rightHand.rotation = rightController.rotation;
            }
        }
    }

    // Call this to set the player's name (called when the player joins)
    public void SetPlayerName(string playerName)
    {
        if (playerNameText != null)
        {
            playerNameText.text = playerName;
        }
    }
}
