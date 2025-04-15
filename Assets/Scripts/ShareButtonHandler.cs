using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class ShareButtonHandler : MonoBehaviour
{
    // Reference to your existing PDFManager (or file conversion manager) script.
    public PDFManager pdfManager;

    // The URL of your Python server endpoint that returns the shared URL.
    // Make sure to replace YOUR_SERVER_IP with your server’s IP or domain.
    public string sharedUrlEndpoint = "http://192.168.137.206:5000/get_shared_url";


    // (Optional) A TextMeshPro text for on-screen debugging.
    public TMP_Text debugText;

    // This method should be linked to your VR Share button's OnClick event.
    public void OnShareButtonPressed()
    {
        Debug.Log("Share button pressed. Fetching shared URL...");
        if (debugText != null)
        {
            debugText.text = "Fetching shared URL...";
        }
        StartCoroutine(GetSharedURL());
    }

    IEnumerator GetSharedURL()
    {
        UnityWebRequest request = UnityWebRequest.Get(sharedUrlEndpoint);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching shared URL: " + request.error);
            if (debugText != null)
                debugText.text = "Error: " + request.error;
            yield break;
        }

        string jsonResponse = request.downloadHandler.text;
        Debug.Log("Shared URL response: " + jsonResponse);
        if (debugText != null)
            debugText.text = "Response: " + jsonResponse;

        // Parse the JSON response.
        SharedURLResponse response = JsonUtility.FromJson<SharedURLResponse>(jsonResponse);
        if (response == null || string.IsNullOrEmpty(response.url))
        {
            Debug.LogError("No URL retrieved from server.");
            if (debugText != null)
                debugText.text = "No URL retrieved.";
            yield break;
        }

        Debug.Log("Fetched shared URL: " + response.url);
        if (debugText != null)
            debugText.text = "URL: " + response.url;

        // Trigger your existing file conversion and processing method.
        // For example, if your PDFManager has a method called DownloadAndProcessPDF(),
        // you may want to rename or modify it to DownloadAndProcessFile() so it works for both PDFs and PPTX.
        pdfManager.DownloadAndProcessFile(response.url);

    }

    [System.Serializable]
    public class SharedURLResponse
    {
        public string url;
    }
}
