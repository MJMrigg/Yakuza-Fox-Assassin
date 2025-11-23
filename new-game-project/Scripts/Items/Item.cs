using Godot;
using System;

public partial class Item : Interactable
{
	[Export]
	public int ID; //Unique ID of the item
	
	[Export]
	public CompressedTexture2D Portrait; //Portrait of the item(for when it's in the inventory)
	
	public bool Equipable; //If the item can be equiped
	
	public int InventorySlot = 0; //The slot in the player's inventory the weapon is in
		
	[Signal]
	public delegate void SendToPlayerEventHandler(int ID); //Signal to send the object to the player's invtory
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Begin dialogue
	public override void BeginDialogue()
	{
		base.BeginDialogue();
		//Set second to last dialogue option to pick up the item
		GridContainer DialogueContainer = ((GridContainer)DialogueBox.GetNode("DialogText/DialogOptions"));
		int DialogueCount = DialogueContainer.GetChildCount();
		DialogueOption PickUpOption = (DialogueOption)DialogueContainer.GetChild(DialogueCount-2);
		PickUpOption.Pressed += PickUp;
	}
	
	//Pick up the item and add to the player's inventory
	public void PickUp()
	{
		//Send the object to the player's inventory, end the dialogue and remove it from the scene
		EmitSignal(SignalName.SendToPlayer, ID);
		EndDialogue();
		Remove();
	}
	
	public override async void Remove()
	{
		QueueFree();
	}
}
