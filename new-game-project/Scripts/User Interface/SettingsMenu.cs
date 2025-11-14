using Godot;
using System;

public partial class SettingsMenu : Control
{
	public string[] KeyBindings; //Array of current keybindings
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change a keybinding
	public void ChangeKeyBinding()
	{
		
	}
	
	//Change the current master volume
	public void ChangeMasterVolume(float NewVolume)
	{
		
	}
	
	
	//Change the current sound effect volume
	public void ChangeSoundVolume(float NewVolume)
	{
		
	}
	
	//Change the current music volume
	public void ChangeMusicVolume(float NewVolume)
	{
		
	}
	
	//Return to the scene
	public void ReturnToScene()
	{
		Visible = false;
	}
}
