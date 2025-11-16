using Godot;
using System;

public partial class WinLoseMenu : ColorRect
{
	
	public string Text; //Text the menu says
	
	public AudioStreamPlayer2D ButtonSound;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonSound = (AudioStreamPlayer2D)GetNode("GeneralSounds/MenuButtonClick");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Restart the game
	public void Restart()
	{
		ButtonSound.Play();
	}
	
	//Return to main menu
	public void Return()
	{
		ButtonSound.Play();
	}
	
	//Go to the credits
	public void Credits()
	{
		ButtonSound.Play();
	}
}
