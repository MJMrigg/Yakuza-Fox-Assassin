using Godot;
using System;

public partial class QuitMenu : Control
{
	
	public AudioStreamPlayer ButtonSound;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonSound =  (AudioStreamPlayer)(GetTree().GetRoot().GetChild(1).GetNode("ButtonSound"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void openMenu()
	{
		ButtonSound.Play();
		Visible = true;
	}
	
	//Quit to main menu or quit the game if already there
	public void quitGame()
	{
		ButtonSound.Play();
		if(GetTree().CurrentScene.Name != "MainMenu")
		{
			GetTree().ChangeSceneToFile("res://Packed Scenes/User Interface/MainMenu.tscn");
			return;
		}
		GetTree().Quit();
	}
	
	public void closeMenu()
	{
		ButtonSound.Play();
		Visible = false;
	}
	
}
