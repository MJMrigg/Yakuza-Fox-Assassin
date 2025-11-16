using Godot;
using System;

public partial class SettingsMenu : Control
{
	public string[] KeyBindings; //Array of current keybindings
	
	public float maxVolume = 10.0f;
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//GD.Print(InputMap.GetActions());
		//InputMap.ActionAddEvent("p_melee", );

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change a keybinding
	public void ChangeKeyBinding()
	{
		
	}
	
	//
	public void buttonToggle(bool tog)
	{
		
	}
	
	//Change the current master volume
	public void ChangeMasterVolume(float NewVolume)
	{
		if (NewVolume > maxVolume){
			NewVolume = maxVolume;
		}
		//audiobus 0 is master bus
		AudioServer.SetBusVolumeDb(0, NewVolume);
	}
	
	
	//Change the current sound effect volume
	public void ChangeSoundVolume(float NewVolume)
	{
		if (NewVolume > maxVolume){
			NewVolume = maxVolume;
		}
		//audiobus 1 is the sound effect bus
		AudioServer.SetBusVolumeDb(1, NewVolume);
	}
	
	//Change the current music volume
	public void ChangeMusicVolume(float NewVolume)
	{
		if (NewVolume > maxVolume){
			NewVolume = maxVolume;
		}
		//audiobus 2 is the music bus
		AudioServer.SetBusVolumeDb(2, NewVolume);
	}
	
	//Return to the scene
	public void ReturnToScene()
	{
		Visible = false;
	}
}
