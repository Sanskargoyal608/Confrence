using UnityEngine;

using System;

public class UniqueChildRenamer : MonoBehaviour
{
    void Awake()
    {
        foreach (Transform child in transform)
        {
            // Optionally check if this child has an XR interactable component
            if (child.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>() != null)
            {
                child.gameObject.name = $"{child.gameObject.name}_{Guid.NewGuid()}";
            }
        }
    }
}
