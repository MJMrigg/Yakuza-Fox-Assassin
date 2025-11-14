using Godot;
using System;

public partial class Inventory : VBoxContainer
{
	public Item[] ItemsStored; //Array of items stored
	
	public Weapon[] EquipedWeapons; //Array of equiped weapons
	
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
