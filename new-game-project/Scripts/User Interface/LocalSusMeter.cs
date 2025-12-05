using Godot;
using System;

public partial class LocalSusMeter : ProgressBar
{
	public int RoomId;
	
	public bool SuspiciousRoom = true;
	
	[Export]
	public ColorRect thresholdBar;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RoomId = ((Room)GetTree().GetRoot().GetChild(Game.Instance.SceneIndex)).RoomId;
		float MaxSuspicion = Game.Instance.MaxLocalSuspicions[RoomId];
		if(MaxSuspicion == -1) //Make sure this is a room with suspicion
		{ //If it isn't, set the value to 0
			SuspiciousRoom = !Game.Instance.TutorialDone; //Even rooms without suspicion are suspicious during the tutorial
			MaxValue = 10.0f;
			Value = 0.0f;
		}
		else
		{ //If it is, set the value to the suspicion of the room
			MaxValue = MaxSuspicion;
			Value = Game.Instance.LocalSuspicions[RoomId];
		}
		
		UpdateThresholdPosition();
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
		StyleBoxFlat FillStyle = (StyleBoxFlat)(GetThemeStylebox("fill")).Duplicate();
		if(Value >= MaxValue)
		{
			FillStyle.BorderWidthRight = 2;
		}else{
			FillStyle.BorderWidthRight = 0;
		}
		AddThemeStyleboxOverride("fill",FillStyle);
		UpdateThresholdPosition();
	}
	
	private void UpdateThresholdPosition()
	{
		if(thresholdBar == null || !SuspiciousRoom)
			return;
		
		// Calculate threshold percentage (assuming MaxValue is the threshold)
		float thresholdPercent = (Game.Instance.LocalSuspicionThresholds[RoomId] / Game.Instance.MaxLocalSuspicions[RoomId]);
		
		// Position the threshold bar at the threshold point
		float barWidth = Size.X;
		float thresholdX = barWidth * thresholdPercent;
		
		// Position the ColorRect at the threshold position
		// Adjust the X position to align with the threshold
		thresholdBar.Position = new Vector2(thresholdX - thresholdBar.Size.X / 2, 0);
		
		// Make sure it spans the full height
		thresholdBar.Size = new Vector2(thresholdBar.Size.X, Size.Y);
	}
	
	
	
}
