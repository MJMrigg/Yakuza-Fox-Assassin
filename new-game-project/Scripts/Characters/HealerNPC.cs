using Godot;
using System;

public partial class HealerNPC : NPC
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Speed = 0.0f; //Healer NPCs do not move
		base._Ready();
		CurrentDir = "D";
		MySpriteAnimation.Animation="Walk_D";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//If the NPC is dying, do nothing
		if(Dying)
		{
			return;
		}
		
		//base._Process(delta);
	}
	
	//Heal the player half of their max health
	public void Heal()
	{
		int CurrentHealth = Game.Instance.PlayerHealth;
		int MaxHealth = Game.Instance.MaxPlayerHealth;
		Game.Instance.PlayerHealth = Math.Min(MaxHealth, CurrentHealth+(MaxHealth/2));
		((AudioStreamPlayer)GetNode("Sounds/Drinking")).Play();
		Game.Instance.CurrentDrinks += 1; //Increase the amount of drinks they've had
	}
	
	//Begin dialogue
	public override void BeginDialogue()
	{
		base.BeginDialogue();
		//Set second to last dialogue option to heal the player
		GridContainer DialogueContainer = ((GridContainer)DialogueBox.GetNode("DialogText/DialogOptions"));
		int DialogueCount = DialogueContainer.GetChildCount();
		DialogueOption PickUpOption = (DialogueOption)DialogueContainer.GetChild(DialogueCount-2);
		PickUpOption.Pressed += Heal;
	}
}
