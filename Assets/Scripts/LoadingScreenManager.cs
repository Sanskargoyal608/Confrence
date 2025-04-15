using UnityEngine;

public class LoadingScreenManager : MonoBehaviour
{
    // Reference to the LoadingCanvas GameObject (assign in Inspector)
    public GameObject loadingCanvas;

    public void HideLoadingScreen()
    {
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(false);
        }
    }

    public void ShowLoadingScreen()
    {
        if (loadingCanvas != null)
        {
            loadingCanvas.SetActive(true);
        }
    }
}
