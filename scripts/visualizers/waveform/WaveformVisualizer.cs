using Godot;
using System;
using NAudio.Wave;

[GlobalClass]
public partial class WaveformVisualizer : Control
{
	
	private GlobalManager GM;

	private float[] Waveform;

	private Vector2 CanvasSize = new Vector2(1152, 648);
	private Vector2 GraphSize = new Vector2(1100, 648);

	private float XOffset;
	private float GraphSizeYHalf;

	private int WaveformMax = 0;

	private bool test = false;

	public override void _Ready()
	{
		GM = GetNode<GlobalManager>("/root/GlobalManager");

		GraphSizeYHalf = GraphSize.Y / 2;

		XOffset = (CanvasSize.X - GraphSize.X) / 2; 

	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateWaveform();
		QueueRedraw();
	}

    public override void _Draw()
    {      


		if (Waveform == null) return;

		DrawRect(
			new Rect2(0, 0, CanvasSize.X , CanvasSize.Y), 
			new Color(0.7f, 0.7f, 0.7f, 1), // black background
			true // filled
		);

		for (int i = 0; i < Waveform.Length - 1; i++)
		{
			// points (x, y) 


			float x1 = (float)i / WaveformMax * GraphSize.X + XOffset;
			float y1 = Waveform[i] * -GraphSizeYHalf + GraphSizeYHalf;

			float x2 = (float)(i + 1) / WaveformMax * GraphSize.X + XOffset;
			float y2 = Waveform[i + 1] * - GraphSizeYHalf + GraphSizeYHalf;

			

			DrawLine (
				new Vector2(x1, y1),
				new Vector2(x2, y2),
				Colors.Blue,
				2f
			);

			//GD.Print($"Current Sample: {Waveform[i]}");
		//	GD.Print($"x1: {x1}, y1: {y1}, x2: {x2}, y2: {y2}");
			
		}
    }

	private void UpdateWaveform()
	{
		
		Waveform = GM.Audio.GetSamples();
		WaveformMax = Waveform.Length - 1;


		if (!test)
		{
			test = true;
			GD.Print($"Sample Size: {WaveformMax}");
		}

	}

}
