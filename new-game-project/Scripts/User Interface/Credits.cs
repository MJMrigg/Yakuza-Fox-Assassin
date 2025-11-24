using Godot;
using System;

public partial class Credits : ColorRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Change song back to the original song
		((MusicPlayer)GetTree().GetRoot().GetChild(1)).ChangeSong(0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Move the credits up
		Position = new Vector2(Position.X,Position.Y-0.5f);
	}
}
