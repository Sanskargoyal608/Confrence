using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PDFUIHandler : MonoBehaviour
{
    public TMP_InputField pdfUrlInputField;
    public PDFManager pdfManager; // Reference to the manager that handles PDF processing

    // Called when the Submit button is pressed
    public void OnLoadPDFButtonPressed()
    {
        string url = "https://drive.google.com/uc?export=download&id=1ink0VbVa-esPDPmLFtN7-ZaJ8IzDEDM2";

        Debug.Log("PDF URL: " + url); // Debug log to check the URL before processing

        if (!string.IsNullOrEmpty(url))
        {
            pdfManager.DownloadAndProcessPDF(url);
        }
        else
        {
            Debug.LogError("URL is empty or null.");
        }
    }
}
