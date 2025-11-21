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
	public Item[] PlayerInventory = new Item[10]; //Player's inventory
	public Weapon[] PlayerWeapons = new Weapon[2]; //Player's equiped weapons
	public Dictionary<int, List<string>> NPCs = new Dictionary<int, List<string>>(); //NPC data
	//NPC Data key = _type, x, y, health, direction, animation, frame, dying
	public Dictionary<int, List<Projectile>> Projectiles = new Dictionary<int, List<Projectile>>(); //Projectile data
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
	public int PlayerHealth = 100;
	
	//Test Info for room transition
	//<key, value>
	//<current room id, Dict<goes to this room id,Vector2 of Player Position in current room >>
	public Dictionary<int, Dictionary<int, Vector2>> roomMap = new Dictionary<int, Dictionary<int, Vector2>>
		{
			//FIRST HALF
			//<Docks(1), [Market(1)]>
			{0, new Dictionary<int, Vector2> { { 1, new Vector2(759, 253) } } },
			// <Market(1), [Docks(0), Living1(2), Cafeteria(4), Storage(6), SecurityEngine(7), Production(9), TrainingYard(10)]>
			{1, new Dictionary<int, Vector2> { { 0, new Vector2(-104, 252) }, { 2, new Vector2(181, -125) }, { 4, new Vector2(466, -125) }, { 6, new Vector2(684, -125) }, { 7, new Vector2(145, 719) }, { 9, new Vector2(578, 719) }, { 10, new Vector2(935, 250) } } },
			//<Living1(2), [Market(1), Bathroom1(3)]>
			{2, new Dictionary<int, Vector2> { { 1, new Vector2(974, 825) }, { 3, new Vector2(254, -195) } } },
			//<Bathroom1(3), [Living1(2)]>
			{3, new Dictionary<int, Vector2> { { 2, new Vector2(254, 1109) } } },
			//<Cafeteria(4) [Market(1), Bar1(5)]>
			{4, new Dictionary<int, Vector2> { { 1, new Vector2(974, 910) }, { 5, new Vector2(323, -177) } } },
			//<Bar1(5) [Cafeteria(4)]>
			{5, new Dictionary<int, Vector2> { { 4, new Vector2(289, 607) } } },
			//<Storage(6) [Market(1)]>
			{6, new Dictionary<int, Vector2> { { 1, new Vector2(434, 874) } } },
			// <SecurityEngine(7), [Market(1), Engine(8)]>
			{7, new Dictionary<int, Vector2> { { 1, new Vector2(253, -171) }, { 8, new Vector2(718, 489) } } },
			// <Engine(8), [SecurityEngine(7)]>
			{8, new Dictionary<int, Vector2> { { 7, new Vector2(394, -54) } } },
			// <Production(9), [Market(1)]>
			{9, new Dictionary<int, Vector2> { { 1, new Vector2(433, -211) } } },

			//SECOND HALF
			//<Training Yard(10), [Market(1), Medic(11), Bar2(13), Range(14), Living(16), Security2(18)]>
			{10, new Dictionary<int, Vector2> { {1, new Vector2(-141,251) }, {11, new Vector2(180,-57) }, {13, new Vector2(469,-57) }, {14, new Vector2(684,-61) }, { 16, new Vector2(364,693) }, { 18, new Vector2(860,257) } } },
			//<Medic(11), [Training Yard(10, Lab(12)]>
			{11, new Dictionary<int, Vector2> { { 10, new Vector2(360,913) }, { 12, new Vector2(1407,502) } } },
			//<Lab(12), [Medic(11)]>
			{12, new Dictionary<int, Vector2> { { 11, new Vector2(-57,577) } } },
			//<Bar2(13), [Training Yard(10)]>
			{13, new Dictionary<int, Vector2> { { 10, new Vector2(289,589) } } },
			//<Range(14), [Training Yard(10), Armory(15)]>
			{14, new Dictionary<int, Vector2> { { 10, new Vector2(143,812) }, { 15, new Vector2(502,-88) } } },
			//<Armory(15), [Range(14)]>
			{15, new Dictionary<int, Vector2> { { 14, new Vector2(505,524) } } },
			//<Living2(16), [Training Yard(10), Bathroom2(17)]>
			{16, new Dictionary<int, Vector2> { { 10, new Vector2(358,-24) }, { 17, new Vector2(1366,511) } } },
			//<Bathroom2(17), [Living2(16)]>
			{17, new Dictionary<int, Vector2> { { 16, new Vector2(-57,582) } } },
			//<Security2(18), [Training Yard(10), Controls(19)]>
			{18, new Dictionary<int, Vector2> { { 10, new Vector2(-107,256) }, { 19, new Vector2(687,-109) } } },
			//<Controls(19), [Security2(18)]>
			{19, new Dictionary<int, Vector2> { { 18, new Vector2(289,848) } } },
			//<Boss(20), [Security2(18)]>
			{20, new Dictionary<int, Vector2> { { 18, new Vector2(0,0) } } }
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
			{0, "uid://usxq7o82i0fb"}, {1, "uid://bj6i0sm27sq0y"}, {2, "uid://b5nusknx2t2"}, {3, "uid://yp8mbu2mbiq4"}, {4, "uid://drppupoqbsboi"}, {5, "uid://di3dj135mkpr1"}, {6, "uid://d0rvt1e2grm7m"},
			{7, "uid://btmygoh2s5mhk"}, {8, "uid://cpommox3imbij"}, {9, "uid://bfwsfpm7kj31g"}, {10, "uid://bo6rq3jnspnsh"}, {11, "uid://iekpssg83fnc"}, {12, "uid://bo2n32asslba0"}, {13, "uid://15ua3onjiawf"},
			{14, "uid://obh4emo4mbpj"}, {15, "uid://bt0dqpbuyngjm"}, {16, "uid://blvgy3jyqklp4"}, {17, "uid://t7rl7tpgiehl"}, {18, "uid://w6j3y6jgntrl"}, {19, "uid://d0xe8p32vykp"}, {20, "uid://dvgnk8gvwgja8"}
		};
		
	//Settings Default controlMap
	public Dictionary<string, Godot.Collections.Array<Godot.InputEvent>> defControls = new Dictionary<string, Godot.Collections.Array<Godot.InputEvent>>();
	
	//Global instance of the game
	public static Game Instance { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Create a global instance of the Game
		Instance = this;
		//Set up local suspicions, thresholds, and NPCs
		for(int i = 0; i < 20; i++)
		{
			LocalSuspicions[i] = 0;
			RoomsHostile[i] = false;
			NPCs[i] = new List<string>();
			Projectiles[i] = new List<Projectile>();
		}
		RoomsHostile[19] = true; //Boss room starts hostile
		NPCs[0] = null; //No NPCs in the first room
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
		Bite.Portrait = null;
		PlayerWeapons[0] = Bite;
		//Set up player pistol
		PlayerWeapons[1] = new Weapon();
		PlayerWeapons[1].ID = ItemData[3];
		PlayerWeapons[1].Damage = ItemData[4];
		PlayerWeapons[1].CoolDown = ItemData[5];
		//Place it in their inventory
		PlayerInventory[0] = new Item();
		PlayerInventory[0].ID = ItemData[3];
		
		//Set Up Default controls
		var actions = InputMap.GetActions();
		foreach (var i in actions)
		{
			string actionName = i.ToString();
			if (!actionName.StartsWith("ui_"))
			{
				var test = InputMap.ActionGetEvents(actionName);
				defControls.Add(i, test);
			}
		}
		
		
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
