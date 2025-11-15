using Godot;
using System;

public partial class Inventory : VBoxContainer
{
	public Item[] ItemsStored; //Array of items stored
	
	public Weapon[] EquipedWeapons = new Weapon[2]; //Array of equiped weapons
	
	//Signal to tell the weapon slot to change to a different texture
	[Signal]
	public delegate void ChangeMeleeWeaponEventHandler(string path);
	[Signal]
	public delegate void ChangeRangedWeaponEventHandler(string path);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Equip a new item
	public void EquipItem(int NewEquip)
	{
		if(NewEquip == 2)
		{ //Equip the knife
			EmitSignal(SignalName.ChangeMeleeWeapon,"res://Art Assets/Items/knife.png");
		}
		else if(NewEquip == 3)
		{ //Equip the shotgun
			EmitSignal(SignalName.ChangeRangedWeapon,"res://Art Assets/Items/gun_Shotgun.png");
		}
	}
	
	//Unequip an item
	public void UnEquipItem(int UnEquip)
	{
	}
	
	//Close the Iventory menu
	public void ReturnToGame()
	{
		Visible = false;
	}
}
