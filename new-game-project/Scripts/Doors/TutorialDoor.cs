using Godot;
using System;

public partial class TutorialDoor : Door
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void ChangeRoom(bool AllowedThrough)
	{
		base.ChangeRoom(AllowedThrough);
	}
	
	public override void CheckPlayer(Node2D body)
	{
		GD.Print("Tut dorr check");
		//If the door is already unlocked, let the player through
		if(CheckLock() || regDoor)
		{
			ChangeRoom(true);
		}
		//NPCs can't go through the door
		if(!(body is Player))
		{
			GD.Print("NOT PLAYER");
			return;
		}
		//Check the player's inventory for any keys
		Player player = (Player)body;
		Inventory PlayerInventory = player.Inv;
		for(int i = 0; i < PlayerInventory.ItemsStored.Length; i++)
		{
			Item CurrentItem = PlayerInventory.ItemsStored[i];
			//If the player does not have an item in that part of the inventory or that item is not a key, move on
			if(CurrentItem == null || CurrentItem.ID != 7)
			{
				continue;
			}
			GD.Print("Blue key found");
			//Check if the key's color is the same as the door color
			ChangeLock(true);
			ChangeRoom(true);
			//If the key unlocked the door, move on
			if(CheckLock())
			{
				return;
			}
		}
		
	}
}
