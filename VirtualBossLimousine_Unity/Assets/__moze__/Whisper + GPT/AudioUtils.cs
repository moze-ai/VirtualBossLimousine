using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class AudioUtils
{
    public static AudioClip ChangeAudioLength(AudioClip source, float seconds)
    {
        var samples = (int) (source.samples * (seconds / source.length));
        var result = AudioClip.Create(
            source.name,
            samples,
            source.channels,
            source.frequency,
            false);
        var data = new float[samples * source.channels];
        source.GetData(data, 0);
        result.SetData(data, 0);
        return result;
    }

    public static async UniTask<AudioClip> LoadAudioClip(string url, bool isStream, CancellationToken token)
    {
        try
        {
            using var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            ((DownloadHandlerAudioClip) request.downloadHandler).streamAudio = isStream;
            await request.SendWebRequest().WithCancellation(token);

            Debug.Log(request.downloadHandler.data.Length);
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError(request.error);
                return null;
            }

            return DownloadHandlerAudioClip.GetContent(request);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
