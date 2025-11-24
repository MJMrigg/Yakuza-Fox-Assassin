using Godot;
using System;

public partial class MainMenu : Node
{
	public bool GameStart; //Whether the game has started
	
	[Export]
	public Control quitMenu;
	
	[Export]
	public ColorRect difMenu; //This bitch won't appear for some reason.
	
	[Export]
	public Control setMenu;
	
	[Export]
	public ColorRect playMenu;
	
	[Export]
	public AudioStreamPlayer buttonSound;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GameStart = false;
		buttonSound = ((AudioStreamPlayer)GetTree().GetRoot().GetChild(1).GetNode("ButtonSound"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Check for visibility
		if(quitMenu.Visible == false && setMenu.Visible == false && difMenu.Visible == false)
		{
			playMenu.Visible = true;
		}
	}
	
	//Open the settings menu
	public void GoSettingsMenu()
	{
		if (setMenu != null)
		{
			buttonSound.Play();
			setMenu.Visible = true; 
			playMenu.Visible = false;
		} else {
			GD.Print("Setting Menu has not been assigned");
		}
	}
	
	//Start the game by getting the difficulty menu
	public void GoDifficultyMenu()
	{
		if (difMenu != null)
		{
			buttonSound.Play();
			difMenu.Visible = true; 
			playMenu.Visible = false;
		} else {
			GD.Print("Difficulty Menu has not been assigned");
		}
	}
	
	//Open the quit menu
	public void GoQuitMenu()
	{
		if (quitMenu != null)
		{
			quitMenu.Visible = true; 
			playMenu.Visible = false;
			buttonSound.Play();
		} else {
			GD.Print("Quit Menu has not been assigned");
		}
	}
}
