using Godot;
using System;

public partial class Game : Node
{
	public float GlobalSuspicion;
	
	public float MaxGlobalSuspicion;
	
	public float[] LocalSuspicions; //Local Suspcisions of every room
	
	public float[] MaxLocalSuspicions; //Maximum Local Supsicions
	
	public float[] LocalSuspicionThresholds; //Local Suspicion thresholds of each room
	
	public float MasterVolume; //Master volume of the whole game
	
	public float SoundVolume; //Volume of all sound effects
	
	public float MusicVolume; //Volume of all music
	
	//Whether each step in a puzzle in complete
	public bool[] SecurityPuzzle;
	public bool[] MedicPuzzle;
	public bool[] StoragePuzzle;
	public bool[] ControlPuzzle;
	
	public bool[] RoomsHostile; //Whether each room is hostile
	
	//Rooms of player and patrolling NPC
	public int PlayerRoom;
	public int PatrolRoom;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Decay all local suspicions and global suspicion
	public void DecaySuspicion()
	{
		
	}
	
	//Increase the local suspicion of a room
	public void IncreaseLocalSuspicion(int Room)
	{
		
	}
	
	//Handle the player losing
	public void PlayerLost()
	{
		
	}
	
	//Handle the player winning
	public void PlayerWon()
	{
		
	}
}
