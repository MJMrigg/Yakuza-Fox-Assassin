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
		
		// Set exitAnimPlayed in Game.cs to false to prevent Paul from despawning
		Game.Instance.exitAnimPlayed = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
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
		
		//If Paul is supposed to be moving to an exit, and this destination has not been set yet
		if(Game.Instance.paulCanMove && Game.Instance.PatrolRoom != Game.Instance.destPatrolRoom && TimeToMove){
			//Pick final destination
			MoveToExit();
			TimeToMove = false;
			paulStuckTime();
			GD.Print("Final Destination Selected");
			return;
		}
		
		// Checl for final dest
		if(!TimeToMove && (NavAgent.IsTargetReached() || !NavAgent.IsTargetReachable() || stuckTime))
		{
			//Final destination has been reached. Move to new room
			Game.Instance.exitAnimPlayed = true;
			GD.Print("Final Destination reached");
			return;
		}
		
		//Choose a new target if the target was reached or is unreachable unless we are moving to the final destination
		if((NavAgent.IsTargetReached() || !NavAgent.IsTargetReachable()) && TimeToMove)
		{
			PickNewTarget();
			GD.Print("New Random Point selected");
			return;
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
