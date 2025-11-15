using Godot;
using System;

public partial class Entity : CharacterBody2D
{
	[Export]
	public CollisionShape2D MyPhysicsCollider; //Collider(rotates with movement)
	
	[Export]
	public AnimatedSprite2D MySpriteAnimation; //Animator
	
	[Export]
	public int RoomId; //Room the entity is in (helpful for save/load)
	
	public bool Stop = false; //Whether the entity should stop moving because the player has paused something
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.AddToGroup("Pausable");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Pause/Unpause the Entity
	public void Pause()
	{
		Stop = !Stop;
	}
}
