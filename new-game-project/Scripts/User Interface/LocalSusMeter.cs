using Godot;
using System;

public partial class LocalSusMeter : ProgressBar
{
	public int RoomId;
	
	public bool SuspiciousRoom = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RoomId = ((Room)GetTree().GetRoot().GetChild(Game.Instance.SceneIndex)).RoomId;
		float MaxSuspicion = Game.Instance.MaxLocalSuspicions[RoomId];
		if(MaxSuspicion == -1) //Make sure this is a room with suspicion
		{ //If it isn't, set the value to 0
			SuspiciousRoom = false;
			MaxValue = 100.0f;
			Value = 0.0f;
		}
		else
		{ //If it is, set the value to the suspicion of the room
			MaxValue = MaxSuspicion;
			Value = Game.Instance.LocalSuspicions[RoomId];
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!SuspiciousRoom) //If this room doesn't have suspicion, do nothing
		{
			return;
		}
		//Update local suspicion in real time
		Value = Game.Instance.LocalSuspicions[RoomId];
		//Make the room hostile if it's meant to be hostile
		if(Value >= MaxValue && !(Game.Instance.RoomsHostile[RoomId]))
		{
			Game.Instance.RoomsHostile[RoomId] = true;
			GetTree().CallGroup("NPCs","MakeHostile");
		}
		//Update border width based on if the bar is filled or not
		//Ensures the fill right border color doesn't show up when it's not full
		if(Value >= MaxValue)
		{
			((StyleBoxFlat)GetThemeStylebox("fill")).BorderWidthRight = 2;
		}else{
			((StyleBoxFlat)GetThemeStylebox("fill")).BorderWidthRight = 0;
		}
	}
}
