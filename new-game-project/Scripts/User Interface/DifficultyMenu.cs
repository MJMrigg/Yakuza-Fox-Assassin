using Godot;
using System;

public partial class DifficultyMenu : ColorRect
{
	public float Difficulty; //Chosen difficulty
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Start the game
	public void StartGame()
	{
		//On call change to appropriate scene
		GetTree().ChangeSceneToFile(Game.Instance.roomIDS[0]);
	}
	
	//Set the difficulty
	public void ChangeDifficulty()
	{
		foreach(Button i in GetTree().GetNodesInGroup("difButtons"))
		{
			if(i.IsPressed()){
				switch(i.Text.ToLower().Trim())
				{
					case "easy":
						Difficulty = 0.9f;
						StartGame();
						break;
					case "medium":
						Difficulty = 0.8f;
						StartGame();
						break;
					case "hard":
						Difficulty = 0.7f;
						StartGame();
						break;
					default:
						// Somehow broke the game. So **** you, you get the hardest difficulty.
						Difficulty = 0.1f; 
						StartGame();
						break;
				}
			}
		}
		//DEBGUG. Remove later - DT
		GD.Print("Current Dif: " + Difficulty);
	}
}
