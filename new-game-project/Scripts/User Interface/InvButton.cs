using Godot;
using System;

public partial class InvButton : Button
{
	public Player player; //The player
	
	public AudioStreamPlayer ButtonSound; //Sound the button will play
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		player = ((Player)GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).GetNode("Player"));
		ButtonSound = ((AudioStreamPlayer)GetTree().GetRoot().GetChild(1).GetNode("ButtonSound"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		var actions = InputMap.ActionGetEvents("p_inv");
		Text = "Inventory (" + actions[0].AsText().Split(" (")[0] + ")";
	}
	
	public void OpenInventory()
	{
		ButtonSound.Play();
		player.OpenInv();
	}
}
