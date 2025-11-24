using Godot;
using System;

public partial class YesNoMenu : Control
{
	[Signal]
	public delegate void ChoiceEventHandler(bool choice);
	
	AudioStreamPlayer ButtonSound;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonSound = (AudioStreamPlayer)GetTree().GetRoot().GetChild(1).GetNode("ButtonSound");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void Yes()
	{
		ButtonSound.Play();
		EmitSignal(SignalName.Choice,true);
	}
	
	public void No()
	{
		ButtonSound.Play();
		EmitSignal(SignalName.Choice,false);
	}
}
