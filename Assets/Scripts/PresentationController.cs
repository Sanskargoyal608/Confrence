using System.Collections;
using UnityEngine;
using Fusion;

public class PresentationController : NetworkBehaviour
{
    public static PresentationController Instance;
    public Renderer[] screenRenderers; // Renderers on screens in your environment
    private Texture2D[] slides;

    [Networked] public int CurrentSlide { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keeps this object alive across scene loads
            Debug.Log("PresentationController Instance assigned.");
        }
        else
        {
            Debug.LogWarning("Multiple instances of PresentationController detected! Destroying duplicate.");
            Destroy(gameObject); // Prevents duplicates
        }
    }


    // Called once slides (textures) are ready after PDF processing
    public void SetSlides(Texture2D[] newSlides)
    {
        if (Instance == null)
        {
            Debug.LogError("PresentationController Instance is NULL! Ensure it's assigned properly.");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            Debug.LogError("No State Authority! Only the presenter should set slides.");
            return;
        }

        slides = newSlides;
        SetCurrentSlide(0);
    }


    // Navigation methods to be hooked up to VR UI buttons (only for presenter)
    public void NextSlide()
    {
        if (!Object.HasStateAuthority || slides == null || CurrentSlide >= slides.Length - 1)
            return;
        SetCurrentSlide(CurrentSlide + 1);
    }

    public void PreviousSlide()
    {
        if (!Object.HasStateAuthority || slides == null || CurrentSlide <= 0)
            return;
        SetCurrentSlide(CurrentSlide - 1);
    }

    void SetCurrentSlide(int newIndex)
    {
        CurrentSlide = newIndex;
        RPC_UpdateSlide(newIndex);
    }

    // Fusion RPC to update all screens across the network
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateSlide(int slideIndex)
    {
        if (slides == null || slideIndex < 0 || slideIndex >= slides.Length)
            return;
        foreach (Renderer renderer in screenRenderers)
        {
            renderer.material.mainTexture = slides[slideIndex];
        }
    }
}
