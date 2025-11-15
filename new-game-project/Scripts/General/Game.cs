using Godot;
using System;

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
	public Item[] PlayerInventory = new Item[5]; //Player's inventory
	public Weapon[] PlayerWeapons = new Weapon[2]; //Player's equiped weapons
	public NPC[] NPCs = new NPC[5]; //NPC data
	public Projectile[] Projectiles = new Projectile[5]; //Projectile data
	
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
		
		//Set up player inventory and weapons
		for(int i = 0; i < 5; i++)
		{
			PlayerInventory[i] = null;
		}
		//Equip the bite and pistol
		for(int i = 0; i < 2; i++)
		{
			PlayerWeapons[i] = new Weapon();
			PlayerWeapons[i].ID = i;
		}
		PlayerWeapons[0].Portrait = (CompressedTexture2D)GD.Load("");
		PlayerWeapons[1].Portrait = (CompressedTexture2D)GD.Load("res://Art Assets/Items/gun_Pistol.png");
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
