using Godot;
using System;

public partial class WinLoseMenu : Panel
{
	
	public string Text; //Text the menu says
	
	public AudioStreamPlayer ButtonSound;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonSound = (AudioStreamPlayer)GetTree().GetRoot().GetChild(1).GetNode("ButtonSound");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Restart the game
	public void Restart()
	{
		ButtonSound.Play();
		//Reset game properties
		Game.Instance.StartGame();
		Game.Instance.GameStart = true;
		//Change scene and music to first room
		((MusicPlayer)GetTree().GetRoot().GetChild(1)).ChangeSong(0);
		Game.Instance.TutorialDone = false;
		GetTree().ChangeSceneToFile("uid://br5o8skjv87ol");
	}
	
	//Return to main menu
	public void Return()
	{
		ButtonSound.Play();
		Game.Instance.GameStart = false;
		((MusicPlayer)GetTree().GetRoot().GetChild(1)).ChangeSong(0);
		GetTree().ChangeSceneToFile("res://Packed Scenes/User Interface/MainMenu.tscn");
	}
	
	//Go to the credits
	public void Credits()
	{
		ButtonSound.Play();
		GetTree().ChangeSceneToFile("res://Packed Scenes/User Interface/Credits.tscn");
	}
}
