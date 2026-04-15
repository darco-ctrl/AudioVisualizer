using Godot;
using System;

public partial class AudioManager : Node
{
	
	public SampleData _SampleData;
	
	public override void _Ready()
	{
		_SampleData = GetNode<SampleData>("/root/SampleData");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public float[] GetSamples()
	{
		return _SampleData.GetSamples();
	}

	public float[] GetSpectrum()
	{
		return _SampleData.GetSpectrum();
	}

	public int GET_FFT_SIZE()
	{
		return _SampleData.FFT_SIZE;
	}

	public float BinToHz (int bin)
	{
		return (float)bin * _SampleData.Capture.WaveFormat.SampleRate / GET_FFT_SIZE();
	}

	public float[] GetLeftSpectrum()
	{
		return _SampleData.LeftSpectrum;
	}
}
