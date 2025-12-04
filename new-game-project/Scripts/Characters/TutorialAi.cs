using Godot;
using System;

public partial class TutorialAi : Interactable
{
	public string[] TutorialText;
	
	public int TutorialPart = 0;
	
	[Export]
	public CompressedTexture2D Portrait; //Dialogue portrait
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		CurrentDir = "D";
		MySpriteAnimation.Animation="Walk_D";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Reset sus
	public void Reset()
	{
		Game.Instance.GlobalSuspicion = 0;
		Game.Instance.LocalSuspicions[0] = 0;
		GetTree().CallGroup("NPCs","MakeNotHostile");
		//Game.Instance.RoomsHostile[RoomId] = false;
	}
	
	
	public override void BeginDialogue()
	{
		base.BeginDialogue();	
		TextureRect DialoguePortrait = ((TextureRect)DialogueBox.GetNode("Portrait"));
		DialoguePortrait.Texture = Portrait;
		DialoguePortrait.Visible = true;
		
		//Set last dialogue option to clear sus
		GridContainer DialogueContainer = ((GridContainer)DialogueBox.GetNode("DialogText/DialogOptions"));
		int DialogueCount = DialogueContainer.GetChildCount();
		DialogueOption PickUpOption = (DialogueOption)DialogueContainer.GetChild(DialogueCount-2);
		PickUpOption.Pressed += Reset;
		
	}
	
	public override void EndDialogue()
	{
		base.EndDialogue();
		//QueueFree();
	}
}
