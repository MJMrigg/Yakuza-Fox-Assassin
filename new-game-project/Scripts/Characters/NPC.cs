using Godot;
using System;

public partial class NPC : Interactable
{
	public float speed; //Movement speed
	
	public int Health; //Health
	
	public bool IsHostile; //Whether the NPC is hostile
	
	[Export]
	public CompressedTexture2D Portrait; //Dialogue portrait
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Move around
	public virtual void Move()
	{
		
	}
	
	//Take damage from an attack
	public void TakeDamage()
	{
		
	}
	
	//Handle being hostile(either by running away or attacking)
	public virtual void HandleHostile()
	{
		
	}
	
	//Remove the NPC from the scene(by dying)
	public override void Remove()
	{
		
	}
}
