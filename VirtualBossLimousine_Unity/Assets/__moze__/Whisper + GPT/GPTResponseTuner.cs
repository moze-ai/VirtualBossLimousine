using System;
using System.ComponentModel;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class GPTResponseTuner
{
	//modelはテストの時は gpt-3.5-turbo-16k, 本番はgpt-4
	public static async UniTask<string> GetGPTResponse(ChatMessage[] message, GPTSettings gptSettings, CancellationToken token, int maxTryCount = 10)
	{
		try
		{
			string url = gptSettings.model switch
			{
				GPTSettings.Model.gpt3_5_turbo_16k_azure => "https://limousine-girl-whisper-dev.openai.azure.com/openai/deployments/gpt-35-turbo-16k-dev/chat/completions?api-version=2023-07-01-preview",
				GPTSettings.Model.gpt4_azure => "https://sukideca-dev-gpt4.openai.azure.com/openai/deployments/gpt-4-dev/chat/completions?api-version=2023-07-01-preview",
				_ => throw new ArgumentOutOfRangeException()
			};

			ChatBody chatBody = new ChatBody
			{
				model = gptSettings.model.GetDescription(), messages = message, max_tokens = gptSettings.max_tokens,
				temperature = gptSettings.temperature, top_p = gptSettings.top_p, frequency_penalty = gptSettings.frequency_penalty,
				presence_penalty = gptSettings.presences_penalty
			};
			string myJson = JsonUtility.ToJson(chatBody);
			byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(myJson);

			var privateApiKey = gptSettings.model switch
            {
                GPTSettings.Model.gpt3_5_turbo_16k_azure => StaticAPIUtil.AzureOpenAIKey,
                GPTSettings.Model.gpt4_azure => StaticAPIUtil.AzureOpenAIKey_gpt4,
                _ => throw new ArgumentOutOfRangeException()
            };

			using UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
			www.uploadHandler = new UploadHandlerRaw(jsonToSend);
			www.downloadHandler = new DownloadHandlerBuffer();
			www.SetRequestHeader("api-key", privateApiKey); //Azure Open AI向け
			www.SetRequestHeader("Content-Type", "application/json");

			await www.SendWebRequest().ToUniTask(cancellationToken: token);
			var response = www.downloadHandler.text;
			EditorOnlyDebug.Log(response);
			//JSON形式のresponseをBodyクラスに変換
			var responseJson = JsonUtility.FromJson<ChatResponse>(response);
			//Bodyクラスの中のmessagesの中のcontentを取得
			var output = responseJson.choices[0].message.content;
			return output;
		}
		catch (Exception e)
		{
			//エラーが出たら、maxTryCount回失敗するまでもう一度実行
			Debug.LogError($"【あと{maxTryCount}回リトライ】ChatGPTのエラー:{e}");

			var newCount = maxTryCount - 1;
			if (newCount < 1)
			{
				throw new Exception("ChatGPTのエラーが10回続いたため、処理を中断しました.");
			}

			return await GetGPTResponse(message, gptSettings, token, newCount);
		}
	}
}

[Serializable]
public class GPTSettings
{
	public enum Model
	{
		[Description("gpt-3.5-turbo-16k")] gpt3_5_turbo_16k_azure = 2,
		[Description("gpt-4")] gpt4_azure = 3,
	}

	public Model model = Model.gpt3_5_turbo_16k_azure;

	[Tooltip("The maximum number of tokens to generate. Requests can use up to 4000 tokens shared between prompt and completion. (One token is roughly 4 characters for normal English text)")]
	public int max_tokens = 2048;

	[Range(0.0f, 1.0f), Tooltip("Controls randomness: Lowering results in less random completions. As the temperature approaches zero, the model will become deterministic and repetitive.")]
	public float temperature = 0.2f;

	[Range(0.0f, 1.0f), Tooltip("Controls diversity via nucleus sampling: 0.5 means half of all likelihood-weighted options are considered.")]
	public float top_p = 0.8f;

	[Tooltip("Where the API will stop generating further tokens. The returned text will not contain the stop sequence.")]
	public string stop;

	[Range(0.0f, 2.0f), Tooltip("How much to penalize new tokens based on their existing frequency in the text so far. Decreases the model's likelihood to repeat the same line verbatim.")]
	public float frequency_penalty = 0;

	[Range(0.0f, 2.0f), Tooltip("How much to penalize new tokens based on whether they appear in the text so far. Increases the model's likelihood to talk about new topics.")]
	public float presences_penalty = 0;
}
