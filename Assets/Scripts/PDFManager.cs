using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PDFManager : MonoBehaviour
{
    public PresentationController presentationController;

    // Call this method from your UI when the user submits the PDF URL
    public void DownloadAndProcessPDF(string pdfUrl)
    {
        Debug.Log("Starting PDF download: " + pdfUrl);
        StartCoroutine(DownloadPDFCoroutine(pdfUrl));
    }

    // Step 1: Download the PDF file from the provided URL
    IEnumerator DownloadPDFCoroutine(string pdfUrl)
    {
        Debug.Log("Downloading PDF from: " + pdfUrl);
        UnityWebRequest pdfRequest = UnityWebRequest.Get(pdfUrl);
        yield return pdfRequest.SendWebRequest();

        if (pdfRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading PDF: " + pdfRequest.error);
            yield break;
        }

        byte[] pdfData = pdfRequest.downloadHandler.data;
        // Step 2: Upload the PDF data to the Flask server for conversion
        StartCoroutine(UploadPDFToServer(pdfData));
    }

    // Upload the PDF to your Flask server and process it
    IEnumerator UploadPDFToServer(byte[] pdfData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("file", pdfData, "presentation.pdf", "application/pdf");

        // Replace with your server's URL (e.g., "http://192.168.1.100:5000/convert_pdf")
        string serverUrl = "http://192.168.47.13:5000/convert_file";

        UnityWebRequest uploadRequest = UnityWebRequest.Post(serverUrl, form);
        yield return uploadRequest.SendWebRequest();

        if (uploadRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error converting PDF: " + uploadRequest.error);
            yield break;
        }

        // Step 3: Parse the JSON response containing image URLs
        // Expected format: {"images": ["http://.../slide_0.png", "http://.../slide_1.png", ...]}
        string jsonResponse = uploadRequest.downloadHandler.text;
        Debug.Log("Server Response: " + jsonResponse);
        PDFConversionResponse response = JsonUtility.FromJson<PDFConversionResponse>(jsonResponse);

        if (response == null || response.images == null || response.images.Length == 0)
        {
            Debug.LogError("No images returned from conversion.");
            yield break;
        }

        // Step 4: Download each image
        yield return StartCoroutine(DownloadImages(response.images));
    }

    // Download all slide images and pass them to the PresentationController
    IEnumerator DownloadImages(string[] imageUrls)
    {
        List<Texture2D> downloadedSlides = new List<Texture2D>();

        foreach (string url in imageUrls)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to download image: " + url);
                }
                else
                {
                    downloadedSlides.Add(DownloadHandlerTexture.GetContent(www));
                }
            }
        }

        if (downloadedSlides.Count > 0)
        {
            StartCoroutine(WaitForPresentationController(downloadedSlides.ToArray()));
        }
        else
        {
            Debug.LogError("No images downloaded, cannot update PresentationController.");
        }
    }

    IEnumerator WaitForPresentationController(Texture2D[] slides)
    {
        while (PresentationController.Instance == null)
        {
            Debug.LogWarning("Waiting for PresentationController to be initialized...");
            yield return new WaitForSeconds(0.1f);
        }

        Debug.Log("PresentationController found! Setting slides...");
        PresentationController.Instance.SetSlides(slides);
    }




    // Helper class for JSON deserialization of the server response
    [System.Serializable]
    public class PDFConversionResponse
    {
        public string[] images;
    }
}
