using Godot;
using System;

public partial class Options : Control
{

	[Export] public Button OptionsButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		OptionsButton.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnMouseEntered()
	{
		OptionsButton.Visible = true;
		GD.Print("Mouse Entered");	
	}

	public void OnMouseExited()
	{
		OptionsButton.Visible = false;
		GD.Print("Mouse Exited");
	}
}
