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
		var actions = InputMap.GetActions();
		foreach (var i in actions){
			if(i == actionType){
				var events = InputMap.ActionGetEvents(i);
				defText = events[0].AsText();
				Text = events[0].AsText();
			}
		}
		//defText = Text;
	}
	
	public void buttonToggle(bool tog)
	{
		EmitSignal(SignalName.butID, this, tog, defText, actionType);
	}
}
