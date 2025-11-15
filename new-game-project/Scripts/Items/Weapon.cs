using Godot;
using System;

public partial class Weapon : Item
{
	[Export]
	public int Damage; //Damage the weapon does
	
	[Export]
	public int CoolDown; //Cool down of the weapon
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Equipable = true; //Weapons can be equiped
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
