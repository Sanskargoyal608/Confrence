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
            Debug.LogError("PresentationController Instance is NULL!");
            return;
        }

        if (!Object.HasStateAuthority)
        {
            Debug.LogError("No State Authority! Only presenter should set slides.");
            return;
        }

        if (newSlides == null || newSlides.Length == 0)
        {
            Debug.LogError("SetSlides: newSlides is null or empty!");
            return;
        }

        slides = newSlides;
        Debug.Log("Slides loaded: " + slides.Length);
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
        if (slides == null)
        {
            Debug.LogError("Slides array is null!");
            return;
        }

        if (slideIndex < 0 || slideIndex >= slides.Length)
        {
            Debug.LogError("Invalid slide index: " + slideIndex);
            return;
        }

        if (slides[slideIndex] == null)
        {
            Debug.LogError("Slide texture at index " + slideIndex + " is null!");
            return;
        }

        foreach (Renderer renderer in screenRenderers)
        {
            if (renderer == null)
            {
                Debug.LogError("Renderer in screenRenderers is null.");
                continue;
            }

            if (renderer.material == null)
            {
                Debug.LogWarning("Renderer has no material. Assigning new default material.");
                renderer.material = new Material(Shader.Find("Unlit/Texture"));
            }

            renderer.material.mainTexture = slides[slideIndex];
        }
    }

}
