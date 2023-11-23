using System;
using UnityEngine;

public class RecordingManager
{
    public AudioClip _audioClip;
    DateTime _recordingStart;
    DateTime _recordingEnd;
    readonly string _deviceName;

    public RecordingManager(string deviceName = null)
    {
        _deviceName = deviceName;
    }

    public bool IsRecording()
    {
        return Microphone.IsRecording(_deviceName);
    }

    public AudioClip CreateOptimalTrimmedAudio()
    {
        var diffSeconds = (float) ((_recordingEnd - _recordingStart).TotalMilliseconds / 1000.0);
        if (diffSeconds < 0 || diffSeconds > _audioClip.length)
        {
            return _audioClip;
        }

        return AudioUtils.ChangeAudioLength(_audioClip, diffSeconds);
    }

    public void StartRecording(int lengthSec)
    {
        _recordingStart = DateTime.Now;
        _audioClip = Microphone.Start(_deviceName, false, lengthSec, 44100);
    }

    public void StopRecording()
    {
        _recordingEnd = DateTime.Now;
        Microphone.End(_deviceName);
    }
}
