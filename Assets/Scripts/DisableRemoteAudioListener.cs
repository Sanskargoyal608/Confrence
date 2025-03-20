using UnityEngine;
using Fusion;

public class DisableRemoteAudioListener : MonoBehaviour
{
    private void Start()
    {
        // Get the NetworkObject component attached to the player prefab
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj != null)
        {
            // If this instance is not the local player, disable the AudioListener
            if (!netObj.HasInputAuthority)
            {
                // Disable all AudioListener components in children (if any)
                AudioListener[] listeners = GetComponentsInChildren<AudioListener>();
                foreach (var listener in listeners)
                {
                    listener.enabled = false;
                }
            }
        }
    }
}
