using Godot;
using System;

public partial class Enemy2 : Enemy
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure to set the damage of the enemy
		base._Ready();
		Damage = 7;
		AttackCooldown = 2;
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
	
	//Attack for enemy 2
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
			//Fire a bullet at the player
			if(body is Player)
			{
				//Create the bullets
				PackedScene ProjectileScene = GD.Load<PackedScene>("res://Packed Scenes/Projectiles/Bullet.tscn");
				Projectile NewBullet = (Projectile)ProjectileScene.Instantiate();
				float BulletDistance = ((CapsuleShape2D)MyPhysicsCollider.GetShape()).Height/2+10; //Distance the bullet will spawn from the enemy
				//Set Bullet dirction
				NewBullet.CurrentDir = CurrentDir;
				if(CurrentDir == "D")
				{ //Down
					NewBullet.Position = new Vector2(GlobalPosition.X,GlobalPosition.Y+BulletDistance);
				}
				else if(CurrentDir == "U")
				{ //Up
					NewBullet.Position = new Vector2(GlobalPosition.X,GlobalPosition.Y-BulletDistance);
				}
				else if(CurrentDir == "L")
				{ //Left
					NewBullet.Position = new Vector2(GlobalPosition.X-BulletDistance,GlobalPosition.Y);
				}
				else if(CurrentDir == "R")
				{ //Right
					NewBullet.Position = new Vector2(GlobalPosition.X+BulletDistance,GlobalPosition.Y);
				}
				//Set bullet speed and collision layers
				NewBullet.Speed = 300;
				NewBullet.SetCollisionLayerValue(4,true); //Bullet is enemy projectile
				NewBullet.SetCollisionMaskValue(5,true); //Bullet is looking for the player
				NewBullet.Direction = (NewTarget-Position).Normalized(); //Aim the bullet at the player
				NewBullet.Damage = Damage;
				//Create the bullet
				GetTree().GetRoot().GetChild(1).AddChild(NewBullet);
				//Play the animation and sound
				MySpriteAnimation.Animation = "Shoot_"+CurrentDir;
				MySpriteAnimation.Play();
				((AudioStreamPlayer2D)GetNode("Sounds/PistolLoud")).Play();
				//Begin the attack cool down
				AttackCooledDown = false;
				AttackCoolDown();
				return;
			}
		}
		
	}
}
