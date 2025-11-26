using Godot;
using System;
using System.Collections.Generic;

public partial class SettingsMenu : Control
{
	public string[] KeyBindings; //Array of current keybindings
	public float maxVolume = 10.0f;
	
	public string curAction;
	public string curText;
	public Button curButton;
	
	public AudioStreamPlayer ButtonSound;
	
	[Export]
	public HSlider MasterVolume;
	[Export]
	public HSlider MusicVolume;
	[Export]
	public HSlider SoundVolume;
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MasterVolume.Value = Game.Instance.MasterVolume;
		MusicVolume.Value = Game.Instance.MusicVolume;
		SoundVolume.Value = Game.Instance.SoundVolume;
		ButtonSound = ((AudioStreamPlayer)GetTree().GetRoot().GetChild(1).GetNode("ButtonSound"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change a keybinding
	public void ChangeKeyBinding(InputEvent @event, string action) {
		//ButtonSound.Play();
		// CHECK WHAT TYPE OF EVENT
		InputEvent newEvent = null;
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			var newKeyEvent = new InputEventKey();
			newKeyEvent.PhysicalKeycode = keyEvent.PhysicalKeycode;
			newEvent = newKeyEvent;
		} 
		else if (@event is InputEventMouseButton mouseEvent)
		{
			var newMouseEvent = new InputEventMouseButton();
			newMouseEvent.ButtonIndex = mouseEvent.ButtonIndex;
			newEvent = newMouseEvent;
		}
		
		if (newEvent == null)
		{
			return;
		}
		
		// Check if duplicate key
		bool duplicate = false;
		foreach (Button i in GetTree().GetNodesInGroup("keyBinds"))
		{
			if (i != curButton && i.Text == newEvent.AsText())
			{
				//Duplicate Input. Can not allow.
				duplicate = true;
				break;
			}
		}
		if(!duplicate)
		{
			// We good boss
			InputMap.ActionEraseEvents(action);
			InputMap.ActionAddEvent(action, newEvent);
			ButtonSound.Play();
		}
		SetProcessInput(false);
		var events = InputMap.ActionGetEvents(action);
		if(curButton != null)
		{
			curButton.Text = events[0].AsText();
			ButtonSound.Play();
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			ChangeKeyBinding(@event, curAction);
		} else if (@event is InputEventMouseButton mouseEvent)
		{
			ChangeKeyBinding(@event, curAction);
		}
		if(curButton != null)
		{
			curButton.ButtonPressed = false;
		}
	}
	
	public void buttonsTog(Button thisBut, bool tog, string defaultText, string action)
	{
		if(tog){
			ButtonSound.Play();
			thisBut.Text = "Press A Key";
			curAction = action;
			curButton = thisBut;
			SetProcessInput(tog);
			foreach (Button i in GetTree().GetNodesInGroup("keyBinds")){
				if (i != thisBut){
					i.ToggleMode = false;
					i.SetProcessInput(false);
				}
			}
		} else {
			foreach (Button i in GetTree().GetNodesInGroup("keyBinds")){
				if (i != thisBut){
					i.ToggleMode = true;
					i.SetProcessInput(false);
				}
			}
		}
		
	}
	
	//Change the current master volume
	public void ChangeMasterVolume(float NewVolume)
	{
		if (NewVolume >= maxVolume){
			return;
		}
		//audiobus 0 is master bus
		AudioServer.SetBusVolumeDb(0, NewVolume);
		Game.Instance.MasterVolume = NewVolume;
	}
	
	
	//Change the current sound effect volume
	public void ChangeSoundVolume(float NewVolume)
	{
		if (NewVolume >= maxVolume){
			return;
		}
		//audiobus 1 is the sound effect bus
		AudioServer.SetBusVolumeDb(1, NewVolume);
		Game.Instance.SoundVolume = NewVolume;
	}
	
	//Change the current music volume
	public void ChangeMusicVolume(float NewVolume)
	{
		if (NewVolume >= maxVolume){
			return;
		}
		//audiobus 2 is the music bus
		AudioServer.SetBusVolumeDb(2, NewVolume);
		Game.Instance.MusicVolume = NewVolume;
	}
	
	//Return to the scene
	public void ReturnToScene()
	{
		ButtonSound.Play();
		Visible = false;
	}
	
	// Restores default controls scheme
	public void restoreDefault()
	{
		ButtonSound.Play();
		var actions = InputMap.GetActions();
		foreach (var i in actions)
		{
			string actionName = i.ToString();
			if (!actionName.StartsWith("ui_") && Game.Instance.defControls.ContainsKey(i))
			{
				
				foreach (Button j in GetTree().GetNodesInGroup("keyBinds"))
				{
					if (InputMap.ActionGetEvents(actionName)[0].AsText() == j.Text)
					{
						//This button's text needs to change
						j.Text = Game.Instance.defControls[i][0].AsText();
						break;
					}
				}
				InputMap.ActionEraseEvents(i);
				var x = Game.Instance.defControls[i];
				foreach (var events in x)
				{
					InputMap.ActionAddEvent(i, events);
				}
			}
		}
	}
	
	// Test Function
	public void printInput()
	{
		var actions = InputMap.GetActions();
		foreach (var i in actions)
		{
			string actionName = i.ToString();
			if (!actionName.StartsWith("ui_"))
			{
				GD.Print($"Action: {actionName}");
				
				var events = InputMap.ActionGetEvents(actionName);
				foreach (var ev in events)
				{
					GD.Print($"  - {ev.AsText()}");
				}
				
				GD.Print("");
			}
		}
	}
	
	//Open the settings menu
	public void OpenSettings()
	{
		((AudioStreamPlayer)GetTree().GetRoot().GetChild(1).GetNode("ButtonSound")).Play();
		Visible = true;
	}
	
}
