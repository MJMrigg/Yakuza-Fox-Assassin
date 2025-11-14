using Godot;
using System;

public partial class Door : CharacterBody2D
{
	public bool Unlocked; //If the door is unlocked or not
	
	[Export]
	public bool Color; //What color the door is
	
	[Export]
	public int ConnectedRoom; //The room that the door leads to
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Unlocked = false; //Start the door with being unlocked
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change if the door is locked or unlocked
	public void ChangeLock(bool ConditionsMet)
	{
		
	}
	
	//Check if the door is unlocked
	public bool CheckLock()
	{
		return Unlocked;
	}
	
	//Change to the next room
	public void ChangeRoom(bool Unlocked)
	{
		
	}
}
