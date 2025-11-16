using Godot;
using System;

public partial class NPC : Interactable
{
	[Export]
	public float speed = 100; //Movement speed
	
	[Export]
	public int Health = 100; //Health
	
	public bool IsHostile; //Whether the NPC is hostile
	
	[Export]
	public CompressedTexture2D Portrait; //Dialogue portrait
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	
	//Add protrait to dialogue box
	public override void BeginDialogue()
	{
		base.BeginDialogue();
		TextureRect DialoguePortrait = ((TextureRect)DialogueBox.GetNode("Portrait"));
		DialoguePortrait.Texture = Portrait;
		DialoguePortrait.Visible = true;
	}
	
	//Move around
	public virtual void Move()
	{
		
	}
	
	//Take damage from an attack
	public void TakeDamage(int damage)
	{
		
	}
	
	//Handle being hostile(either by running away or attacking)
	public virtual void HandleHostile()
	{
		
	}
	
	//Remove the NPC from the scene(by dying)
	public override void Remove()
	{
		base.Remove();
	}
}
