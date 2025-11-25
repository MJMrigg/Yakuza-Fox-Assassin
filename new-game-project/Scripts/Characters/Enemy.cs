using Godot;
using System;

public partial class Enemy : NPC
{
	[Export]
	public int Damage; //Attack damage
	
	[Export]
	public Area2D AttackRadius; //Attack range
	
	[Export]
	public int AttackCooldown; //Time attack takes to cool down
	
	public bool AttackCooledDown=true; //Whether the attack has finished cooling down
	
	public AudioStreamPlayer2D AttackSound; //Sound for attacks
	
	[Export]
	public AudioStreamPlayer2D DeathSound; //Sound that plays upon death
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//If the NPC is dying, do nothing
		if(Dying)
		{
			return;
		}
		base._Process(delta);
	}
	
	//Wait for the attack to cool down
	public async void AttackCoolDown()
	{
		AttackCooledDown = false;
		await ToSignal(GetTree().CreateTimer(AttackCooldown),"timeout");
		AttackCooledDown = true;
	}
	
	//Die
	public async override void Remove()
	{
		DeathSound.Play();
		base.Remove();
	}
}
