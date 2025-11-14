using Godot;
using System;

public partial class MainMenu : Node
{
	public bool GameStart; //Whether the game has started
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameStart = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Open the settings menu
	GoSettingsMenu()
	{
		
	}
	
	//Start the game by getting the difficulty menu
	GoDifficultyMenu()
	{
		
	}
}
