using Godot;
using System;

public partial class buttonSendId : Button
{
	public string defText;
	
	[Export]
	public string actionType;
	
	[Signal]
	public delegate void butIDEventHandler(Button thisBut, bool tog, string defaultText, string action);
	
	public override void _Ready()
	{
		var action = InputMap.ActionGetEvents(actionType);
		defText = action[0].AsText();
		Text = action[0].AsText();
	}
	
	public void buttonToggle(bool tog)
	{
		EmitSignal(SignalName.butID, this, tog, defText, actionType);
	}
}
