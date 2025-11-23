using Godot;
using System;

public partial class MusicPlayer : AudioStreamPlayer
{
	//UIDs for the different song audio files
	public string BarSongUID = "uid://fym3tpyx4jtg";
	public string BossSongUID = "uid://c6ovg3y2qdmhp";
	public string Song1UID = "uid://cwtnej12rivfh";
	public string Song2UID = "uid://bmgs2cf2ro1uh";
	
	public string ChosenSong = ""; //Chosen and song that is playing
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//If the game hasn't started yet, don't play any music
		if(!Game.Instance.GameStart)
		{
			return;
		}
		//Play a song based on the player's room if there is no song playing
		if(!Playing)
		{
			ChangeSong(Game.Instance.PlayerRoom);
		}
	}
	
	//Change the song based on the player's room
	public void ChangeSong(int PlayerRoom)
	{
		//Stop playing the current song
		if(Playing)
		{
			Stop();
		}
		//Change the song based on the player's room
		if(PlayerRoom == 20) //If the player is in the boss room
		{
			ChosenSong = BossSongUID;
		}
		else if(PlayerRoom == 5 || PlayerRoom == 13) //If the player is in the bar
		{
			ChosenSong = BarSongUID;
		}
		else //If the player is anywhere else
		{
			//Pick the other song that plays throughout the ship
			if(ChosenSong == Song2UID)
			{
				ChosenSong = Song1UID;
			}
			else 
			{
				ChosenSong = Song2UID;
			}
		}
		//Play the chosen song
		Stream = ResourceLoader.Load<AudioStream>(ChosenSong);
		Play();
	}
}
