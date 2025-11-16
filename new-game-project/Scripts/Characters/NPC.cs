using Godot;
using System;

public partial class NPC : Interactable
{
	[Export]
	public float Speed = 100; //Movement speed
	
	[Export]
	public int Health = 100; //Health
	
	public bool IsHostile; //Whether the NPC is hostile
	
	[Export]
	public CompressedTexture2D Portrait; //Dialogue portrait
	
	[Export]
	public NavigationAgent2D NavAgent; //Navigation Agent
	
	public bool Dying = false; //Whether the NPC is dying
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		PickNewTarget();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//If the NPC is dying, do nothing
		if(Dying)
		{
			return;
		}
		
		base._Process(delta);
		
		if(Stop)
		{
			MySpriteAnimation.Pause();
			return;
		}
		
		//Choose a new target if the target was reached or is unreachable
		if(NavAgent.IsTargetReached() || !NavAgent.IsTargetReachable())
		{
			PickNewTarget();
			return;
		}
		
		//Set up velocity
		Vector2 NextPosition = GlobalPosition.DirectionTo(NavAgent.GetNextPathPosition());
		Velocity = NextPosition * Speed;
		
		//Set up direction it's facing
		SetDirection();
		
		MoveAndSlide();
		
	}
	
	//Add protrait to dialogue box
	public override void BeginDialogue()
	{
		base.BeginDialogue();
		TextureRect DialoguePortrait = ((TextureRect)DialogueBox.GetNode("Portrait"));
		DialoguePortrait.Texture = Portrait;
		DialoguePortrait.Visible = true;
	}
	
	//Move around
	public virtual void Move()
	{
		
	}
	
	//Take damage from an attack
	public void TakeDamage(int damage)
	{
		//If the NPC is already dying, don't continue taking damage
		if(Dying)
		{
			return;
		}
		
		//Make sure damage is valid
		if(damage <= 0)
		{
			GD.Print("Error: Damage is "+damage);
			return;
		}
		
		//Take away health
		Health -= damage;
		
		//Mark the NPC has hostile
		IsHostile = true;
		
		//Handle if the NPC died
		if(Health <= 0)
		{
			Dying = true;
			Remove();
		}
		int Chosen = (int)(GD.Randi() % 12) + 1;
		((AudioStreamPlayer2D)GetNode("Sounds/GeneralSounds/DogHurt"+Chosen)).Play();
	}
	
	//Handle being hostile(either by running away or attacking)
	public virtual void HandleHostile()
	{
		
	}
	
	//Remove the NPC from the scene(by dying)
	public async override void Remove()
	{
		MySpriteAnimation.Animation = "Die_"+CurrentDir;
		MySpriteAnimation.Play();
		base.Remove();
	}
	
	//Pick a new target for the NPC to travel to
	public void PickNewTarget()
	{
		float RandX = Position.X+(GD.Randf()*400 - 200);
		float RandY = Position.Y+(GD.Randf()*400 - 200);
		NavAgent.TargetPosition = new Vector2(RandX,RandY);
	}
	
	//Set the NPC's direction
	public virtual void SetDirection()
	{
		//Determine direction the sprite and collider will face
		if(Mathf.Abs(Velocity.X) <= 0.1 && Mathf.Abs(Velocity.Y) > 0.1)
		{ //Vertical
			if(Velocity.Y > 0)
			{ //Down
				CurrentDir = "D";
			}
			else
			{ //Up
				CurrentDir = "U";
			}
			//Rotate the collider to be vertical
			MyPhysicsCollider.Rotation = 0;
		}
		else if(Mathf.Abs(Velocity.X) > 0.1)
		{ //Horizontal or Diagonal
			if(Velocity.X > 0)
			{ //Right
				CurrentDir = "R";
			}
			else
			{ //Left
				CurrentDir = "L";
			}
			MyPhysicsCollider.Rotation = (float)(MathF.PI/180)*90;
		}
		//Play the walking animation and sound
		MySpriteAnimation.Animation = "Walk_"+CurrentDir;
		if(!MySpriteAnimation.IsPlaying())
		{
			MySpriteAnimation.Play();
		}
		PlayWalkingSound();
	}
}
