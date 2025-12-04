using Godot;
using System;

public partial class Enemy1 : Enemy
{
	// Telegraph visual settings
	public float telegraphDuration = 1.5f; // Time in seconds before attack lands
	public float telegraphTimer = 0f;
	public bool isTelegraphing = false;
	
	[Export]
	public ColorRect attackIndicator;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Make sure to set the damage of the enemys
		base._Ready();
		
		SetupAttackIndicator();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		
		// Update telegraph visual
		if (isTelegraphing)
		{
			telegraphTimer += (float)delta;
			float progress = Mathf.Clamp(telegraphTimer / telegraphDuration, 0f, 1f);
			
			// Update the visual to show fill progress
			UpdateTelegraphVisual(progress);
			
			// When telegraph completes, execute the attack
			if (progress >= 1f)
			{
				ExecuteAttack();
				isTelegraphing = false;
				attackIndicator.Visible = false;
			}
		}
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
		//If the player wasn't in the hostile radius, don't handle being hostile
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
		
		// Already telegraphing. Do nothing
		if(isTelegraphing)
		{
			return;
		}
		
		//If the player is in the attack radius, begin telegraph
		Godot.Collections.Array<Node2D> InAttackRadius = AttackRadius.GetOverlappingBodies();
		foreach(Node2D body in InAttackRadius)
		{
			if(body is Player player)
			{
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
				
				// Start telegraph
				StartTelegraph();
				
				//Play attack sound and animation
				int Chosen = (int)GD.RandRange(1, 7);
				AttackSound = ((AudioStreamPlayer2D)GetNode("Sounds/ChihuahuaBark"+Chosen));
				AttackSound.SetVolumeDb(-15.0f);
				AttackSound.Play();
				MySpriteAnimation.Animation = "Charge_"+CurrentDir;
				MySpriteAnimation.Play();
				
				//Begin the attack cool down
				AttackCooledDown = false;
				AttackCoolDown();
				return;
			}
		}
	}
	
	
	public void SetupAttackIndicator()
	{
		attackIndicator.Visible = false;
		//transparent red
		attackIndicator.Color = new Color(1f, 0f, 0f, 0f); 
	}
	
	public void UpdateTelegraphVisual(float progress)
	{
		//increase opacity 
		float alpha = Mathf.Lerp(0.2f, 0.6f, progress);
		attackIndicator.Color = new Color(1f, 0f, 0f, alpha);
		
		// Pulse effect
		float pulse = Mathf.Lerp(0f, 1f, progress);
		attackIndicator.Scale = new Vector2(2.4f, pulse);
	}
	
	public void ExecuteAttack()
	{
		// Get bodies in attack radius and deal damage
		Godot.Collections.Array<Node2D> InAttackRadius = AttackRadius.GetOverlappingBodies();
		foreach(Node2D body in InAttackRadius)
		{
			if(body is Player player)
			{
				if(player.Health <= 0)
				{
					GetTree().CallGroup("Pausable","Pause");
					player.Stop = false;
					return;
				}
				
				player.TakeDamage(Damage);
				return;
			}
		}
	}
	
	public void StartTelegraph()
	{
		isTelegraphing = true;
		telegraphTimer = 0f;
		attackIndicator.Visible = true;
		attackIndicator.Color = new Color(1f, 0f, 0f, 0.2f);
		attackIndicator.Scale = new Vector2(2.4f, 0.0f);
	}
	
	
}
