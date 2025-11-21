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
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change a keybinding
	public void ChangeKeyBinding(InputEvent @event, string action) {
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
		}
		SetProcessInput(false);
		var events = InputMap.ActionGetEvents(action);
		if(curButton != null)
		{
			curButton.Text = events[0].AsText();
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
	
	// Restores default controls scheme
	public void restoreDefault()
	{
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
	
}
