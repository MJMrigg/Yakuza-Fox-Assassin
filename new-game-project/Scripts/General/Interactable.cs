using Godot;
using System;

public partial class Interactable : CharacterBody2D
{
	[Export]
	public string[] DialogueOptions; //Dialog options that the player can select
	
	[Export]
	public string[] DialogResponses; //Responses to each option
	
	[Export]
	public int[] DialogSuspicion; //How much each option increases suspicion
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Send dialogue options to the dialogue box
	public void BeginDialogue()
	{
		
	}
	
	//End the dialogue
	public void EndDialogue()
	{
		
	}
	
	//Send the dialogue response to the dialogue box
	public void SendDialogResponse()
	{
		
	}
	
	//Remove the interactable from the scene(varies)
	public virtual void Remove()
	{
		
	}
}
