using Godot;
using System;
using System.Collections.Generic;

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
			//Door is unlocked. It should disappear.
			SetCollisionLayerValue(1,false);
			((Sprite2D)GetNode("DoorSprite")).Visible = false;
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
	public virtual void ChangeRoom(bool AllowedThrough)
	{
		if(!Game.Instance.TutorialDone)
		{
			Game.Instance.FinishTutorial();
			//Change scene based on the room the door takes the player during the next physics process
			CallDeferred(nameof(PhysicsProcessSceneChange));
		}
		
		//DT EDIT. REMOVE IF CAN'T PUSH
		if (!AllowedThrough || !Game.Instance.roomIDS.ContainsKey(ConnectedRoom))
		{
			return;
		}
		
		int CurrentRoom = Game.Instance.PlayerRoom;
		//Save all NPC data if the player isn't leaving the bars
		if(CurrentRoom != 5 && CurrentRoom != 13)
		{ //Nothing to save in the bars
			Game.Instance.NPCs[CurrentRoom] = new List<string>(); //Overide past NPC data
			var NPCsInRoom = GetTree().GetNodesInGroup("NPCs");
			for(int i = 0; i < GetTree().GetNodeCountInGroup("NPCs"); i++)
			{
				if(NPCsInRoom[i] is NPC)
				{
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i])._type).ToString() );
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i]).Position.X).ToString());
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i]).Position.Y).ToString());
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i]).Health).ToString());
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i]).CurrentDir).ToString());
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i]).MySpriteAnimation.Animation).ToString());
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i]).MySpriteAnimation.Frame).ToString());
					Game.Instance.NPCs[CurrentRoom].Add((((NPC)NPCsInRoom[i]).Dying).ToString());
				}
			}
			//Save all item data
			var Items = GetTree().GetNodesInGroup("Items");
			int ItemCount = GetTree().GetNodeCountInGroup("Items");
			Game.Instance.RoomItems[CurrentRoom] = new List<float>(); //Overwrite previous data
			if(ItemCount > 0)
			{
				for(int i = 0; i < ItemCount; i++)
				{
					Game.Instance.RoomItems[CurrentRoom].Add(((Item)Items[i]).ID);
					Game.Instance.RoomItems[CurrentRoom].Add(((Item)Items[i]).Position.X);
					Game.Instance.RoomItems[CurrentRoom].Add(((Item)Items[i]).Position.Y);
				}
			}
			//Mark the room as having saved data
			Game.Instance.FirstSaved[CurrentRoom] = true;
		}
		//Change the music based on the room
		if(ConnectedRoom == 20 || ConnectedRoom == 5 || ConnectedRoom == 13)
		{ //If the player is going to the boss room or the bar
			((MusicPlayer)GetTree().GetRoot().GetChild(1)).ChangeSong(ConnectedRoom);
		}
		else if(CurrentRoom == 20 || CurrentRoom == 5 || CurrentRoom == 13)
		{ //If the player is leaving the boss room or the bar
			((MusicPlayer)GetTree().GetRoot().GetChild(1)).ChangeSong(ConnectedRoom);
		}
		
		//Change scene based on the room the door takes the player during the next physics process
		CallDeferred(nameof(PhysicsProcessSceneChange));
	}
	
	//Change scenes during the next physics process
	public void PhysicsProcessSceneChange()
	{
		if(!IsInsideTree())
		{
			return;
		}
		GetTree().ChangeSceneToFile(Game.Instance.roomIDS[ConnectedRoom]);
	}
	
	//Detect if the player has the key or not
	public virtual void CheckPlayer(Node2D body)
	{
		//If the door is already unlocked, let the player through
		if(CheckLock() || regDoor)
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
			if(CurrentItem == null || CurrentItem.ID < 4 || CurrentItem.ID > 5)
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
