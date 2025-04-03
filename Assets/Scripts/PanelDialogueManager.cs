using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class PanelDialogueManager : MonoBehaviour
{
    public string panelCategory; // Set this in Inspector: "soft", "technical", "situational"
    public string candidateResponse = ""; // This can be updated with candidate input later
    public TMPro.TextMeshProUGUI dialogueDisplay; // Reference to a UI Text element (optional for debugging)

    // Your Microsoft Azure OpenAI endpoint and API key
    private string apiEndpoint = "https://YOUR_AZURE_OPENAI_ENDPOINT";
    private string apiKey = "YOUR_API_KEY";

    // Build the prompt based on panel category and candidate input
    private string BuildPrompt()
    {
        string prompt = "";
        switch (panelCategory.ToLower())
        {
            case "soft":
                prompt = "Act as a soft skills interviewer. Ask a follow-up question based on the candidate's answer: " + candidateResponse;
                break;
            case "technical":
                prompt = "Act as a technical interviewer. Ask a technical follow-up question based on the candidate's answer: " + candidateResponse;
                break;
            case "situational":
                prompt = "Act as an interviewer focused on situational questions. Ask a scenario-based follow-up question based on the candidate's answer: " + candidateResponse;
                break;
            default:
                prompt = "Ask a relevant interview question.";
                break;
        }
        return prompt;
    }

    // Call Azure OpenAI API asynchronously
    public async Task<string> GetAIResponse()
    {
        string prompt = BuildPrompt();
        // Create a JSON payload with your prompt and any parameters you need (e.g., max_tokens)
        string jsonPayload = "{\"prompt\": \"" + prompt + "\", \"max_tokens\": 100}";

        using (UnityWebRequest request = new UnityWebRequest(apiEndpoint, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API Error: " + request.error);
                return "Error retrieving response.";
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                // Parse the response as needed (assuming the response contains a field 'text')
                // For example purposes, we simply display the full JSON:
                if (dialogueDisplay != null)
                    dialogueDisplay.text = jsonResponse;
                return jsonResponse;
            }
        }
    }

    // For testing, trigger AI response on a key press (or tie this to a dialogue trigger)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) // Example: press '1' to trigger this panel's response
        {
            candidateResponse = "Example candidate answer."; // Update with actual candidate input
            StartCoroutine(TriggerResponse());
        }
    }

    IEnumerator TriggerResponse()
    {
        Task<string> task = GetAIResponse();
        yield return new WaitUntil(() => task.IsCompleted);
        string aiResponse = task.Result;
        Debug.Log("AI Response from " + panelCategory + " panel: " + aiResponse);
        // Here you can add additional logic, such as playing TTS audio or triggering animations
    }
}
