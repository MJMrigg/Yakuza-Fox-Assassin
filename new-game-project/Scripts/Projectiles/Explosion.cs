using Godot;
using System;

public partial class Explosion : Entity
{
	public bool Exploding = false; //Whether this explosion has gone off yet
	
	[Export]
	public Area2D ExplosionRadius; //Radius from the explosion that characters will take damage
	
	public int Damage = 25;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//If the explosion spawned in a wall, get rid of it
		if(GetSlideCollisionCount() > 0)
		{
			QueueFree();
		}
		MySpriteAnimation.Animation = "Warning";
		MySpriteAnimation.Play();
		Warning();
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
		//If the explosion is exploding, do nothing
		if(Exploding)
		{
			return;
		}
		base._Process(delta);
		//If the animation is not playing, play it
		if(!MySpriteAnimation.IsPlaying())
		{
			MySpriteAnimation.Play();
		}
	}
	
	
	//Be a warning for a set amount of time
	public async void Warning()
	{
		await ToSignal(GetTree().CreateTimer(5),"timeout");
		//Explode now that the warning is over
		Exploding = true;
		Explode();
	}
	
	//Explode
	public async void Explode()
	{
		//Play explosion animation and sound
		MySpriteAnimation.Animation = "Explode";
		MySpriteAnimation.Play();
		await ToSignal(MySpriteAnimation, AnimatedSprite2D.SignalName.AnimationFinished);
		AudioStreamPlayer ExplosionSound = ((AudioStreamPlayer)GetNode("FireExplosion"));
		ExplosionSound.Play();
		await ToSignal(ExplosionSound, AudioStreamPlayer.SignalName.Finished);
		//Go through every object in the explosion radius
		Godot.Collections.Array<Node2D> InExplosionRadius = ExplosionRadius.GetOverlappingBodies();
		foreach(Node2D body in InExplosionRadius)
		{
			//If the player was there, damage it
			if(body is Player)
			{
				((Player)body).TakeDamage(Damage);
				break;
			}
		}
		//Get rid of the explosion
		QueueFree();
	}
	
	//Despawn the explosion if it spawned in a wall
	public void Despawn(Node2D body)
	{
		QueueFree();
	}
}
