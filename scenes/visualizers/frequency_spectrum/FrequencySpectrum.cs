using Godot;
using System;
using System.Linq;

[GlobalClass]
public partial class FrequencySpectrum : Control
{
	private GlobalManager GM;

	[Export] private CanvasData canvasData;

	private float[] leftSpectrum;
	private float[] rightSpectrum;

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
		if ((leftSpectrum == null) || (rightSpectrum == null)) return;

		DrawRect(
			new Rect2(0, 0, canvasData.CanvasSize.X, canvasData.CanvasSize.Y),
			new Color(0.7f, 0.7f, 0.7f, 1),
			true
		);

		for (int i=0; i < GM.Audio.GetSingleChannelLength() - 1; i++) // Single Channel LEnght = 1024 / 4
		{

			// normalized position x value for right
			float Normailized_x1 = (float)i / GM.Audio.GetSingleChannelLength();
			float Normailized_x2 = (float)(i + 1) / GM.Audio.GetSingleChannelLength();


			// left x points
			float lx1 = CenterX - Normailized_x1 * GraphSizeXHalf;
			float lx2 = CenterX - Normailized_x2 * GraphSizeXHalf;

			// right x points
			float rx1 = CenterX + Normailized_x1 * GraphSizeXHalf; 
			float rx2 = CenterX + Normailized_x2 * GraphSizeXHalf; 

			// left y point
			float ly1 = YOffset - leftSpectrum[i] * canvasData.GraphSize.Y;
			float ly2 = YOffset - leftSpectrum[i + 1] * canvasData.GraphSize.Y;

			// right y point
			float ry1 = YOffset - rightSpectrum[i] * canvasData.GraphSize.Y;
			float ry2 = YOffset - rightSpectrum[i + 1] * canvasData.GraphSize.Y;


			DrawLine(new Vector2(lx1, ly1), new Vector2(lx2, ly2), Colors.Red, 2f);
			DrawLine(new Vector2(rx1, ry1), new Vector2(rx2, ry2), Colors.Red, 2f);

			if (Input.IsActionJustPressed("Get Debug"))
			{
				GD.Print($" lx1: {lx1}\n rx1: {rx1}\n ly1: {ly1}\n ry1: {ly2}\n  lx2: {lx1}\n rx2: {rx1}\n ly2: {ly1}\n ry2: {ly2}");
			}

		}
    }

	public void UpdateSpectrum()
	{
		//spectrum = GM.Audio.GetSpectrum();

		leftSpectrum = GM.Audio.GetLeftSpectrum();
		rightSpectrum = GM.Audio.GetRightSpectrum();
		
	}

}
