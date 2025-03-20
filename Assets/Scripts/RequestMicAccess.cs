using UnityEngine;

public class RequestMicAccess : MonoBehaviour
{
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            Debug.Log("Microphones detected: " + Microphone.devices[0]);
            // Start a temporary recording to prompt mic access
            AudioClip tempClip = Microphone.Start(Microphone.devices[0], false, 1, 44100);
            Debug.Log("Recording started, please speak into the mic...");
            // Stop recording after a short delay
            Invoke("StopRecording", 1f);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void StopRecording()
    {
        Microphone.End(Microphone.devices[0]);
        Debug.Log("Recording ended.");
    }
}
