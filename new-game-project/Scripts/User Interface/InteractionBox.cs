using Godot;
using System;

public partial class InteractionBox : Label
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Print the current input map interact key onto the sprite
		var actions = InputMap.ActionGetEvents("p_interact");
		Text = actions[0].AsText().Split(" (")[0];
	}
}
