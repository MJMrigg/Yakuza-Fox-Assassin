using Godot;
using System;

public partial class DialogueOption : Button
{
	public string Dialogue; //Text that the dialogue is
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = Dialogue; //Set the text on the button to the text in the dialogue
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	//???
	public void ReturnOption()
	{
		
	}
}
