using UnityEngine;
using Fusion;
using TMPro;

public class NetworkVRAvatar : NetworkBehaviour
{
    [Header("Networked Visual Components (Offline Avatar)")]
    // This is the part others see:
    public Transform avatarHead;  // Assign the visual head from the Offline_Player_Avatar

    [Header("Local Tracker References (from XR Origin)")]
    // These come from the local XR rig (only used if HasInputAuthority):
    public Transform headTracker;       // Assign the Main Camera transform from XR Origin
    public Transform leftController;    // (Optional for hand sync)
    public Transform rightController;   // (Optional for hand sync)

    [Header("UI")]
    public TMP_Text playerNameText;     // UI text on the Offline_Player_Avatar (e.g., above head)

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority)
        {
            // Sync the visual head from the XR tracking data.
            if (headTracker != null && avatarHead != null)
            {
                avatarHead.position = headTracker.position;
                avatarHead.rotation = headTracker.rotation;
            }
            // Add similar sync for hands if needed.
        }
    }

    public void SetPlayerName(string playerName)
    {
        if (playerNameText != null)
            playerNameText.text = playerName;
    }
}
