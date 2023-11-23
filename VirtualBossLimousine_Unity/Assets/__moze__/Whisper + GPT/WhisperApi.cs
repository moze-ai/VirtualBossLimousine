using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class WhisperApi
{
    public static async UniTask<string> GetWhisperResponse(byte[] fileBytes, CancellationToken token)
    {
        try
        {
            var privateApiKey = StaticAPIUtil.AzureOpenAIKey;
            var url = "https://limousine-girl-whisper-dev.openai.azure.com/openai/deployments/whisper-dev/audio/transcriptions?api-version=2023-09-01-preview";

            WWWForm form = new WWWForm();
            form.AddBinaryData("file", fileBytes, "rec.mp3", "audio/mp3");
            using UnityWebRequest www = UnityWebRequest.Post(url, form);
            www.SetRequestHeader("api-key", privateApiKey); //Azure Open AI向け
            await www.SendWebRequest().ToUniTask(cancellationToken: token);
            var response = www.downloadHandler.text;
            var responseJson = JsonUtility.FromJson <WhisperResponse>(response);
            var output = responseJson.text;
            EditorOnlyDebug.Log($"<color=cyan>Whisper response: {output}</color>");
            return output;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    [Serializable]
    public class WhisperResponse
    {
        public string text;
    }
}
