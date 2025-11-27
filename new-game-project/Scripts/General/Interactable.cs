using Godot;
using System;

public partial class Interactable : Entity
{
	[Export]
	public string InitialDialog; //First Dialog option ever said
	
	[Export]
	public string[] DialogueOptions; //Dialog options that the player can select
	
	[Export]
	public string[] DialogResponses; //Responses to each option
	
	[Export]
	public float[] DialogSuspicion; //How much each option increases suspicion
	
	public HBoxContainer DialogueBox; //Dialogue box on the screen
	
	[Export]
	public Sprite2D InteractionBox; //Box that will pop up to remind the player to interact with it
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//Get the dialog box on the screen
		DialogueBox = ((HBoxContainer)GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).GetNode("MainUI/Dialog"));
		//Hide interaction box
		InteractionBox.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	
	//Send dialogue options to the dialogue box
	public virtual void BeginDialogue()
	{
		//Pause all entities
		GetTree().CallGroup("Pausable","Pause");
		//Make sure there is dialog
		if(DialogueOptions.Length == 0){
			//Not an error. The player may have just gone through all of the dialog options
			return;
		}
		if(InitialDialog == "" || InitialDialog == null)
		{
			GD.Print("Error, no initial dialog");
			return;
		}
		//Reveal the dialogue box
		DialogueBox.Visible = true;
		//But hide the portrait(NPC children will handle the portrait)
		((TextureRect)DialogueBox.GetNode("Portrait")).Visible = false;
		//Place the initial dialogue on the screen
		((Label)DialogueBox.GetNode("DialogText/Text")).Text = InitialDialog;
		GridContainer DialogueContainer = ((GridContainer)DialogueBox.GetNode("DialogText/DialogOptions"));
		//Get rid of all dialogue options that may still be in the dialog box
		for(int i = DialogueContainer.GetChildCount()-1; i >= 0; i--)
		{
			DialogueContainer.GetChild(i).QueueFree();
		}
		//Place all Dialog options in the dialog box
		for(int i = 0; i < DialogueOptions.Length; i++)
		{
			PackedScene DialogueOptionScene = GD.Load<PackedScene>("res://Packed Scenes/User Interface/DialogOption.tscn");
			DialogueOption NewOption = ((DialogueOption)DialogueOptionScene.Instantiate());
			NewOption.Dialogue = DialogueOptions[i];
			DialogueContainer.AddChild(NewOption);
			//Add dialog functionality
			if(i == DialogueOptions.Length-1)
			{
				//If it's a normal option, connect it to its response
				NewOption.Pressed += EndDialogue;
			}
			else
			{
				//If it's the final option, connect it to ending the dialogue
				NewOption.ChooseOption += SendDialogResponse;
			}
		}
	}
	
	//End the dialogue
	public void EndDialogue()
	{
		//Unpause all entities
		GetTree().CallGroup("Pausable","Pause");
		//Tell the player that they are no longer in dialogue by going through all pausable nodes
		var Nodes = GetTree().GetNodesInGroup("Pausable");
		foreach(Node body in Nodes)
		{
			if(body is Player)
			{
				Player player = (Player)body;
				player.InDialogue = false;
				break;
			}
		} 
		DialogueBox.Visible = false;
	}
	
	//Send the dialogue response to the dialogue box
	public void SendDialogResponse(int ChosenOption)
	{
		//Update the dialogue box
		((Label)DialogueBox.GetNode("DialogText/Text")).Text = DialogResponses[ChosenOption];
		//Make the dialogue increase local suspision
		Game.Instance.IncreaseLocalSuspicion(RoomId, DialogSuspicion[ChosenOption]);
	}
}
