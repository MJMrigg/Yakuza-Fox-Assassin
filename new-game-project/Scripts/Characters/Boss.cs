using Godot;
using System;

public partial class Boss : Enemy
{
	public int ExplosionDamage; //Damage that each explosion does
	
	public int ExplosionCoolDown; //Explosion attack cooldown
	
	public bool ExplosionCooledDown=false; //Whether the explosion attack has cooled down
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure to set the damage of the boss
		base._Ready();
		ExplosionDamage = 25;
		ExplosionCoolDown = 10;
		//Start out being hostile
		IsHostile = true;
		IsBoss = true;
		//Prep an explosion attack
		ExplosionCooledDown=false;
		ExplosionAttackCoolDown();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//If the NPC is dying, do nothing
		if(Health <= 0 || Dying)
		{
			return;
		}
		base._Process(delta);
	}
	
	//Attack for the boss
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
			Speed = 150;
			return;
		}
		
		//Face the player
		Speed = 50;
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
			//Shoot a fireball at the player
			if(body is Player)
			{
				//Create the fireball
				PackedScene FireBallScene = GD.Load<PackedScene>("res://Packed Scenes/Projectiles/FireBall.tscn");
				FireBall NewFireBall = (FireBall)FireBallScene.Instantiate();
				float BulletDistance = ((CapsuleShape2D)MyPhysicsCollider.GetShape()).Height/2+20; //Distance the bullet will spawn from the enemy
				//Set Bullet dirction
				NewFireBall.CurrentDir = CurrentDir;
				if(CurrentDir == "D")
				{ //Down
					NewFireBall.Position = new Vector2(GlobalPosition.X,GlobalPosition.Y+BulletDistance);
				}
				else if(CurrentDir == "U")
				{ //Up
					NewFireBall.Position = new Vector2(GlobalPosition.X,GlobalPosition.Y-BulletDistance);
				}
				else if(CurrentDir == "L")
				{ //Left
					NewFireBall.Position = new Vector2(GlobalPosition.X-BulletDistance,GlobalPosition.Y);
				}
				else if(CurrentDir == "R")
				{ //Right
					NewFireBall.Position = new Vector2(GlobalPosition.X+BulletDistance,GlobalPosition.Y);
				}
				//Set bullet speed and collision layers
				NewFireBall.Speed = 300;
				NewFireBall.SetCollisionLayerValue(4,true); //Bullet is enemy projectile
				NewFireBall.SetCollisionMaskValue(5,true); //Bullet is looking for the player
				NewFireBall.Direction = (NewTarget-Position).Normalized(); //Aim the bullet at the player
				NewFireBall.Damage = Damage;
				//Create the bullet
				GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).AddChild(NewFireBall);
				//Play the animation and sound
				MySpriteAnimation.Animation = "Shoot_"+CurrentDir;
				MySpriteAnimation.Play();
				int Chosen = (int)GD.RandRange(1, 4);
				((AudioStreamPlayer2D)GetNode("Sounds/FireBall"+Chosen)).Play();
				//Begin the attack cool down
				AttackCooledDown = false;
				AttackCoolDown();
				return;
			}
		}
	}
	
	//Create explosions all around the map with one being on the player
	public void SpawnExplosions()
	{
		//If the explosion cool down isn't finished yet, do nothing
		if(!ExplosionCooledDown)
		{
			return;
		}
		//Generate random explosions
		int Amount = (int)GD.RandRange(5,10);
		PackedScene ExplosionScene = GD.Load<PackedScene>("res://Packed Scenes/Projectiles/Explosion.tscn");
		for(int i = 0; i <= Amount; i++)
		{
			//Pick random x and y coordinates
			float RandX = GlobalPosition.X+(float)GD.RandRange(-500,500);
			float RandY = GlobalPosition.Y+(float)GD.RandRange(-300,300);
			//Create Explosion
			Explosion NewExplosion = (Explosion)ExplosionScene.Instantiate();
			NewExplosion.Position = new Vector2(RandX,RandY);
			GetTree().GetRoot().AddChild(NewExplosion);
		}
		//Place an explosion on the player
		Vector2 PlayerPosition = GetPlayerPosition();
		Explosion PlayerExplosion = (Explosion)ExplosionScene.Instantiate();
		PlayerExplosion.Position = new Vector2(PlayerPosition.X, PlayerPosition.Y);
		GetTree().GetRoot().AddChild(PlayerExplosion);
		//Begin Explosion attack cool down
		ExplosionCooledDown = false;
		ExplosionAttackCoolDown();
	}
	
	//Cool down for explosion attack
	public async void ExplosionAttackCoolDown()
	{
		ExplosionCooledDown = false;
		await ToSignal(GetTree().CreateTimer(ExplosionCoolDown),"timeout");
		ExplosionCooledDown = true;
		if(!Stop)
		{ //If not paused, continue spawning explosions
			SpawnExplosions();
		}
	}
}
