using Godot;
using System;

public partial class Enemy3 : Enemy
{
	[Export]
	public bool Transformed = false; //Whether the tanuki is transformed
	
	[Export]
	public Area2D TransformRadius; //Distance player has to be for tanuki to transform
	
	public Player Ambushee = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//Make sure to set the damage of the enemy
		Damage = 10;
		AttackCooldown = 2;
		//If the Tanuki is transformed, it shouldn't move
		if(Transformed)
		{
			Speed = 0;
			//Transform the tanuki
			MySpriteAnimation.Animation = "TransformOut_"+CurrentDir;
			MySpriteAnimation.Frame = 0;
		}
		else
		{
			Speed = 210;
			MySpriteAnimation.Animation = "Walk_"+CurrentDir;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//If the NPC is dying, do nothing
		if(Dying)
		{
			return;
		}
		
		//If the tanuki is transformed
		if(Transformed)
		{
			//Only unstransform if the player is near and the tanuki is hostile
			if(IsHostile && Ambushee != null)
			{
				Transform();
			}
			else
			{
				return;
			}
		}
		base._Process(delta);
	}
	
	//Attack for enemy 3
	public override void HandleHostile()
	{
		if(!IsHostile)
		{
			return;
		}
		
		//Get the player's position
		Vector2 NewTarget = GetPlayerPosition();
		//If the player wasn't in the hostile radiue, don't handle being hostile
		if(NewTarget.X == Position.X || NewTarget.Y == Position.Y)
		{
			Speed = 210;
			return;
		}
		
		//Face the player
		Speed = 100;
		NavAgent.TargetPosition = NewTarget;
		
		//If the attack is still cooling down, do nothing
		if(!AttackCooledDown)
		{
			return;
		}
		
		//If the attack is still cooling down, do nothing
		if(!AttackCooledDown)
		{
			return;
		}
		
		//If the player is in the attack radius, attack the player
		Godot.Collections.Array<Node2D> InAttackRadius = AttackRadius.GetOverlappingBodies();
		foreach(Node2D body in InAttackRadius)
		{
			//Fire a bullet at only the player
			if(!(body is Player))
			{
				continue;
			}
			//Create the bullets
			PackedScene ProjectileScene = GD.Load<PackedScene>("res://Packed Scenes/Projectiles/Bullet.tscn");
			Projectile NewBullet = (Projectile)ProjectileScene.Instantiate();
			float BulletDistance = ((CapsuleShape2D)MyPhysicsCollider.GetShape()).Height/2+50; //Distance the bullet will spawn from the enemy
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
			//Create the bullet
			GetTree().GetRoot().AddChild(NewBullet);
			//Play the attack sound and animation
			int Chosen = (int)GD.RandRange(1, 4);
			((AudioStreamPlayer2D)GetNode("Sounds/TanukiBark"+Chosen)).Play();
			MySpriteAnimation.Animation = "Throw_"+CurrentDir;
			MySpriteAnimation.Play();
			//Begin the attack cool down
			AttackCooledDown = false;
			AttackCoolDown();
			return;
		}
	}
	
	//Transform back into the normal tanuki
	public async void Transform()
	{
		//If the tanuki is already not transformed or hostile, do nothing
		if(!Transformed || !IsHostile)
		{
			return;
		}
		Transformed = false;
		IsHostile = true;
		//Transform
		MySpriteAnimation.Animation = "TransformOut_"+CurrentDir;
		MySpriteAnimation.Play();
		((AudioStreamPlayer2D)GetNode("Sounds/TanukiDisguise")).Play();
		await ToSignal(MySpriteAnimation, AnimatedSprite2D.SignalName.AnimationFinished);
		//Return to normal functionality
		MySpriteAnimation.Animation = "Walk_"+CurrentDir;
	}
	
	//Ambush the player
	public void Ambush(Node2D body)
	{
		if(body is Player)
		{
			Ambushee = (Player)body;
		}
	}
	
	//The player has left their range
	public void LieInWait(Node2D body)
	{
		if(body is Player)
		{
			Ambushee = null;
		}
	}
}
