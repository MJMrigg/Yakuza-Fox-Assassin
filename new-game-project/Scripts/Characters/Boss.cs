using Godot;
using System;

public partial class Boss : Enemy
{
	public int ExplosionDamage; //Damage that each explosion does
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure to set the damage of the boss
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Attack for the boss
	public override void HandleHostile()
	{
		
	}
	
	//Create explosions all around the map with one being on the player
	public void SpawnExplosions()
	{
		
	}
	
	//Boss has special death logic
	public override void Remove()
	{
		//Death logic goes here
		
		base.Remove();
	}
}
