using Godot;
using System;

public partial class ValueBox : ColorRect
{

	[Export] public float Value;
	
	[Export] private Label ValueLable;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateValue(float _new_value)
	{
		Value = _new_value;
		ValueLable.Text = _new_value.ToString();
	}
}
