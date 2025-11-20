using Godot;
using System;

public partial class Door : StaticBody2D
{
	public bool Unlocked; //If the door is unlocked or not
	
	[Export]
	public bool DoorColor; //What color the door is
	
	[Export]
	public int ConnectedRoom; //The room that the door leads to
	
	//DT ADDITION. REMOVE IF NEEDED
	[Export]
	public bool regDoor;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//DT ADDITION. REMOVE IF NEEDED
		if (regDoor){
			Unlocked = true; 
		} else {
			Unlocked = false; //Start the door with being unlocked
		}
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change if the door is locked or unlocked
	public void ChangeLock(bool ConditionsMet)
	{
		if(ConditionsMet == DoorColor)
		{
			Unlocked = true;
			ChangeRoom(Unlocked);
		}
	}
	
	//Check if the door is unlocked
	public bool CheckLock()
	{
		return Unlocked;
	}
	
	//Change to the next room
	public void ChangeRoom(bool AllowedThrough)
	{
		GD.Print("TEST");
		//DT EDIT. REMOVE IF CAN'T PUSH
		if (AllowedThrough && Game.Instance.roomIDS.ContainsKey(ConnectedRoom))
		{
			var x = Game.Instance.roomIDS[ConnectedRoom];
			GD.Print("Connected Room UID" + x);
			GetTree().ChangeSceneToFile(Game.Instance.roomIDS[ConnectedRoom]);
		}
		
	}
	
	//Detect if the player has the key or not
	public void CheckPlayer(Node2D body)
	{
		//If the door is already unlocked, let the player through
		if(CheckLock())
		{
			ChangeRoom(true);
		}
		//NPCs can't go through the door
		if(!(body is Player))
		{
			return;
		}
		//Check the player's inventory for any keys
		Player player = (Player)body;
		Inventory PlayerInventory = player.Inv;
		for(int i = 0; i < PlayerInventory.ItemsStored.Length; i++)
		{
			Item CurrentItem = PlayerInventory.ItemsStored[i];
			//If the player does not have an item in that part of the inventory or that item is not a key, move on
			if(CurrentItem == null || CurrentItem.ID < 4)
			{
				continue;
			}
			//Check if the key's color is the same as the door color
			Key PotentialKey = (Key)CurrentItem;
			ChangeLock(PotentialKey.KeyColor);
			//If the key unlocked the door, move on
			if(CheckLock())
			{
				return;
			}
		}
	}
	
	//Let the player through if they're coming in through the unlocked side
	public void LetPlayerThrough(Node2D body)
	{
		//NPCs can't go through the door
		if(!(body is Player))
		{
			return;
		}
		ChangeRoom(true);
	}
	
}
