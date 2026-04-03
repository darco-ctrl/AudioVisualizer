using Godot;
using NAudio.Wave;
using System;

public partial class BoxVisualizer : Control
{
	[Export] public int MaximumValueBoxes = 10;

	[Export] public PackedScene ValueBoxeScene;

	[Export] public HBoxContainer Container;

	public ValueBox[] ValueBoxes;

	public override void _Ready()
	{
		GD.Print("Making value boxes.");
		MakeValueBoxes();
	}

	
	public override void _Process(double delta)
	{
		
	}

	private void Capture_DataAvailable(Object _sender, WaveInEventArgs _eventArgs)
	{
		int sampleCount = _eventArgs.BytesRecorded / 4;

		for (int i = 0; i < sampleCount; i++)
		{
			float sample = BitConverter.ToSingle(_eventArgs.Buffer, i * 4);
		}
	}

	private void MakeValueBoxes()
	{
		ValueBoxes = new ValueBox[MaximumValueBoxes];

		for (int i=0; i < MaximumValueBoxes; i++)
		{
            ValueBox _value_box = new ValueBox
            {
                CustomMinimumSize = new Vector2(100, 100),
				Size = new Vector2(100, 100),
				Visible = true
		
            };

            Container.AddChild(_value_box);

			ValueBoxes[i] = _value_box;
		}
	}
}
