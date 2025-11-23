using Godot;
using System;

public partial class Entity : CharacterBody2D
{
	[Export]
	public CollisionShape2D MyPhysicsCollider; //Collider(rotates with movement)
	
	[Export]
	public AnimatedSprite2D MySpriteAnimation; //Animator
	
	AudioStreamPlayer2D Walker; //Play walking sound
	
	[Export]
	public int RoomId; //Room the entity is in (helpful for save/load)
	
	public bool Stop = false; //Whether the entity should stop moving because the player has paused something
	
	public string CurrentDir = "D"; //Start looking down
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		this.AddToGroup("Pausable");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	
	//Pause/Unpause the Entity
	public void Pause()
	{
		Stop = !Stop;
	}
	
	//Play walking sound
	public void PlayWalkingSound()
	{
		//If the sound is already playing, return
		if(Walker != null && Walker.Playing)
		{
			return;
		}
		if(RoomId == 5 || RoomId == 13)
		{ //If this is the bar, play a random wood sound
			int Chosen = (int)GD.RandRange(1, 7);
			Walker = (AudioStreamPlayer2D)GetNode("Sounds/GeneralSounds/WoodFootstep"+Chosen);
		}else{ //If this is any other room, play the metal sound
			Walker = (AudioStreamPlayer2D)GetNode("Sounds/GeneralSounds/MetalFootstep");
		}
		Walker.Play();
	}
	
	//Remove the entity from the scene(varies)
	public async virtual void Remove()
	{
		await ToSignal(MySpriteAnimation, AnimatedSprite2D.SignalName.AnimationFinished);
		this.QueueFree();
	}
}
