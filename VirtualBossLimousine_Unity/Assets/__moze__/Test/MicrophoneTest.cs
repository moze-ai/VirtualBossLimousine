using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneTest : MonoBehaviour
{
	AudioSource audioSource;

	void Start()
	{
		// Get the AudioSource component
		audioSource = GetComponent<AudioSource>();

		// Check if a microphone is connected and print the name
		if (Microphone.devices.Length > 0)
		{
			Debug.Log("Microphone found: " + Microphone.devices[0]);

			// Start recording with the first available microphone
			audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);

			// Loop the playback
			audioSource.loop = true;

			// Don't play the audio back until the microphone is actually sending data
			while (!(Microphone.GetPosition(Microphone.devices[0]) > 0)) {}

			// Start playing the audio source
			audioSource.Play();
		}
		else
		{
			Debug.LogError("No microphone found to test input.");
		}
	}
}
