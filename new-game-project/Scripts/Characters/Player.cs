using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export]
	public AnimatedSprite2D MySpriteAnimation; //Sprite of the player

	public string CurrentDir = "D"; //Start looking down
	
	public int MaxHealth = 100; //Store maximum health for healing purposes
	
	public int Health; //Store player's health
	
	[Export]
	public FollowCamera PlayerCamera; //Camera that follows the player
	
	[Export]
	public CollisionShape2D PhysicsCollider; //Physics collider of the player
	
	public bool RangedCooldown = true; //Whether the ranged attack cool down is finished
	
	public override void _Ready()
	{
		base._Ready();

		Velocity = new Vector2(0, 0); //Don't move at the start
		
		Health = MaxHealth; //Start at maximum health
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		//If the player wishes to create a projectile and their cooldown is over
		if(Input.IsActionJustPressed("p_ranged") && RangedCooldown)
		{
			CreateProjectile();
		}
		
		//If the animation is currently not walking
		if (!MySpriteAnimation.Animation.ToString().StartsWith("Walk"))
		{
			//If the animation is not playing, have the player face its current direction
			if (!MySpriteAnimation.IsPlaying())
			{
				MySpriteAnimation.Animation = "Walk_" + CurrentDir;
				MySpriteAnimation.Frame = 0;
			}
			else
			{
				//If it is playing, do nothing, as the animation must finish playing
				return;
			}
		}

		//Get player input and set up velocity
		float hInput = Input.GetAxis("p_a", "p_d");
		float vInput = Input.GetAxis("p_s", "p_w");
		Velocity = new Vector2(0,0);	
		if (Mathf.Abs(hInput) < 0.1f && Mathf.Abs(vInput) < 0.1f)
		{ //If the velocity is 0, just play the first frame of walking
			MySpriteAnimation.Frame = 0;
		}
		else if (Mathf.Abs(hInput) > Mathf.Abs(vInput))
		{
			//If the player is moving left or right
			PhysicsCollider.Rotation = ((float)Math.PI/180)*90.0f;
			if (hInput > 0)
			{ //If the player is moving right
				Velocity += new Vector2(100, 0);
				MySpriteAnimation.Animation = "Walk_R";
				CurrentDir = "R";
			}
			else
			{ 
				//If the player is moving left
				Velocity += new Vector2(-100, 0);
				MySpriteAnimation.Animation = "Walk_L";
				CurrentDir = "L";
			}
		}
		else
		{ //If the player is moving up or down
			PhysicsCollider.Rotation = 0;
			MySpriteAnimation.Play();
			if (vInput > 0)
			{
				//up
				Velocity += new Vector2(0, -100);
				MySpriteAnimation.Animation = "Walk_U";
				CurrentDir = "U";
			}
			else
			{
				//down
				Velocity += new Vector2(0, 100);
				MySpriteAnimation.Animation = "Walk_D";
				CurrentDir = "D";
			}
		}

		MoveAndSlide();
		
		//Move the player camera along with the player
		if(PlayerCamera != null)
		{
			PlayerCamera.ChangePosition(Position);
		}

	}
	
	//Open inventory
	public void OpenInv()
	{
		
	}
	
	//Shoot a bullet
	public void CreateProjectile()
	{
		//If the ranged weapon is still cooling down, don't shoot the bullet
		if(!RangedCooldown)
		{
			return;
		}
		//Unpack the scene
		PackedScene PlayerProjectileScene = GD.Load<PackedScene>("res://Packed Scenes/Projectiles/Player Projectile.tscn");
		Projectile NewBullet = (Projectile)PlayerProjectileScene.Instantiate();
		//Set Position and Direction
		if(CurrentDir == "D")
		{
			NewBullet.Direction = new Vector2(0,1);
		}
		else if(CurrentDir == "U")
		{
			NewBullet.Direction = new Vector2(0,-1);
		}
		else if(CurrentDir == "L")
		{
			NewBullet.Direction = new Vector2(-1,0);
		}
		else if(CurrentDir == "R")
		{
			NewBullet.Direction = new Vector2(1,0);
		}
		NewBullet.Position = new Vector2(NewBullet.Direction.X*65,NewBullet.Direction.Y*65);
		//Create the bullet
		AddChild(NewBullet);
	}
	
	//Remove a door when opening it from the unlocked side
	public void PlayerRemovesDoor()
	{
		
	}
	
	//Use a secret passage
	public void UseSecretPass()
	{
		
	}
	
	//Increase local suspicion because of an event
	public void IncreaseLocalSus()
	{
		
	}
	
	//Tell the game that the player died
	public void InformOfDeath()
	{
		
	}
	
	//Deal damage when attacking something
	public void DealDamage()
	{
		
	}
	
	//Take damage when attacked or hit by a projectile
	public void TakeDamage(int damage)
	{
		Health -= damage;
		//If the player's health is below 0, die
		if(Health <= 0)
		{
			//Death logic goes here
			InformOfDeath();
		}
	}
	
	public async void RangedCooldown()
	{
		
	}
}
