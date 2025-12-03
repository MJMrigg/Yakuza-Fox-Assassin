using Godot;
using System;

public partial class DifficultyMenu : Panel
{
	public float Difficulty; //Chosen difficulty
	
	[Export]
	public AudioStreamPlayer buttonSound;
	
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
		//On call change to appropriate scene and start game
		Game.Instance.StartGame();
		Game.Instance.MaxGlobalSuspicion *= Difficulty;
		Game.Instance.GameStart = true;
		Game.Instance.TutorialDone = false; 
		GetTree().ChangeSceneToFile("uid://br5o8skjv87ol");
	}
	
	//Set the difficulty
	public void ChangeDifficulty()
	{
		foreach(Button i in GetTree().GetNodesInGroup("difButtons"))
		{
			if(i.IsPressed()){
				((AudioStreamPlayer)GetTree().GetRoot().GetChild(1).GetNode("ButtonSound")).Play();
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
