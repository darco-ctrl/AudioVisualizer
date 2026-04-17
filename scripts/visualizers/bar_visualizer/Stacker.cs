using Godot;
using System;

public partial class Stacker : Control
{

	public int Spacing = 10;
	public int NumberOfBars = 10;

	private ColorRect[] Bars;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Bars = new ColorRect[NumberOfBars];
		LoadBars();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadBars()
	{
		float totalSpacing = (NumberOfBars - 1) * Spacing;
		float barWidth = (Size.X - totalSpacing) / NumberOfBars;

		float barPositionX = 0;

		for (int i = 0; i < NumberOfBars; i++)
		{
			ColorRect bar = new ColorRect();

			bar.Name = $"Bar_{i}";
			bar.Color = Colors.White;
			bar.Size = new Vector2(barWidth, 60);

			AddChild(bar);
			bar.Position = new Vector2(barPositionX, Size.Y - bar.Size.Y);

			barPositionX += barWidth + Spacing;

			if (i == NumberOfBars - 1)
			{
				barPositionX -= Spacing;
			}

			GD.Print($"Bar_{i}: Size ({bar.Size.X}, {bar.Size.Y}, Position ({bar.Position.X}, {bar.Position.Y})");
		}
	}
}
