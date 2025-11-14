using Godot;
using System;

public partial class Projectile : CharacterBody2D
{
	[Export]
	public int damage; //Damage the projectile does upon hitting
	
	[Export]
	public float speed; //Movement speed
	
	public Vector2 direction; //Direction the projectile moves(determined by who shot it)
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Velocity = direction * speed; //Set velocity
		MoveAndSlide(); //Move
	}
}
