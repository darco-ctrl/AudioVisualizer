using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class FrequencySpectrum : Control
{
	private GlobalManager GM;

	[Export] private CanvasData canvasData;

	

	private float[] spectrum;

	private float XOffset = 0;
	private float CenterX = 0;

	private float YOffset = 0;

	private float GraphSizeXHalf = 0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GM = GetNode<GlobalManager>("/root/GlobalManager");

		XOffset = (canvasData.CanvasSize.X - canvasData.GraphSize.X) / 2;

		CenterX = XOffset + canvasData.GraphSize.X / 2;
		
		float y_offset = (canvasData.CanvasSize.Y - canvasData.GraphSize.Y) / 2;
		YOffset = canvasData.CanvasSize.Y - y_offset;

		GraphSizeXHalf = canvasData.GraphSize.X / 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateSpectrum();
		QueueRedraw();
	}

    public override void _Draw()
    {
		if (spectrum == null) return;

		DrawRect(
			new Rect2(0, 0, canvasData.CanvasSize.X, canvasData.CanvasSize.Y),
			new Color(0.7f, 0.7f, 0.7f, 1),
			true
		);

		for (int i=0; i < spectrum.Length - 1; i++)
		{
			float t1 = (float)i / spectrum.Length;
			float t2 = (float)(i + 1) / spectrum.Length;

			float rx1 = CenterX + t1 * GraphSizeXHalf;
			float rx2 = CenterX + t2 * GraphSizeXHalf;

			float lx1 = CenterX - t1 * GraphSizeXHalf;
			float lx2 = CenterX - t2 * GraphSizeXHalf;

			float y1 = YOffset - spectrum[i] * canvasData.GraphSize.Y;
			float y2 = YOffset - spectrum[i + 1] * canvasData.GraphSize.Y;

			DrawLine(new Vector2(rx1, y1), new Vector2(rx2, y2), Colors.Red, 2f);
			DrawLine(new Vector2(lx1, y1), new Vector2(lx2, y2), Colors.Red, 2f);
		}
    }

	public void UpdateSpectrum()
	{
		spectrum = GM.Audio.GetSpectrum();
	}

}
