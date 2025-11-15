using Godot;
using System;

public partial class InventorySlot : VBoxContainer
{
	public int ID; //Weapon Id
	
	[Signal]
	public delegate void EquipEventHandler(int id, int index);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//When the button is pressed, equip the weapon
	public void WhenPressed()
	{
		//Emit the id and index in the inventory the weapon is in
		EmitSignal(SignalName.Equip, ID, GetIndex());
	}
}
