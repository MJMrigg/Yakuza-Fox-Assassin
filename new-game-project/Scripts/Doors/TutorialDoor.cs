using Godot;
using System;

public partial class TutorialDoor : Door
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void ChangeRoom(bool AllowedThrough)
	{
		base.ChangeRoom(AllowedThrough);
	}
	
	public override void CheckPlayer(Node2D body)
	{
	}
}
