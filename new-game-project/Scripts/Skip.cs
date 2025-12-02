using Godot;
using System;

public partial class Skip : Panel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Become visible after 10 seconds
		Visible = false;
		Reveal();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Reveal the skip button after some amount of time
	public async void Reveal()
	{
		ToSignal(GetTree().CreateTimer(10),"timeout");
		Visible = true;
	}
}
