using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Serialization;
using Input = UnityEngine.Input;

public class GirlController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] Animator _animator;
    [SerializeField] LookAtIK _lookAtIK;
    [Header("GPT")]
    [SerializeField] GPTSettings.Model _model = GPTSettings.Model.gpt3_5_turbo_16k_azure;
    [SerializeField, TextArea(3, 10)] string _customInstruction;
    [Header("Voicevox")]
    [SerializeField] bool _useLocalHost = false;
    [FormerlySerializedAs("_styleId"),SerializeField] int _voicevoxStyleId = 0;
    [SerializeField] List<ChatMessage> _messages;

    readonly RecordingManager _recordingManager = new RecordingManager();
    const int _targetMp3BitRate = 32;
    const int _limitAudioRecordSeconds = 10;
    bool _processing;

    SaveChatLog _saveChatLog;

    [SerializeField] AudioSource _audio;

    void Start()
    {
        _saveChatLog = new SaveChatLog();
        //CustomInstructionをuserのmessageとして記録
        _saveChatLog.AddMessageToMemory("user", _customInstruction);
    }

    void Update()
    {
        //Rキーを押して録音開始/停止
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!_recordingManager.IsRecording() && !_processing)
            {
                _processing = true;
                EditorOnlyDebug.Log("<color=cyan>Start recording</color>");
                _recordingManager.StartRecording(_limitAudioRecordSeconds);
            }
            else if(_recordingManager.IsRecording() && _processing)
            {
                EditorOnlyDebug.Log("<color=cyan>Stop recording and requesting...</color>");
                _recordingManager.StopRecording();
                var userInputAudioClip = _recordingManager.CreateOptimalTrimmedAudio();
                //Assets/StreamingAssets/TalkForAFewSeconds16.wav を読み込んでbyte[]に変換
                //string filePath = Path.Combine(Application.streamingAssetsPath, "TalkForAFewSeconds16.wav");
                //byte[] bytes = File.ReadAllBytes(filePath);
                byte[] bytes = WavToMp3.ConvertWavToMp3(userInputAudioClip, _targetMp3BitRate);
                //string byteString = BitConverter.ToString(bytes);
                // 出力にはダッシュ（-）が含まれているので、それを取り除く（オプション）
                //byteString = byteString.Replace("-", " ");
                //Debug.Log(byteString);
                GetGirlResponse(bytes);
            }
        }

        //マイク一覧を取得
        // if (Input.GetKeyDown(KeyCode.M))
        // {
        //     var micDevices = Microphone.devices;
        //     var deviceCount = 0;
        //
        //     foreach (var device in micDevices)
        //     {
        //         Debug.Log(deviceCount + ":" + device);
        //         deviceCount++;
        //     }
        // }
    }

    //Whisper → GPT → Voicevox
    //引数bytesはユーザーの発話音声データ
    async void GetGirlResponse(byte[] bytes)
    {
        var token = this.GetCancellationTokenOnDestroy();
        //Whisper
        var whisperResponse = await WhisperApi.GetWhisperResponse(bytes, token);
        if(whisperResponse == null)
        {
            EditorOnlyDebug.Log("<color=red>Whisper response is null</color>");
            _processing = false;
            return;
        }
        //GPT
        var message = new ChatMessage()
        {
            role = "user",
            content = whisperResponse
        };
        List<ChatMessage> input = _saveChatLog.AddMessageToMemory(message);
        _saveChatLog.GetChatLogText(); //ChatLogを確認デバッグ
        var gptSettings = new GPTSettings()
        {
            model = _model
        };
        var output = await GPTResponseTuner.GetGPTResponse(input.ToArray(), gptSettings, token);
        _saveChatLog.AddMessageToMemory("assistant", output);
        _messages = _saveChatLog._messages;
        Debug.Log($"<color=yellow>assistant: {output}</color>");
        //Voicevox
        var body = await VoicevoxApi.GetAudioQuery(output, _voicevoxStyleId, token, _useLocalHost);
        var clip = await VoicevoxApi.GetSynthesis(body, _voicevoxStyleId, token, _useLocalHost);
        _audio.clip = clip;
        _audio.Play();
        //アニメーションを再生
        _animator.SetBool("talking", true);
        DOVirtual.Float(0f, 1f, 0.5f, value =>
        {
            _lookAtIK.GetIKSolver().IKPositionWeight = value;
        });
        await UniTask.WaitUntil(() => !_audio.isPlaying, cancellationToken: token);
        _animator.SetBool("talking", false);
        DOVirtual.Float(1f, 0f, 0.5f, value =>
        {
            _lookAtIK.GetIKSolver().IKPositionWeight = value;
        });
        _processing = false;
        EditorOnlyDebug.Log("<color=cyan>Finish requesting</color>");
    }
}
