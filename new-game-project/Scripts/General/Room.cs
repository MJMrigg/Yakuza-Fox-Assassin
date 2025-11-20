using Godot;
using Godot.Collections;
using System;

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
			var x = Game.Instance.roomMap[RoomId];
			if(x.ContainsKey(Game.Instance.PlayerRoom)){
				var y = x[Game.Instance.PlayerRoom];
				GD.Print("The Player's position in this room should be: " + y);
			} else {
				GD.Print("The players original room does not match. Error or Cheating");
			}
		} else {
			GD.Print("This rooms position is not recorded in roomMap");
		}
		
		// Set Player's Room to this room now
		Game.Instance.PlayerRoom = RoomId;
		
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
