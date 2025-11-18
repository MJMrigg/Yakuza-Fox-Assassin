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
	
	public bool Hit = false; //Whether the bullet has hit something
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//Set the sprite frame of the bullet based on the direction
		MySpriteAnimation.Animation = "Move";
		if(CurrentDir == "D")
		{ //Down
			MySpriteAnimation.Frame = 0;
		}else if(CurrentDir == "U")
		{ //Up
			MySpriteAnimation.Frame = 2;
		}else if(CurrentDir == "R")
		{ //Right
			MySpriteAnimation.Frame = 1;
			MyPhysicsCollider.Rotation = ((float)Math.PI/180)*90;
		}
		else if(CurrentDir == "L")
		{ //Left
			MySpriteAnimation.Frame = 3;
			MyPhysicsCollider.Rotation = ((float)Math.PI/180)*90;
		}
		//Adjust Animation sprite since the right and left bullets are off by -4
		MySpriteAnimation.Position = new Vector2(0,Math.Abs(Direction.X)*-4);
		//Despawn the bullet if it lasts more then 20 seconds
		Despawn();
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
		else if(!Stop && !MySpriteAnimation.IsPlaying() && MySpriteAnimation.Animation != "Move")
		{
			MySpriteAnimation.Play();
		}
		
		//If the bullet had hit something, do nothing
		if(Hit)
		{
			return;
		}
		
		//Check if the projectile collided with anyting
		if(GetSlideCollisionCount() > 0)
		{
			//Get the first object the projectile hit
			KinematicCollision2D Collision = GetSlideCollision(0);
			
			//Handle what it collided with
			if(Collision.GetCollider() is NPC CollidedNPC)
			{ //NPC
				CollidedNPC.TakeDamage(Damage);
			}
			else if(Collision.GetCollider() is Player CollidedPlayer)
			{ //Player
				CollidedPlayer.TakeDamage(Damage);
			}else if(Collision.GetCollider() is Projectile CollidedProjectile)
			{ //Other Projectile
				CollidedProjectile.Remove();
			}
			
			//Mark the projectile as having hit something
			Hit = true;
			
			//Get rid of the projectile
			Remove();
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
	
	//Remove the bullet when it hits something or is hit
	public async override void Remove()
	{
		//Stop moving
		Speed = 0;
		//Readjust the animation sprite since the left and right animations for moving where off by -4
		if(CurrentDir == "R" || CurrentDir == "L")
		{
			MySpriteAnimation.Position = new Vector2(0,Math.Abs(Direction.X)*-1/4);
		}
		//Play the animation
		MySpriteAnimation.Animation = "Hit_"+CurrentDir;
		MySpriteAnimation.Play();
		//When it finishes, delete the projectile
		base.Remove();
	}
}
