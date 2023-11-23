using UnityEngine;

/// <summary>
/// Editorの時のみログを出す
/// </summary>
public static class EditorOnlyDebug
{
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }
}
