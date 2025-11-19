using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node
{
	public float GlobalSuspicion = 0;
	
	public float MaxGlobalSuspicion = 0;
	
	public float[] LocalSuspicions = new float[20]; //Local Suspcisions of every room
	
	public float[] MaxLocalSuspicions = new float[20]; //Maximum Local Supsicions
	
	public float[] LocalSuspicionThresholds = new float[20]; //Local Suspicion thresholds of each room
	
	public bool[] RoomsHostile = new bool[20]; //Whether each room is hostile
	
	public float MasterVolume; //Master volume of the whole game
	
	public float SoundVolume; //Volume of all sound effects
	
	public float MusicVolume; //Volume of all music
	
	//Whether each step in a puzzle in complete
	public bool[] SecurityPuzzle = new bool[5];
	public bool[] MedicPuzzle = new bool[5];
	public bool[] StoragePuzzle = new bool[5];
	public bool[] ControlPuzzle = new bool[5];
	
	//Rooms of player and patrolling NPC
	public int PlayerRoom = 0;
	public int PatrolRoom = 18;
	
	//Save/load data
	public Item[] PlayerInventory = new Item[6]; //Player's inventory
	public Weapon[] PlayerWeapons = new Weapon[2]; //Player's equiped weapons
	public NPC[] NPCs = new NPC[5]; //NPC data
	public Projectile[] Projectiles = new Projectile[5]; //Projectile data
	public Weapon Bite = new Weapon(); //Bite data
	public int[] ItemData = {
		//ID, damage, cooldown
		0,7,1, //Bite
		1,7,1, //Pistol
		2,10,2, //Knife
		3,10,2, //Shotgun
		//ID,color
		4,1, //Green Key
		5,0, //Red Key
		//ID
		6 //Hamster
	};
	
	//Test Info for room transition
	//<key, value>
	//<this room id, Dict<goes to this room id,Vector2 of Player Position >>
	public Dictionary<int, Dictionary<int, Vector2>> roomMap;
	
	//Global instance of the game
	public static Game Instance { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Create a global instance of the Game
		Instance = this;
		//Set up local suspicions and thresholds
		for(int i = 0; i < 20; i++)
		{
			LocalSuspicions[i] = 0;
			RoomsHostile[i] = false;
		}
		MaxLocalSuspicions = 
		[
			//First Half
			-1,10,10,5,10,-1,5,15,10,10,
			//Second Half
			13,5,5,-1,10,7,10,5,15,15,-1
		];
		LocalSuspicionThresholds = 
		[
			//First Half
			-1,5,4,1,5,-1,5,3.5f,7,4,
			//Second Half
			4,1,1,-1,5,1,4,1,3.5f,3,-1
		];
		
		//Calculate Max Global Suspcition
		for(int i = 0; i < 20; i++)
		{
			//Ignore rooms without suspicion
			if(MaxLocalSuspicions[i] == -1)
			{
				continue;
			}
			MaxGlobalSuspicion += MaxLocalSuspicions[i];
		}
		
		//Set up player inventory
		for(int i = 0; i < 5; i++)
		{
			PlayerInventory[i] = null;
		}
		//Set up player bite
		Bite.ID = 0;
		Bite.Damage = 10;
		Bite.CoolDown = 2;
		Bite.Portrait = (CompressedTexture2D)GD.Load("");
		PlayerWeapons[0] = Bite;
		//Set up player pistol
		PlayerWeapons[1] = new Weapon();
		PlayerWeapons[1].ID = ItemData[3];
		PlayerWeapons[1].Damage = ItemData[4];
		PlayerWeapons[1].CoolDown = ItemData[5];
		//Place it in their inventory
		PlayerInventory[0] = new Item();
		PlayerInventory[0].ID = ItemData[3];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//Decay all local suspicions and global suspicion
	public void DecaySuspicion()
	{
		
	}
	
	//Increase the local suspicion of a room
	public void IncreaseLocalSuspicion(int Room, float Amount)
	{
		//Don't increase Local Suspisions of rooms that don't have local suspicions
		if(MaxLocalSuspicions[Room] == -1)
		{
			return;
		}
		LocalSuspicions[Room] += Amount;
		//Make sure the local suspicion doesn't go over the max
		LocalSuspicions[Room] = Mathf.Min(LocalSuspicions[Room],MaxLocalSuspicions[Room]);
	}
	
	//Handle the player losing
	public void PlayerLost()
	{
		
	}
	
	//Handle the player winning
	public void PlayerWon()
	{
	}
}
