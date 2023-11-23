using System.IO;
using UnityEngine;

/// <summary>
/// OpenAIのAPIKeyを保持する
/// </summary>
public static class StaticAPIUtil
{
    public static string AzureOpenAIKey { get; private set; }
    public static string AzureOpenAIKey_gpt4 { get; private set; }

    //初期化
    static StaticAPIUtil()
    {
        var filePath1 = Path.Combine(Application.dataPath, "GameData", "azure_openai_key.txt");
        AzureOpenAIKey = File.ReadAllText(filePath1);
        var filePath2 = Path.Combine(Application.dataPath, "GameData", "azure_openai_key_gpt4.txt");
        AzureOpenAIKey_gpt4 = File.ReadAllText(filePath2);
        //Debug.Log($"{AzureOpenAIKey}, {AzureOpenAIKey_gpt4}");
    }
}
