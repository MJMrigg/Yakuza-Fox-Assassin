using Godot;
using System;

public partial class Enemy1 : Enemy
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure to set the damage of the enemys
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	
	//Attack for enemy 1
	public async override void HandleHostile()
	{
		//Increase speed
		if(Game.Instance.TutorialDone)
		{
			Speed = 270;
		}
		
		//Get the player's position
		Vector2 NewTarget = GetPlayerPosition();
		//If the player wasn't in the hostile radiue, don't handle being hostile
		if(NewTarget.X == Position.X || NewTarget.Y == Position.Y)
		{
			return;
		}
		
		//Chase after the player
		NavAgent.TargetPosition = new Vector2(NewTarget.X, NewTarget.Y);
		
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
				int Chosen = (int)GD.RandRange(1, 7);
				AttackSound = ((AudioStreamPlayer2D)GetNode("Sounds/ChihuahuaBark"+Chosen));
				AttackSound.SetVolumeDb(-15.0f);
				AttackSound.Play();
				MySpriteAnimation.Animation = "Charge_"+CurrentDir;
				MySpriteAnimation.Play();
				player.TakeDamage(Damage);
				//Begin the attack cool down
				AttackCooledDown = false;
				AttackCoolDown();
				return;
			}
		}
	}
}
