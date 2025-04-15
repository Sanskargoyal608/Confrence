using UnityEngine;
using Fusion;

public class PlayerVRManager : NetworkBehaviour
{
    public GameObject xrOrigin;

    private bool _initialized = false;

    void Start()
    {
        if (xrOrigin == null) return;

        if (HasInputAuthority)
        {
            xrOrigin.SetActive(true);
            Debug.Log("Local XR Origin active.");

            // Move XR Origin to match network spawn position
            xrOrigin.transform.position = transform.position;
            xrOrigin.transform.rotation = transform.rotation;

            _initialized = true;
        }
        else
        {
            xrOrigin.SetActive(false);
            Debug.Log("Disabling XR Origin for remote player.");
        }
    }

    void Update()
    {
        // Ensure XR Origin stays aligned (for safety against override after scene load)
        if (!_initialized && HasInputAuthority && xrOrigin != null)
        {
            xrOrigin.transform.position = transform.position;
            xrOrigin.transform.rotation = transform.rotation;
            _initialized = true;
        }
    }
}
