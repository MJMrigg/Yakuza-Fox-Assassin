using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node
{
	public float GlobalSuspicion = 0;
	
	public float MaxGlobalSuspicion = 0;
	
	public float[] LocalSuspicions = new float[21]; //Local Suspcisions of every room
	
	public float[] MaxLocalSuspicions = new float[21]; //Maximum Local Supsicions
	
	public float[] LocalSuspicionThresholds = new float[21]; //Local Suspicion thresholds of each room
	
	public bool[] RoomsHostile = new bool[21]; //Whether each room is hostile
	
	public float MasterVolume; //Master volume of the whole game
	
	public float SoundVolume; //Volume of all sound effects
	
	public float MusicVolume; //Volume of all music
	
	//Whether each step in a puzzle in complete
	public bool[] SecurityPuzzle = new bool[5];
	public bool[] MedicPuzzle = new bool[5];
	public bool[] StoragePuzzle = new bool[5];
	public bool[] ControlPuzzle = new bool[5];
	
	//Rooms of player
	public int PlayerRoom = 0;
	
	//Can Pual Move
	public bool paulCanMove = true;
	
	//Save/load data
	public bool[] FirstSaved = new bool[21]; //Whether the room has saved data
	public Item[] PlayerInventory = new Item[10]; //Player's inventory
	public Weapon[] PlayerWeapons = new Weapon[2]; //Player's equiped weapons
	public Dictionary<int, List<string>> NPCs = new Dictionary<int, List<string>>(); //NPC data
	//NPC Data key = _type, x, y, health, direction, animation, frame, dying
	public Dictionary<int, List<float>> RoomItems = new Dictionary<int, List<float>>
	{ //Data to store items in rooms
		{ 4, new List<float>() }, //Knife in the cafeteria
		{ 7, new List<float>() }, //Green key in security1
		{ 11, new List<float>() }, //Red key in medic
		{ 12, new List<float>() }, //Hamster in lab
		{ 15, new List<float>() } //Shotgun in armory
	};
	//Item Data key = ID
	public int[] ItemData = { //Data regarding items
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
	public Dictionary<int, List<string>> Projectiles = new Dictionary<int, List<string>>(); //Projectile data
	public Weapon Bite = new Weapon(); //Bite data
	public int PlayerHealth = 100;
	public int MaxPlayerHealth = 100;
	
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
	
	// Room UID's
	//<key, value>
	//<roomID, UID>
	//WARNING: Please be careful moving files around outside of Godot. This may fuck up its UID's
	public Dictionary<int, string> roomIDS = new Dictionary<int, string>
		{
			{0, "uid://usxq7o82i0fb"}, {1, "uid://cfrkcj0r61ak1"}, {2, "uid://b5nusknx2t2"}, {3, "uid://yp8mbu2mbiq4"}, {4, "uid://drppupoqbsboi"}, {5, "uid://di3dj135mkpr1"}, {6, "uid://d0rvt1e2grm7m"},
			{7, "uid://btmygoh2s5mhk"}, {8, "uid://cpommox3imbij"}, {9, "uid://bfwsfpm7kj31g"}, {10, "uid://bo6rq3jnspnsh"}, {11, "uid://iekpssg83fnc"}, {12, "uid://bo2n32asslba0"}, {13, "uid://15ua3onjiawf"},
			{14, "uid://obh4emo4mbpj"}, {15, "uid://bt0dqpbuyngjm"}, {16, "uid://blvgy3jyqklp4"}, {17, "uid://t7rl7tpgiehl"}, {18, "uid://w6j3y6jgntrl"}, {19, "uid://d0xe8p32vykp"}, {20, "uid://dvgnk8gvwgja8"}
		};
		
	//Settings Default controlMap
	public Dictionary<string, Godot.Collections.Array<Godot.InputEvent>> defControls = new Dictionary<string, Godot.Collections.Array<Godot.InputEvent>>();
	
	//Global instance of the game
	public static Game Instance { get; private set; }
	
	//Patrolling NPC data
	public int PatrolRoom = 18;
	public bool debugBool = true;
	public int prevPatrolRoom = 18; 
	
	//Paul move probd
	public float baseProbability = 1f;
	public float susWeightMult = 3f;
	
	public bool GameStart = false;
	
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
			Projectiles[i] = new List<string>();
			FirstSaved[i] = false; //The rooms currently have no saved data
		}
		RoomsHostile[20] = true; //Boss room starts hostile
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
		for(int i = 0; i < 21; i++)
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
		
		//DEBUG TEST
		//spawnPaul();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(GameStart){
			if(paulCanMove)
			{
				movePaul();
				paulCanMove = false;
			}
			if(PlayerRoom == PatrolRoom){
				GD.Print("Player and Paul are in the same room");
			}
		}
		
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
	
	//Changes Paul's current room
	public void movePaul()
	{
		Random random = new Random();
		GD.Print("-----------------");
		GD.Print("Paul was in Room: " + prevPatrolRoom);
		//Get room options
		var roomOptions = roomMap[PatrolRoom].Keys.ToList();
		
		//Remove bar rooms and docks
		roomOptions.Remove(13);
		roomOptions.Remove(5);
		roomOptions.Remove(0);
		
		//Player Combat Move; Priority 1
		if(roomOptions.Contains(PlayerRoom) && RoomsHostile[PlayerRoom])
		{
			PatrolRoom = PlayerRoom;
			GD.Print("Combat Detected at: " + PatrolRoom);
			return;
		}
		
		
		//Weightes Random Move; Priority 2
		var weights = new List<float>();
		float totalWeight = 0f;
		foreach (var room in roomOptions)
		{
			// Base Probability
			// As it is a ratio, all rooms are equal in Paul's eyes
			float susRatio = MaxLocalSuspicions[room] > 0 ? LocalSuspicions[room] / MaxLocalSuspicions[room] : 0f;
			float weight = baseProbability + (susRatio * susWeightMult);
			
			// Probability Modifiers
			// If a room is above their threshold Paul is more likely to go there
			if (LocalSuspicionThresholds[room] < LocalSuspicions[room])
			{
				weight *= 1.5f;
				GD.Print("Above Thres Weight Applied");
			}
			
			// If the Player is there, Paul is more likely to move to a room
			if (room == PlayerRoom)
			{
				weight *= 1.25f;
				GD.Print("Player Location Weight Applied");
			}
			
			//If Paul had just left a room he is less likely to go there
			if(room == prevPatrolRoom)
			{
				weight *= 0.75f;
				GD.Print("Prev Room Weight Applied");
			}
			
			// If the room would be a dead end, Paul is less likely to go there
			if(!(roomMap[room].Count > 1))
			{
				weight *= 0.75f;
				GD.Print("Dead End Weight Applied");
			}
				
			
			
			weights.Add(weight);
			GD.Print("Room " + room + " Weight: " + weight);
			totalWeight += weight;
		}
		GD.Print("Total Weight: " + totalWeight);
		prevPatrolRoom = PatrolRoom;
		
		// Weighted random selection
		float roll = (float)random.NextDouble() * totalWeight;
		float cumulative = 0f;
		for (int i = 0; i < roomOptions.Count; i++)
		{
			cumulative += weights[i];
			if (roll <= cumulative)
			{
				PatrolRoom = roomOptions.ElementAt(i);
				GD.Print("Selected Room: " + PatrolRoom);
				break;
			}
		}
		GD.Print("Paul is now in Room: " + PatrolRoom);
		GD.Print("-----------------");
		spawnPaul(); // Add Paul to the room he has entered
		killPaul(prevPatrolRoom); // Remove Paul from the room he was in previous
		paulWait();
	}
	
	// I summon Paul in defense position
	// Add Paul's String to NPC
	public void spawnPaul()
	{
		//Save Paul's data into the list
		NPCs[PatrolRoom].Add((7).ToString()); // Paul's TYPE
		NPCs[PatrolRoom].Add((roomMap[PatrolRoom][prevPatrolRoom].X).ToString()); // Paul's X Position (Same as Player)
		NPCs[PatrolRoom].Add((roomMap[PatrolRoom][prevPatrolRoom].Y).ToString()); // Paul's Y Position (Same as Player)
		NPCs[PatrolRoom].Add((500).ToString()); // Paul's HP
		NPCs[PatrolRoom].Add(("D").ToString()); // Paul's Current direction (same as Player)
		NPCs[PatrolRoom].Add(("Walk_D").ToString()); // Paul's current animation
		NPCs[PatrolRoom].Add((0).ToString()); // Paul's Current frame
		NPCs[PatrolRoom].Add((false).ToString()); // Paul's down
		
		GD.Print("Paul's Data has been added to Room ID: " + PatrolRoom);
	}
	
	//I'm sorry little one...
	// Remove's Paul's String from NPC.
	public void killPaul(int removeFromHere)
	{
		var index = NPCs[removeFromHere].FindIndex(x => x == "7"); // Find paul's type
		if (index != -1)
		{
			NPCs[removeFromHere].RemoveRange(index, 8); // Remove that type and the next indexs with his data
			GD.Print("Paul's Data has been removed from Room ID: " + removeFromHere);
		}
	}
	
	//How long Paul waits in a room
	public async void paulWait()
	{
		paulCanMove = false;
		await ToSignal(GetTree().CreateTimer(10),"timeout");
		paulCanMove = true;
	}
	
}
