using Godot;
using System;

public partial class PatrollingNPC : Enemy
{
	public bool Unconcious; //Whether Paul is unconcious
	
	public bool TimeToMove; //Whether it's time to move to another room
	
	public int[] PawPatrol; //Path around the map
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure to set the damage of the boss
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Attack for Paul
	public override void HandleHostile()
	{
		
	}
	
	//Paul falls unconsious instead of being removed from the scene
	public override void Remove()
	{
		
	}
	
	//Paul has special move logic
	public override void Move()
	{
		
	}
	
	//Timer for events
	public async void timer()
	{
		
	}
	
	//Increase the suspicion threshold of a room
	public void IncreaseSuspicionThreshold()
	{
		
	}
}
