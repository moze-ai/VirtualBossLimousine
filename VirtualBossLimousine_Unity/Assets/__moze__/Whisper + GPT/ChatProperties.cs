using System;

[Serializable]
public class ChatMessage
{
    public string role; //user, assistant
    public string content;
}

[Serializable]
public class ChatBody
{
    public string model;
    public ChatMessage[] messages;
    public int max_tokens;
    public float temperature;
    public float top_p;
    public float frequency_penalty;
    public float presence_penalty;
}

[Serializable]
public class ChatResponse
{
    public string id;
    public string obj;
    public int created;
    public string model;
    public ChatUsage usage;
    public ChatChoice[] choices;
}

[Serializable]
public class ChatUsage
{
    public int prompt_tokens;
    public int completion_tokens;
    public int total_tokens;
}

[Serializable]
public class ChatChoice
{
    public ChatMessage message;
    public int index;
    public string finish_reason;
}
