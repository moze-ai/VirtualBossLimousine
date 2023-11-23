using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class VoicevoxApi
{
    //音声合成のクエリを作成
    //https://voicevox.github.io/voicevox_engine/api/#tag/%E3%82%AF%E3%82%A8%E3%83%AA%E4%BD%9C%E6%88%90/operation/audio_query_audio_query_post
    public static async UniTask<string> GetAudioQuery(string text, int styleId, CancellationToken token, bool useLocalHost = false)
    {
        try
        {
            var url = useLocalHost ? "http://localhost:50021/audio_query"
            : "https://limousine-girl-voicevox.azurewebsites.net/audio_query";
            //リクエストクエリとして整形
            var query = useLocalHost ? $"?text={text}&speaker={styleId}"
            : $"?text={text}&style_id={styleId}";
            url += query;
            using UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            await www.SendWebRequest().ToUniTask(cancellationToken: token);
            var response = www.downloadHandler.text;
            EditorOnlyDebug.Log(response);
            return response;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    //音声合成を取得
    //https://voicevox.github.io/voicevox_engine/api/#tag/%E9%9F%B3%E5%A3%B0%E5%90%88%E6%88%90/operation/synthesis_synthesis_post
    public static async UniTask<AudioClip> GetSynthesis(string requestJson, int styleId, CancellationToken token, bool useLocalHost = false)
    {
        try
        {
            var url = useLocalHost ? "http://localhost:50021/synthesis"
            : "https://limousine-girl-voicevox.azurewebsites.net/synthesis";
            //リクエストクエリとして整形
            var query = useLocalHost ? $"?speaker={styleId}"
            : $"?style_id={styleId}";
            url += query;
            using UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(requestJson);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            await www.SendWebRequest().ToUniTask(cancellationToken: token);
            AudioClip clip = WavUtility.ToAudioClip(www.downloadHandler.data);
            return clip;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
