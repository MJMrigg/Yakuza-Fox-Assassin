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
		Game.Instance.FinishTutorial();
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
	
	public override void CheckPlayer(Node2D body)
	{
		
	}
}
