using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaxValue = Game.Instance.MaxPlayerHealth;
		Value = Game.Instance.PlayerHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Update player health in real time
		Value = Game.Instance.PlayerHealth;
	}
}
