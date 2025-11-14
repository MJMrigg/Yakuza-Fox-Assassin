using Godot;
using System;

public partial class Key : Item
{
	[Export]
	public bool color; //What color the key is
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Equipable = false; //Keys can not be equiped
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
