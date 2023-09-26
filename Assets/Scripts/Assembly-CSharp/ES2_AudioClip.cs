using System.ComponentModel;
using UnityEngine;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ES2_AudioClip : ES2Type
{
	public ES2_AudioClip()
		: base(typeof(AudioClip))
	{
		key = 25;
	}

	public override void Write(object data, ES2Writer writer)
	{
		AudioClip audioClip = (AudioClip)data;
		writer.Write((byte)5);
		float[] array = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData(array, 0);
		writer.Write(audioClip.name);
		writer.Write(audioClip.samples);
		writer.Write(audioClip.channels);
		writer.Write(audioClip.frequency);
		writer.Write(array);
	}

	public override object Read(ES2Reader reader)
	{
		AudioClip audioClip = null;
		string name = string.Empty;
		int lengthSamples = 0;
		int channels = 0;
		int frequency = 0;
		int num = reader.Read_byte();
		for (int i = 0; i < num; i++)
		{
			switch (i)
			{
			case 0:
				name = reader.Read_string();
				break;
			case 1:
				lengthSamples = reader.Read_int();
				break;
			case 2:
				channels = reader.Read_int();
				break;
			case 3:
				frequency = reader.Read_int();
				break;
			case 4:
				audioClip = AudioClip.Create(name, lengthSamples, channels, frequency, false, false);
				audioClip.SetData(reader.ReadArray<float>(new ES2_float()), 0);
				break;
			default:
				return audioClip;
			}
		}
		return audioClip;
	}
}
