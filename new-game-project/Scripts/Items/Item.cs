using Godot;
using System;

public partial class Item : Interactable
{
	[Export]
	public int ID; //Unique ID of the item
	
	[Export]
	public CompressedTexture2D Portrait; //Portrait of the item(for when it's in the inventory)
	
	public bool Equipable; //If the item can be equiped
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
}
