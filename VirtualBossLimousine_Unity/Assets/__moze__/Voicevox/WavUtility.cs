using System;
using UnityEngine;

/// <summary>
/// https://note.com/negipoyoc/n/n081e25f5ee9e
/// </summary>
public static class WavUtility
{
	public static AudioClip ToAudioClip(byte[] data)
	{
		// ヘッダー解析
		int channels = data[22];
		int frequency = BitConverter.ToInt32(data, 24);
		int length = data.Length - 44;
		float[] samples = new float[length / 2];

		// 波形データ解析
		for (int i = 0; i < length / 2; i++)
		{
			short value = BitConverter.ToInt16(data, i * 2 + 44);
			samples[i] = value / 32768f;
		}

		// AudioClipを作成
		AudioClip audioClip = AudioClip.Create("AudioClip", samples.Length, channels, frequency, false);
		audioClip.SetData(samples, 0);

		return audioClip;
	}
}
