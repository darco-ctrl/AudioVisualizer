using Godot;
using System;

[GlobalClass]
public partial class FrequencySpectrum : Control
{
	private GlobalManager GM;

	[Export] private CanvasData canvasData;


	private float[] spectrum;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GM = GetNode<GlobalManager>("/root/GlobalManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _Draw()
    {
		if (spectrum == null) return;

		DrawRect(
			new Rect2(0, 0, canvasData.CanvasSize.X, canvasData.CanvasSize.Y),
			new Color(0.7f, 0.7f, 0.7f, 1)
		);
    }

	public void UpdateSpectrum()
	{
		spectrum = GM.Audio.GetSpectrum();
	}

}
