using Cysharp.Threading.Tasks;
using UnityEngine;

public class ApiTester : MonoBehaviour
{
    [SerializeField, TextArea(3, 10)] string _voicevoxTestText;
    [SerializeField] bool _useLocalHost = false;
    [SerializeField] int _styleId = 0;
    bool _processing;

    AudioSource _audio;

    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    async void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && !_processing)
        {
            EditorOnlyDebug.Log("<color=cyan>Start requesting...</color>");
            _processing = true;
            var token = this.GetCancellationTokenOnDestroy();
            var body = await VoicevoxApi.GetAudioQuery(_voicevoxTestText, _styleId, token, _useLocalHost);
            var clip = await VoicevoxApi.GetSynthesis(body, _styleId, token, _useLocalHost);
            _audio.clip = clip;
            _audio.Play();
            _processing = false;
            EditorOnlyDebug.Log("<color=cyan>Finish requesting</color>");
        }
    }
}
