using Godot;
using System;

public partial class DialogueBox : HBoxContainer
{
	public DialogueOption[] DialogueOptions; //Array of options the player can select
	
	public string Text; //Text current being displayed on the screen
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Change the current text
	public void ChangeDialogue()
	{
		
	}
	
	//Display all of the dialogue options
	public void DisplayOptions()
	{
		
	}
	
	//Change to another dialogue option
	public void ChangeOptions()
	{
		
	}
}
