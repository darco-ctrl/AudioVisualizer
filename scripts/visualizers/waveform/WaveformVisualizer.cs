using Godot;
using System;
using NAudio.Wave;

[GlobalClass]
public partial class WaveformVisualizer : Control
{
	
	private GlobalManager GM;

	private float[] Waveform;

	public override void _Ready()
	{
		GM = GetNode<GlobalManager>("/root/GlobalManager");
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Waveform = GM.Audio.GetSamples();
	}
}
