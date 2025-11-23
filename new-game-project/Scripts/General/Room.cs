using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Globalization;

public partial class Room : Node2D
{
	[Export]
	public int RoomId; //Identifier of the room
		
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
		if(Game.Instance.NPCs[RoomId].Count > 0)
		{
			//Clear out all current NPCs
			foreach(Node npc in GetNode("NPCs").GetChildren())
			{
				npc.QueueFree();
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
					if(type == 6) { newNPC.Transformed = true; }
					AddChild(newNPC);
				}
				
			}
		}
	}
	
	public override void _Process(double delta)
	{
	}
	
	//Kept as refrence for now.
	// DO NOT USE FOR SCENE TRANSITIONS
	// Please use doors instead
	public void moveSceneTest(Node2D thing)
	{
		//Note to self - DT
		/*
		This function will have an Area2D connected to it. If it detects the player then this function is called.
		When called this function will need to determine the following.
		1. What room am I sending the player to. 
		*/
		Node2D Player = (Node2D)GetNode("Player");
		GD.Print("Detected Thing: " + thing);
		GD.Print("Player: " + Player);
		if (thing == Player)
		{
			GD.Print("Player Detected! Moving to new room");
			//Get this area2D's ID to figure out what scene to load
			
			
			GetTree().ChangeSceneToFile("uid://cpommox3imbij");
			//GetTree().ChangeSceneToFile("res://Packed Scenes/Rooms/FirstHalf/Engine.tscn");
		}
	}
	
}
