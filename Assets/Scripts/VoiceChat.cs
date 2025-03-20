using Photon.Voice.Unity;
using Photon.Realtime;
using UnityEngine;

public class VoiceChat : MonoBehaviour
{
    private Recorder recorder;

    void Start()
    {
        recorder = GetComponent<Recorder>();
        recorder.TransmitEnabled = true;
    }
}
