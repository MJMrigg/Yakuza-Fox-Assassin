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
	public int PlayerRoom = 8;
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
	//<current room id, Dict<goes to this room id,Vector2 of Player Position in current room >>
	public Dictionary<int, Dictionary<int, Vector2>> roomMap = new Dictionary<int, Dictionary<int, Vector2>>
		{
			//<Docks(1), [Market(1)]>
			{0, new Dictionary<int, Vector2> { { 1, new Vector2(825, 244) } } },
			// <Market(1), [Docks(0), Living1(2), Cafeteria(4), Storage(6), SecurityEngine(7), Production(9), TrainingYard(10)]>, Still needs to be mapped
			{1, new Dictionary<int, Vector2> { { 0, new Vector2(0, 0) }, { 2, new Vector2(0, 0) }, { 4, new Vector2(0, 0) }, { 6, new Vector2(0, 0) }, { 7, new Vector2(0, 0) }, { 9, new Vector2(0, 0) }, { 10, new Vector2(0, 0) } } },
			//<Living1(2), [Market(1), Bathroom1(3)]>, Still needs to be mapped
			{2, new Dictionary<int, Vector2> { { 1, new Vector2(0, 0) }, { 3, new Vector2(0, 0) } } },
			//<Bathroom1(3), [Living1(2)]>, Still needs to be mapped
			{3, new Dictionary<int, Vector2> { { 1, new Vector2(0, 0) } } },
			// <SecurityEngine(7), [Market(1), Engine(8)]>
			{7, new Dictionary<int, Vector2> { { 1, new Vector2(0, 0) }, { 8, new Vector2(718, 489) } } }
		};
		
		/*
		{?, new Dictionary<int, Vector2> { { ?, new Vector2(0, 0) } } }
		{0, }, {1, }, {2, }, {3, }, {4, }, {5, }, {6, },
			{7, }, {8, }, {9, }, {10, }, {11, }, {12, }, {13, },
			{14, }, {15, }, {16, }, {17, }, {18, }, {19, }, {20, }
		*/
		
	
	// Room UID's
	//<key, value>
	//<roomID, UID>
	//WARNING: Please be careful moving files around outside of Godot. This may fuck up its UID's
	public Dictionary<int, string> roomIDS = new Dictionary<int, string>
		{
			{0, "uid://g5yi2oufnd7b"}, {1, "uid://bj6i0sm27sq0y"}, {2, "uid://b5nusknx2t2"}, {3, "uid://yp8mbu2mbiq4"}, {4, "uid://drppupoqbsboi"}, {5, "uid://di3dj135mkpr1"}, {6, "uid://d0rvt1e2grm7m"},
			{7, "uid://btmygoh2s5mhk"}, {8, "uid://cpommox3imbij"}, {9, "uid://bfwsfpm7kj31g"}, {10, "uid://bo6rq3jnspnsh"}, {11, "uid://iekpssg83fnc"}, {12, "uid://bo2n32asslba0"}, {13, "uid://15ua3onjiawf"},
			{14, "uid://obh4emo4mbpj"}, {15, "uid://bt0dqpbuyngjm"}, {16, "uid://blvgy3jyqklp4"}, {17, "uid://t7rl7tpgiehl"}, {18, "uid://w6j3y6jgntrl"}, {19, "uid://d0xe8p32vykp"}, {20, "uid://dvgnk8gvwgja8"}
		};
	
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
		RoomsHostile[19] = true; //Boss room starts hostile
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
