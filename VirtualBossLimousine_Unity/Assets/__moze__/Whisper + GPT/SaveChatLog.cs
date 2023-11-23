using System.Collections.Generic;

/// <summary>
/// LLMのChatLogを保存するクラス
/// クラスのインスタンスを作成して利用する
/// </summary>
public class SaveChatLog
{
    public List<ChatMessage> _messages { get; private set; } = new List<ChatMessage>();
    string _currentChatLogFileName;

    //roleとcontentを_messagesに追加したうえで_messagesを返す
    public List<ChatMessage> AddMessageToMemory(string role, string content)
    {
        _messages.Add(new ChatMessage()
        {
            role = role,
            content = content
        });
        return _messages;
    }

    //ChatMessageを_messagesに追加したうえで_messagesを返す
    public List<ChatMessage> AddMessageToMemory(ChatMessage chatMessage)
    {
        _messages.Add(chatMessage);
        return _messages;
    }

    //_messagesの中身をuserとassistantのroleを区別し、それぞれのconentを改行して表示する
    public string GetChatLogText()
    {
        string chatLogText = "";
        foreach (var message in _messages)
        {
            if (message.role == "user")
            {
                chatLogText += $"user:{message.content}\n";
            }
            else if(message.role == "assistant")
            {
                chatLogText += $"assistant:{message.content}\n";
            }
        }
        EditorOnlyDebug.Log(chatLogText);
        return chatLogText;
    }
    
    //ChatLogを削除してリセットする
    public void ResetChatLog()
    {
        _messages.Clear();
    }
}
