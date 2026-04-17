using Godot;
using System;
using System.Linq;
using System.Text;

public partial class GlobalManager : Node
{

	public AudioManager Audio;
	
	public override void _Ready()
	{
		Audio = GetNode<AudioManager>("/root/AudioManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


}
