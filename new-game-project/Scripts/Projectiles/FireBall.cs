using Godot;
using System;

public partial class FireBall : Projectile
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Rotate the fireball based on the direction
		MySpriteAnimation.Animation = "Move";
		if(CurrentDir == "D")
		{ //Down
			Rotation = ((float)Math.PI/180)*0;
		}else if(CurrentDir == "U")
		{ //Up
			Rotation = ((float)Math.PI/180)*180;
		}else if(CurrentDir == "R")
		{ //Right
			Rotation = ((float)Math.PI/180)*-90;
		}
		else if(CurrentDir == "L")
		{ //Left
			Rotation = ((float)Math.PI/180)*90;
		}
		//Set the animation
		MySpriteAnimation.Animation = "Move";
		MySpriteAnimation.Play();
		//Despawn the bullet if it lasts more then 20 seconds
		Despawn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
		//If it has already hit something, do nothing
		if(Hit)
		{
			return;
		}
		base._Process(delta);
		//If the animation stopped playing, play it again
		if(!MySpriteAnimation.IsPlaying())
		{
			MySpriteAnimation.Animation = "Move";
			MySpriteAnimation.Play();
		}
	}
	
	//Remove the fireball
	public async override void Remove()
	{
		Speed = 0;
		QueueFree();
	}
}
