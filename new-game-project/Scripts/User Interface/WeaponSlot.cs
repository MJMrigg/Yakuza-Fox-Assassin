using Godot;
using System;

public partial class WeaponSlot : Node
{
	[Export]
	public TextureRect Portrait; //Portrait of the weapon
	
	public int WeaponID; //ID of the weapon
	
	[Signal] //Signal to unequip the weapon
	public delegate void UnequipEventHandler(int ID);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change the weapon
	public void EquipWeapon(string path, int ID)
	{
		WeaponID = ID;
		//If there's no path, display nothing in the slot
		if(path == "")
		{
			Portrait.Texture = null;
		}
		else
		{
			Portrait.Texture = (Texture2D)GD.Load(path);
		}
	}
	
	//Unequip the weapon
	public void UnequipWeapon()
	{
		EmitSignal(SignalName.Unequip, WeaponID);
	}
}
