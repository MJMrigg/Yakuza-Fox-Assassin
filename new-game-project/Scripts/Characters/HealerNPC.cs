using Godot;
using System;

public partial class HealerNPC : NPC
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Speed = 0.0f; //Healer NPCs do not move
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Heal the player
	public void Heal()
	{
		
	}
}
