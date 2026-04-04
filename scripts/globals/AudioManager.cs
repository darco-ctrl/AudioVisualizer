using Godot;
using System;

public partial class AudioManager : Node
{
	
	private SampleData _SampleData;
	
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
}
