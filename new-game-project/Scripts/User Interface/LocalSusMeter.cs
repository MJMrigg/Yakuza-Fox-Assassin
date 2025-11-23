using Godot;
using System;

public partial class LocalSusMeter : ProgressBar
{
	public int RoomId;
	
	public bool SuspiciousRoom = true;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RoomId = ((Room)GetTree().GetRoot().GetChild(1)).RoomId;
		float MaxSuspicion = Game.Instance.MaxLocalSuspicions[RoomId];
		if(MaxSuspicion == -1) //Make sure this is a room with suspicion
		{
			SuspiciousRoom = false;
			return;
		}
		MaxValue = MaxSuspicion;
		Value = Game.Instance.LocalSuspicions[RoomId];
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
	}
}
