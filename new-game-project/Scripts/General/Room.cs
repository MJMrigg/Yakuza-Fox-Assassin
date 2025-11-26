using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class Room : Node2D
{
	[Export]
	public int RoomId; //Identifier of the room
	
	public bool paulNotSpawned = true;
		
	public override void _Ready()
	{
		// NOTE TO SELF - DT
		/*
		When this scene is loaded the following needs to happen
		1. Every NPC in this room needs to be placed properly
		2. The Player needs to be moved to the position appropriate for how they entered.
		*/
		base._Ready();
		
		//Assign all of the objects in the room to this room
		var Objects = GetTree().GetNodesInGroup("Pausable");
		for(int i = 0; i < GetTree().GetNodeCountInGroup("Pausable"); i++)
		{
			if(Objects[i] is Entity)
			{
				((Entity)Objects[i]).RoomId = this.RoomId;
			}
		}
		
		//Set local and global suspicion meters
		ProgressBar LocalSuspicion = (ProgressBar)GetNode("MainUI/Main/LocalSuspicion/LocalSuspicionMeter");
		LocalSuspicion.MaxValue = Game.Instance.MaxLocalSuspicions[RoomId];
		LocalSuspicion.Value = Game.Instance.LocalSuspicions[RoomId];
		ProgressBar GlobalSuspicion = (ProgressBar)GetNode("MainUI/GlobalSuspicion/GlobalSuspicionMeter");
		GlobalSuspicion.MaxValue = Game.Instance.MaxGlobalSuspicion;
		GlobalSuspicion.Value = Game.Instance.GlobalSuspicion;
		
		// Determine Player Position
		if(Game.Instance.roomMap.ContainsKey(RoomId)){
			var RoomPlayerStats = Game.Instance.roomMap[RoomId];
			if(RoomPlayerStats.ContainsKey(Game.Instance.PlayerRoom)){
				var PlayerPosition = RoomPlayerStats[Game.Instance.PlayerRoom];
				//Set player position
				Player player = (Player)GetNode("Player");
				if(player == null)
				{
					PackedScene PlayerScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/Player.tscn");
					player = (Player)PlayerScene.Instantiate();
				}
				player.Position = PlayerPosition;
			}
		} else {
			GD.Print("This rooms position is not recorded in roomMap");
		}
		
		// Set Player's Room to this room now
		Game.Instance.PlayerRoom = RoomId;
		
		//Load NPC data
		if(Game.Instance.NPCs[RoomId] != null)
		{
			//Clear out all current NPCs if the room has saved data
			if(Game.Instance.FirstSaved[RoomId])
			{
				if(HasNode("NPCs"))
				{
					foreach(Node npc in GetNode("NPCs").GetChildren())
					{
						npc.QueueFree();
					}
				}
			}
			int amount = Game.Instance.NPCs[RoomId].Count;
			//Add each NPC to the scene
			for(int i = 0; i < amount; i+=8)
			{
				//Get NPC properties
				int type = int.Parse(Game.Instance.NPCs[RoomId][i]);
				float x = float.Parse(Game.Instance.NPCs[RoomId][i+1]);
				float y = float.Parse(Game.Instance.NPCs[RoomId][i+2]);
				int health = int.Parse(Game.Instance.NPCs[RoomId][i+3]);
				string dir = Game.Instance.NPCs[RoomId][i+4];
				string animation = Game.Instance.NPCs[RoomId][i+5];
				int frame = int.Parse(Game.Instance.NPCs[RoomId][i+6]);
				bool IsDying = bool.Parse(Game.Instance.NPCs[RoomId][i+7]);
				//Add the NPC to the scene depending on the type
				PackedScene NPCPackedScene = null;
				if(type == 0)
				{
					NPCPackedScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/NPC1.tscn");
					NPC newNPC = (NPC)NPCPackedScene.Instantiate();
					newNPC._type = type;
					newNPC.Position = new Vector2(x,y);
					newNPC.Health = health;
					newNPC.CurrentDir = dir;
					newNPC.MySpriteAnimation.Animation = animation;
					newNPC.MySpriteAnimation.Frame = frame;
					newNPC.Dying = IsDying;
					newNPC.IsHostile = Game.Instance.RoomsHostile[RoomId];
					newNPC.RoomId = RoomId;
					AddChild(newNPC);
				}
				else if(type == 1)
				{
					NPCPackedScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/NPC2.tscn");
					NPC newNPC = (NPC)NPCPackedScene.Instantiate();
					newNPC._type = type;
					newNPC.Position = new Vector2(x,y);
					newNPC.Health = health;
					newNPC.CurrentDir = dir;
					newNPC.MySpriteAnimation.Animation = animation;
					newNPC.MySpriteAnimation.Frame = frame;
					newNPC.Dying = IsDying;
					newNPC.IsHostile = Game.Instance.RoomsHostile[RoomId];
					newNPC.RoomId = RoomId;
					AddChild(newNPC);
				}
				else if(type == 2)
				{
					NPCPackedScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/NPC3.tscn");
					NPC newNPC = (NPC)NPCPackedScene.Instantiate();
					newNPC._type = type;
					newNPC.Position = new Vector2(x,y);
					newNPC.Health = health;
					newNPC.CurrentDir = dir;
					newNPC.MySpriteAnimation.Animation = animation;
					newNPC.MySpriteAnimation.Frame = frame;
					newNPC.Dying = IsDying;
					newNPC.IsHostile = Game.Instance.RoomsHostile[RoomId];
					newNPC.RoomId = RoomId;
					AddChild(newNPC);
				}
				else if(type == 3)
				{
					NPCPackedScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/Chihuahua.tscn");
					Enemy1 newNPC = (Enemy1)NPCPackedScene.Instantiate();
					newNPC._type = type;
					newNPC.Position = new Vector2(x,y);
					newNPC.Health = health;
					newNPC.CurrentDir = dir;
					newNPC.MySpriteAnimation.Animation = animation;
					newNPC.MySpriteAnimation.Frame = frame;
					newNPC.Dying = IsDying;
					newNPC.IsHostile = Game.Instance.RoomsHostile[RoomId];
					newNPC.RoomId = RoomId;
					AddChild(newNPC);
				}
				else if(type == 4)
				{
					NPCPackedScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/Husky.tscn");
					Enemy2 newNPC = (Enemy2)NPCPackedScene.Instantiate();
					newNPC._type = type;
					newNPC.Position = new Vector2(x,y);
					newNPC.Health = health;
					newNPC.CurrentDir = dir;
					newNPC.MySpriteAnimation.Animation = animation;
					newNPC.MySpriteAnimation.Frame = frame;
					newNPC.Dying = IsDying;
					newNPC.IsHostile = Game.Instance.RoomsHostile[RoomId];
					newNPC.RoomId = RoomId;
					AddChild(newNPC);
				}
				else if(type == 5 || type == 6)
				{ //Two different tanukis, normal and transformed
					NPCPackedScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/Tanuki.tscn");
					Enemy3 newNPC = (Enemy3)NPCPackedScene.Instantiate();
					newNPC._type = type;
					newNPC.Position = new Vector2(x,y);
					newNPC.Health = health;
					newNPC.CurrentDir = dir;
					newNPC.MySpriteAnimation.Animation = animation;
					newNPC.MySpriteAnimation.Frame = frame;
					newNPC.Dying = IsDying;
					newNPC.IsHostile = Game.Instance.RoomsHostile[RoomId];
					newNPC.RoomId = RoomId;
					if(type == 6) { newNPC.Transformed = true; }
					AddChild(newNPC);
				}
			}
		}
		//Load item data
		if(Game.Instance.RoomItems.ContainsKey(RoomId))
		{
			//Clear out all current items if there is saved data
			if(Game.Instance.FirstSaved[RoomId])
			{
				if(HasNode("Items"))
				{
					foreach(Node npc in GetNode("Items").GetChildren())
					{
						npc.QueueFree();
					}
				}
			}
			PackedScene ItemScene = null;
			for(int i = 0; i < Game.Instance.RoomItems[RoomId].Count; i+=3)
			{ //This for loop shouldn't take too long. At most, there's two items in the room
				//Load the item based off of its type
				int ItemType = (int)(Game.Instance.RoomItems[RoomId][i]);
				switch(ItemType)
				{
					case 2: //Knife
						ItemScene = GD.Load<PackedScene>("res://Packed Scenes/Items/Knife.tscn");
						Weapon Knife = (Weapon)ItemScene.Instantiate();
						Knife.Position = new Vector2(Game.Instance.RoomItems[RoomId][i+1], Game.Instance.RoomItems[RoomId][i+2]);
						Knife.Scale = new Vector2(0.5f,0.5f);
						GetNode("Items").AddChild(Knife);
						break;
					case 3: //Shotgun
						ItemScene = GD.Load<PackedScene>("res://Packed Scenes/Items/ShotGun.tscn");
						Weapon Shotgun = (Weapon)ItemScene.Instantiate();
						Shotgun.Position = new Vector2(Game.Instance.RoomItems[RoomId][i+1], Game.Instance.RoomItems[RoomId][i+2]);
						Shotgun.Scale = new Vector2(0.5f,0.5f);
						GetNode("Items").AddChild(Shotgun);
						break;
					case 4: //Green Key
						ItemScene = GD.Load<PackedScene>("res://Packed Scenes/Items/Green Key.tscn");
						Key GreenKey = (Key)ItemScene.Instantiate();
						GreenKey.Position = new Vector2(Game.Instance.RoomItems[RoomId][i+1], Game.Instance.RoomItems[RoomId][i+2]);
						GreenKey.Scale = new Vector2(0.5f,0.5f);
						GetNode("Items").AddChild(GreenKey);
						break;
					case 5: //Red Key
						ItemScene = GD.Load<PackedScene>("res://Packed Scenes/Items/Red Key.tscn");
						Key RedKey = (Key)ItemScene.Instantiate();
						RedKey.Position = new Vector2(Game.Instance.RoomItems[RoomId][i+1], Game.Instance.RoomItems[RoomId][i+2]);
						RedKey.Scale = new Vector2(0.5f,0.5f);
						GetNode("Items").AddChild(RedKey);
						break;
					case 6: //Hamster
						break;
						ItemScene = GD.Load<PackedScene>("res://Packed Scenes/Items/Red Key.tscn");
						Item Hamster = (Item)ItemScene.Instantiate();
						Hamster.Position = new Vector2(Game.Instance.RoomItems[RoomId][i+1], Game.Instance.RoomItems[RoomId][i+2]);
						Hamster.Scale = new Vector2(0.5f,0.5f);
						GetNode("Items").AddChild(Hamster);
				}
			}
		}
		
	}
	
	public override void _Process(double delta)
	{
		// Spawn Paul if he is not here, and in the room.
		// Checks if paulNotSpawned == True && if the rooms data is not null and contains Paul's ID
		if(paulNotSpawned && Game.Instance.NPCs[RoomId] != null && Game.Instance.NPCs[RoomId].FindIndex(z => z == "7") != -1)
		{
			int i = Game.Instance.NPCs[RoomId].FindIndex(z => z == "7");
			//Get NPC properties for Paul
			int type = int.Parse(Game.Instance.NPCs[RoomId][i]);
			float x = float.Parse(Game.Instance.NPCs[RoomId][i+1]);
			float y = float.Parse(Game.Instance.NPCs[RoomId][i+2]);
			int health = int.Parse(Game.Instance.NPCs[RoomId][i+3]);
			string dir = Game.Instance.NPCs[RoomId][i+4];
			string animation = Game.Instance.NPCs[RoomId][i+5];
			int frame = int.Parse(Game.Instance.NPCs[RoomId][i+6]);
			bool IsDying = bool.Parse(Game.Instance.NPCs[RoomId][i+7]);
			
			PackedScene NPCPackedScene = null;
			NPCPackedScene = GD.Load<PackedScene>("uid://bpa0yurigcfsn");
			PatrollingNPC newNPC = (PatrollingNPC)NPCPackedScene.Instantiate();
			newNPC._type = type;
			newNPC.Position = new Vector2(x,y);
			newNPC.Health = health;
			newNPC.CurrentDir = dir;
			newNPC.MySpriteAnimation.Animation = animation;
			newNPC.MySpriteAnimation.Frame = frame;
			newNPC.Dying = IsDying;
			newNPC.IsHostile = Game.Instance.RoomsHostile[RoomId];
			
			if(HasNode("NPCs"))
			{
				GetNode("NPCs").AddChild(newNPC);
			} else {
				GD.PrintErr("NO NPC NODE TO ADD PAUL TO");
			}
			//AddChild(newNPC);
			paulNotSpawned = false;
		}
		
		//Remove Paul if he has left the room
		// Checks if paulNotSpawned == false && if the rooms data is null or does not contain Paul's ID
		if(!paulNotSpawned && (Game.Instance.NPCs[RoomId] == null || Game.Instance.NPCs[RoomId].FindIndex(x => x == "7") == -1))
		{
			GD.Print("Paul should not be in Room: " + RoomId);
			if(HasNode("NPCs/Security"))
			{
				GetNode("NPCs/Security").QueueFree();
			} else {
				GD.PrintErr("DAVID FUCKED UP!");
			}
			paulNotSpawned = true;
		}
	}
}
