using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

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
			for(int i = 0; i < amount; i++)
			{
				GD.Print(Game.Instance.NPCs[RoomId][i]);
				/*//Get NPC properties
				Vector2 NewPosition = Game.Instance.NPCs[RoomId][i].Position;
				int NewHealth = Game.Instance.NPCs[RoomId][i].Health;
				string NewDir = Game.Instance.NPCs[RoomId][i].CurrentDir;
				bool NewDying = Game.Instance.NPCs[RoomId][i].Dying;
				PackedScene NPCScene = null; 
				//Set NPC type
				if(Game.Instance.NPCs[RoomId][i].IsClass("NPC1"))
				{
					NPCScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/NPC1.tscn");
					NPC NewNPC1 = (NPC)NPCScene.Instantiate();
					GetNode("NPCs").AddChild(NewNPC1);
				}
				else if(Game.Instance.NPCs[RoomId][i].IsClass("NPC2"))
				{
					NPCScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/NPC2.tscn");
					NPC NewNPC2 = (NPC)NPCScene.Instantiate();
					GetNode("NPCs").AddChild(NewNPC2);
				}
				else if(Game.Instance.NPCs[RoomId][i].IsClass("NPC3"))
				{
					NPCScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/NPC3.tscn");
					NPC NewNPC3 = (NPC)NPCScene.Instantiate();
					GetNode("NPCs").AddChild(NewNPC3);
				}
				else if(Game.Instance.NPCs[RoomId][i].IsClass("Enemy1"))
				{
					NPCScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/Chihuahua.tscn");
					Enemy1 NewChihuahua = (Enemy1)NPCScene.Instantiate();
					GetNode("NPCs").AddChild(NewChihuahua);
				}
				else if(Game.Instance.NPCs[RoomId][i].IsClass("Enemy2"))
				{
					NPCScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/Husky.tscn");
					Enemy2 NewHusky = (Enemy2)NPCScene.Instantiate();
					GetNode("NPCs").AddChild(NewHusky);
				}
				else if(Game.Instance.NPCs[RoomId][i].IsClass("Enemy3"))
				{
					NPCScene = GD.Load<PackedScene>("res://Packed Scenes/Characters/Characters/Tanuki.tscn");
					Enemy3 NewTanuki = (Enemy3)NPCScene.Instantiate();
					GetNode("NPCs").AddChild(NewTanuki);
					//newNPC.Transformed = ((Enemy3)Game.Instance.NPCs[RoomId][i]).Transformed;
				}*/
			}
		}
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
