using Photon.Voice.Unity;
using UnityEngine;

public class VoiceChat : MonoBehaviour
{
    private Recorder recorder;

    void Start()
    {
        recorder = GetComponent<Recorder>();
        if (recorder != null)
        {
            recorder.TransmitEnabled = true;
            // Enable Debug Echo Mode to hear your own voice for testing:
            recorder.DebugEchoMode = true;
            Debug.Log("VoiceChat: Recorder TransmitEnabled and DebugEchoMode set.");
        }
        else
        {
            Debug.LogError("VoiceChat: Recorder component missing on this GameObject.");
        }
    }
}
