using Godot;
using System;

public partial class Projectile : Entity
{
	[Export]
	public int Damage; //Damage the projectile does upon hitting
	
	[Export]
	public float Speed; //Movement speed
	
	public Vector2 Direction; //Direction the projectile moves(determined by who shot it)
	
	public bool IsDiagonal = false; //Whether this bullet is a diagonal bullet(determined by who shot it)
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//Set the sprite frame of the bullet based on the direction
		MySpriteAnimation.Animation = "Move";
		if((Direction.Y == 1 && Direction.X == 0)  || IsDiagonal)
		{ //Up or Diagonal
			MySpriteAnimation.Frame = 0;
		}else if(Direction.Y == -1 && Direction.X == 0)
		{ //Down
			MySpriteAnimation.Frame = 2;
		}else if(Direction.Y == 0 && Direction.X == 1)
		{ //Right
			MySpriteAnimation.Frame = 1;
			MyPhysicsCollider.Rotation = ((float)Math.PI/180)*90;
		}
		else if(Direction.Y == 0 && Direction.X == -1)
		{ //Left
			MySpriteAnimation.Frame = 3;
			MyPhysicsCollider.Rotation = ((float)Math.PI/180)*90;
		}
		//Adjust Animation sprite since the right and left bullets are off by -4
		MySpriteAnimation.Position = new Vector2(0,Math.Abs(Direction.X)*-4);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		//If paused, do nothing
		if(Stop)
		{
			MySpriteAnimation.Pause();
			return;
		}
		
		//Check if the projectile collided with anyting
		if(GetSlideCollisionCount() > 0)
		{
			//Get the first object the projectile hit
			KinematicCollision2D Collision = GetSlideCollision(0);
			
			//Handle what it collided with
			if(Collision.GetCollider() is NPC CollidedNPC)
			{
				CollidedNPC.TakeDamage(Damage);
			}
			else if(Collision.GetCollider() is Player CollidedPlayer)
			{
				CollidedPlayer.TakeDamage(Damage);
			}
			
			//Remove the projectile
			QueueFree();
			return;
		}
		
		//Continue moving
		Velocity = Direction * Speed; //Set velocity
		MoveAndSlide(); //Move
	}
	
	//Despawn the bullet after 20 seconds
	public async void Despawn()
	{
		await ToSignal(GetTree().CreateTimer(20),"timeout");
		QueueFree();
	}
}
