using Godot;
using System;

public partial class Inventory : VBoxContainer
{
	public Item[] ItemsStored = new Item[6]; //Array of items stored
	
	public Weapon[] EquipedWeapons = new Weapon[2]; //Array of equiped weapons
	
	//Signal to tell the weapon slot to change to a different texture
	[Signal]
	public delegate void ChangeMeleeWeaponEventHandler(string path, int ID);
	[Signal]
	public delegate void ChangeRangedWeaponEventHandler(string path, int ID);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Equip a new item
	public void EquipItem(int NewEquip, int WeaponIndex)
	{
		string path = "";
		if(NewEquip % 2 == 0)
		{ //Equip a melee weapon
			if(NewEquip == 2)
			{ //Equip the knife
				path = "res://Art Assets/Items/knife.png";
			}
			EmitSignal(SignalName.ChangeMeleeWeapon, path, NewEquip);
			EquipedWeapons[0] = (Weapon)ItemsStored[WeaponIndex];
		}
		else
		{ //Equip a ranged weapon
			if(NewEquip == 1)
			{ //Equip the pistol
				path = "res://Art Assets/Items/gun_Pistol.png";
			}
			else if(NewEquip == 3)
			{ //Equip the shot gun
				path = "res://Art Assets/Items/gun_Shotgun.png";
			}
			EmitSignal(SignalName.ChangeRangedWeapon, path, NewEquip);
			EquipedWeapons[1] = (Weapon)ItemsStored[WeaponIndex];
		}
	}
	
	//Unequip an item
	public void UnEquipItem(int UnEquip)
	{
		if(UnEquip % 2 == 0)
		{ //Uequip the melee weapon
			EquipedWeapons[0] = null;
			EmitSignal(SignalName.ChangeMeleeWeapon,"",-1);
		}
		else
		{ //Unequip the ranged weapon
			EquipedWeapons[1] = null;
			EmitSignal(SignalName.ChangeRangedWeapon,"",-1);
		}
	}
	
	//Close the Iventory menu
	public void ReturnToGame()
	{
		Visible = false;
	}
}
