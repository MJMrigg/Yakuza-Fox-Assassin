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
	
	[Export]
	public Area2D HostileRadius; //Radius the NPC will use to detect the player when hostile
	
	public bool Dying = false; //Whether the NPC is dying
	
	public ProgressBar LocalSusMeter; //Local suspicion meter of the room
	
	public bool IsBoss = false; //Whether this NPC is the boss
	
	[Export]
	public int _type; //Type of NPC
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//If the NPC spawned already in the process of dying, kill it
		if(MySpriteAnimation.Animation == ("Die_"+CurrentDir))
		{
			Remove();
			return;
		}
		this.AddToGroup("NPCs");
		LocalSusMeter = ((ProgressBar)GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).GetNode("MainUI/Main/LocalSuspicion/LocalSuspicionMeter"));
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
		
		Move();
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
		//If the animation is currently not walking
		if (!MySpriteAnimation.Animation.ToString().StartsWith("Walk"))
		{
			//If the animation is not playing, have the NPC face its current direction
			if (!MySpriteAnimation.IsPlaying())
			{
				MySpriteAnimation.Animation = "Walk_"+CurrentDir;
				MySpriteAnimation.Frame = 0;
			}
			else
			{
				//If it is playing, do nothing, as the animation must finish playing
				return;
			}
		}
		
		//Choose a new target if the target was reached or is unreachable
		if(NavAgent.IsTargetReached() || !NavAgent.IsTargetReachable())
		{
			PickNewTarget();
			return;
		}
		
		//Set up velocity
		if(IsHostile)
		{ //If the room is hostile, run away from the player
			HandleHostile();
		}
		Vector2 NextPosition = GlobalPosition.DirectionTo(NavAgent.GetNextPathPosition()).Normalized();
		Velocity = NextPosition * Speed;
		
		//Set up direction it's facing
		SetDirection();
		
		MoveAndSlide();
	}
	
	//Take damage from an attack
	public void TakeDamage(int damage)
	{
		//If the NPC is already dying, don't continue taking damage
		if(Dying || Health <= 0)
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
		
		//Make the room hostile
		if(!IsHostile)
		{
			GetTree().CallGroup("NPCs","MakeHostile");
			Game.Instance.RoomsHostile[RoomId] = true;
			//Increase the local suspicion to max
			Game.Instance.IncreaseLocalSuspicion(RoomId,Game.Instance.MaxLocalSuspicions[RoomId]);
			//LocalSusMeter.Value = Game.Instance.LocalSuspicions[RoomId];
		}
		
		//Handle if the NPC died
		if(Health <= 0)
		{
			Dying = true;
			Remove();
			return;
		}
		int Chosen = (int)(GD.Randi() % 12) + 1;
		MySpriteAnimation.Animation = "Hurt_"+CurrentDir;
		MySpriteAnimation.Play();
		if(!IsBoss)
		{
			((AudioStreamPlayer2D)GetNode("Sounds/GeneralSounds/DogHurt"+Chosen)).Play();
		}
	}
	
	//See if player is in the hostile radius
	public Vector2 GetPlayerPosition()
	{
		//Go through everything in the NPC's Hostile radius
		Godot.Collections.Array<Node2D> Entities = HostileRadius.GetOverlappingBodies();
		foreach(Node2D body in Entities)
		{
			//If the body is the player
			if(body is Player)
			{
				Vector2 PlayerPosition = ((Player)body).Position;
				return PlayerPosition;
			}
		}
		//If the player wasn't in the hostile radius, return the NPC's position. The player can't be there
		return new Vector2(Position.X,Position.Y);
	}
	
	//Handle being hostile(either by running away or attacking)
	public virtual void HandleHostile()
	{
		if(!IsHostile)
		{
			GD.Print("Error: NPC is not hostile.");
			return;
		}
		
		//Get the player's position
		Vector2 RunAway = GetPlayerPosition();
		//If the player wasn't in the hostile radiue, don't handle being hostile
		if(RunAway.X == Position.X || RunAway.Y == Position.Y)
		{
			return;
		}
		
		//Run away from the player
		if((RunAway.X < NavAgent.TargetPosition.X && NavAgent.TargetPosition.X < Position.X) || (RunAway.X > NavAgent.TargetPosition.X && NavAgent.TargetPosition.X > Position.X))
		{ //If the NPC is facing the player in the X direction, turn around
			NavAgent.TargetPosition = new Vector2(NavAgent.TargetPosition.X*-1, NavAgent.TargetPosition.Y);
			return; //If this isn't here, the NPC will change directions diagonally, which can really mess up on corners.
		}
		if((RunAway.Y < NavAgent.TargetPosition.Y && NavAgent.TargetPosition.Y < Position.Y) || (RunAway.Y > NavAgent.TargetPosition.Y && NavAgent.TargetPosition.Y > Position.Y))
		{ //If the NPC is facing the player in the Y direction, turn around
			NavAgent.TargetPosition = new Vector2(NavAgent.TargetPosition.X, NavAgent.TargetPosition.Y*-1);
		}
	}
	
	//Remove the NPC from the scene(by dying)
	public async override void Remove()
	{
		Dying = true;
		if(MySpriteAnimation.Animation != ("Die_"+CurrentDir))
		{ //If the animation isn't already set
			MySpriteAnimation.Stop();
			MySpriteAnimation.Animation = "Die_"+CurrentDir;
		}
		MySpriteAnimation.Play();
		if(MySpriteAnimation.Frame < 3)
		{ //Wait for the animation to play
			await ToSignal(MySpriteAnimation, AnimatedSprite2D.SignalName.AnimationFinished);
		}
		//The boss and NPCs have different death logic
		if(IsBoss)
		{ //Boss' death
			//Pause the game
			GetTree().CallGroup("Pausable","Pause");
			//Tell the player that they won
			Game.Instance.BossIsDead = true;
		}
		else
		{ //Normal NPC death
			await ToSignal(GetTree().CreateTimer(2),"timeout");
			this.QueueFree();
		}
	}
	
	//Pick a new target for the NPC to travel to
	public void PickNewTarget()
	{
		//Generate random set of coordinates
		float RandX = Position.X+(GD.Randf()*400 - 200);
		float RandY = Position.Y+(GD.Randf()*400 - 200);
		NavAgent.TargetPosition = new Vector2(RandX,RandY);
	}
	
	//Set the NPC's direction
	public virtual void SetDirection()
	{
		Vector2 Direction = Velocity.Normalized();
		
		//Determine direction the sprite and collider will face
		if(Mathf.Abs(Direction.X) <= 0.1 && Mathf.Abs(Direction.Y) > 0.1)
		{ //Vertical
			if(Direction.Y > 0)
			{ //Down
				CurrentDir = "D";
				MyPhysicsCollider.Rotation = 0;
			}
			else
			{ //Up
				CurrentDir = "U";
				MyPhysicsCollider.Rotation = (float)(MathF.PI/180)*180;
			}
		}
		else if(Mathf.Abs(Direction.X) > 0.1)
		{ //Horizontal or Diagonal
			if(Direction.X > 0)
			{ //Right
				CurrentDir = "R";
				MyPhysicsCollider.Rotation = (float)(MathF.PI/180)*270;
			}
			else
			{ //Left
				CurrentDir = "L";
				MyPhysicsCollider.Rotation = (float)(MathF.PI/180)*90;
			}
		}
		//Play the walking animation and sound
		MySpriteAnimation.Animation = "Walk_"+CurrentDir;
		if(!MySpriteAnimation.IsPlaying())
		{
			MySpriteAnimation.Play();
		}
		PlayWalkingSound();
	}
	
	//Make the NPC hostile
	public void MakeHostile()
	{
		IsHostile = true;
	}
}
