using Godot;
using System;

public partial class PatrollingNPC : Enemy
{
	public bool Unconcious; //Whether Paul is unconcious
	
	public bool TimeToMove = true; //Whether it's time to move to another room
	
	public bool stuckTime = false; //A check for if Paul gets stuck
	
	public int[] PawPatrol; //Path around the map
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure to set the damage of the boss
		base._Ready();
		Damage = 15;
		AttackCooldown = 3;
		
		// Set exitAnimPlayed in Game.cs to false to prevent Paul from despawning
		Game.Instance.exitAnimPlayed = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		//GD.Print("PAUL PROCESS");
	}
	
	//Attack for Paul
	public override void HandleHostile()
	{
		//If the function was accidently called, don't run it
		if(!IsHostile)
		{
			return;
		}
		
		//Get the player's position
		Vector2 NewTarget = GetPlayerPosition();
		//If the player wasn't in the hostile radiue, don't handle being hostile
		if(NewTarget.X == Position.X || NewTarget.Y == Position.Y)
		{
			return;
		}
		
		//Face the player
		NavAgent.TargetPosition = NewTarget;
		
		//If the attack is still cooling down, do nothing
		if(!AttackCooledDown)
		{
			return;
		}
		
		//If the player is in the attack radius, attack the player
		Godot.Collections.Array<Node2D> InAttackRadius = AttackRadius.GetOverlappingBodies();
		foreach(Node2D body in InAttackRadius)
		{
			//Attack the player
			if(body is Player)
			{
				Player player = (Player)body;
				//If the player is already dead, pause the game
				if(player.Health <= 0)
				{
					GetTree().CallGroup("Pausable","Pause");
					player.Stop = false;
					return;
				}
				//If the attack sound is still playing, do not attack
				if(AttackSound != null && AttackSound.Playing)
				{
					return;
				}
				//Play attack sound and animation
				int Chosen = (int)GD.Randi()%8 + 1;
				AttackSound = ((AudioStreamPlayer2D)GetNode("Sounds/PaulBark"+Chosen));
				AttackSound.SetVolumeDb(-15.0f);
				AttackSound.Play();
				//PAUL DOES NOT HAVE AN ATTACK ANIMATION RIGHT NOW
				player.TakeDamage(Damage);
				//Begin the attack cool down
				AttackCooledDown = false;
				AttackCoolDown();
				return;
			}
		}
	}
	
	//Paul falls unconsious instead of being removed from the scene
	public async overrided void Remove()
	{
		GD.Print("PAUL REMOVE");
		Dying = true;
		Game.Instance.unconsious = true;
		//Paul does not have an unconcious animation atm. This is where it would place
	}
	
	//Paul has special move logic
	public override void Move()
	{
		//If the animation is currently not walking && should not be ideling
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
		
		//If Paul is supposed to be moving to an exit, this destination has not been set yet
		if(Game.Instance.paulCanMove && Game.Instance.PatrolRoom != Game.Instance.destPatrolRoom && TimeToMove && !IsHostile){
			//Pick final destination
			MoveToExit();
			TimeToMove = false;
			paulStuckTime();
			//GD.Print("Final Destination Selected");
			return;
		}
		
		// Checl for final dest
		if(!TimeToMove && !IsHostile && (NavAgent.IsTargetReached() || !NavAgent.IsTargetReachable() || stuckTime))
		{
			//Final destination has been reached. Move to new room
			Game.Instance.exitAnimPlayed = true;
			//GD.Print("Final Destination reached");
			return;
		}
		
		//Choose a new target if the target was reached or is unreachable unless we are moving to the final destination
		if((NavAgent.IsTargetReached() || !NavAgent.IsTargetReachable()) && TimeToMove)
		{
			PickNewTarget();
			//GD.Print("New Random Point selected");
			return;
		}
		
		if(IsHostile)
		{ //If the room is hostile, run away from the player
			HandleHostile();
			//GD.Print("Paul is coming for your ass");
		}
		
		//Set up velocity
		Vector2 NextPosition = GlobalPosition.DirectionTo(NavAgent.GetNextPathPosition()).Normalized();
		Velocity = NextPosition * Speed;
		
		//Set up direction it's facing
		SetDirection();
		
		MoveAndSlide();
	}
	
	public void MoveToExit()
	{
		NavAgent.TargetPosition = Game.Instance.roomMap[Game.Instance.PatrolRoom][Game.Instance.destPatrolRoom];
	}
	
	public async void paulStuckTime()
	{
		stuckTime = false;
		//Give Paul 15 seconds to reach destination
		await ToSignal(GetTree().CreateTimer(15, false),"timeout");
		stuckTime = true;
	}
	
	//Timer for events
	public async void timer()
	{
		
	}
	
	//Increase the suspicion threshold of Paul's current room
	public void IncreaseSuspicionThreshold()
	{
		var curRoom = Game.Instance.PatrolRoom;
		Game.Instance.LocalSuspicionThresholds[curRoom] = Game.Instance.LocalSuspicions[curRoom];
	}
}
