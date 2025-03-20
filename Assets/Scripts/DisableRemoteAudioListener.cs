using UnityEngine;
using Fusion;

public class DisableRemoteAudioListener : MonoBehaviour
{
    private void Start()
    {
        // Get the NetworkObject component attached to this player
        NetworkObject netObj = GetComponent<NetworkObject>();
        if (netObj != null)
        {
            // If this instance is not the local player, disable all AudioListeners in children
            if (!netObj.HasInputAuthority)
            {
                AudioListener[] listeners = GetComponentsInChildren<AudioListener>();
                foreach (var listener in listeners)
                {
                    listener.enabled = false;
                }
            }
        }
    }
}
