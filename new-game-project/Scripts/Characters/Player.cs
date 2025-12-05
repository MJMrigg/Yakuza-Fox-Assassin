using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : Entity
{
	[Export]
	public int Speed = 200; //Movement speed
	
	public bool IsDashing = false; //Whether the player is dashing
	
	public int MaxHealth = 100; //Store maximum health for healing purposes
	
	public int Health; //Store player's health
	
	[Export]
	public FollowCamera PlayerCamera; //Camera that follows the player
	
	public bool RangedCooldown = true; //Whether the ranged attack cool down is finished
	
	public bool MeleeCooldown = true; //Whether the melee attack cool down is finished
	
	public bool DashCooldown = true; //Whether the dash cool down is finished
	
	[Export]
	public Area2D InteractionZone; //Zone where interactables will register for the player
		
	public PanelContainer InvPanel;
	public Inventory Inv; //Player's inventory
	
	public int ItemCount = 0; //Player's item count
	
	public bool InDialogue = false; //Whether the player is in dialogue or not
	
	public bool Dying = false; //Whether the player is dying
	
	[Export]
	public CanvasLayer PlayerUI; //Menus the player controls
	
	public bool Choice = false; //Whether the player has chosen to start combat
	
	//Player sus go up
	public bool increraseSus = true;
	public bool susDelay = false;
	
	[Export]
	public Area2D MeleeAttackRadius; //Zone where things the player can melee attack are
	
	public override void _Ready()
	{
		base._Ready();
		
		Velocity = new Vector2(0, 0); //Don't move at the start
		
		Health = MaxHealth; //Start at maximum health
		
		MySpriteAnimation.Animation = "Walk_" + CurrentDir; //Set start animation
		
		Inv = (Inventory)(GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).GetNode("MainUI/Inventory/Inventory")); //Get Player's inventory\
		InvPanel = (PanelContainer)(GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).GetNode("MainUI/Inventory"));
		
		//Set up the player's inventory
		for(int i = 0; i < 6; i++)
		{
			//If there are no more items, break
			if(Game.Instance.PlayerInventory[i] == null)
			{
				break;
			}
			Inv.ItemsStored[i] = Game.Instance.PlayerInventory[i];
			this.Pickup(Inv.ItemsStored[i].ID);
		}
		
		//Equip the player with their starting weapons
		for(int i = 0; i < 2; i++)
		{
			int WeaponId = Game.Instance.PlayerWeapons[i].ID;
			//If the player didn't have a weapon equipped in the slot, move on
			if(WeaponId == 0 || WeaponId == null || WeaponId == -1)
			{
				//It it was because the player had the bite equiped, equip the bite
				if(WeaponId == 0)
				{
					Inv.EquipedWeapons[i] = Game.Instance.Bite;
				}
				continue;
			}
			if(ItemCount > 0)
			{
				Inv.EquipItem(Game.Instance.PlayerWeapons[i].ID, Game.Instance.PlayerWeapons[i].InventorySlot);
			}
		}
		
		//Set player current health
		Health = Game.Instance.PlayerHealth;
		
		//Start Player susDelay timer
		initialSusDelay();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		//If the player killed the boss, tell the player
		if(Game.Instance.BossIsDead)
		{
			MySpriteAnimation.Stop();
			InformOfDeath();
			return;
		}
		
		//If the player has no more health, kill the player
		if(Health <= 0)
		{
			if(!Dying)
			{
				GetTree().CallGroup("Pausable","Pause");
				InformOfDeath();
			}
			//Stop playing the animation
			if(MySpriteAnimation.Animation != "Die_"+CurrentDir)
			{
				MySpriteAnimation.Stop();
			}
			return;
		}
		
		//If the player became to suspicious, tell the player
		if(Game.Instance.GlobalSuspicion >= Game.Instance.MaxGlobalSuspicion)
		{
			MySpriteAnimation.Stop();
			GetTree().CallGroup("Pausable","Pause");
			InformOfDeath();
			return;
		}
		
		//If the player drank too much champaw, tell the player
		if(Game.Instance.CurrentDrinks >= Game.Instance.DrinkLimit)
		{
			MySpriteAnimation.Stop();
			GetTree().CallGroup("Pausable","Pause");
			InformOfDeath();
			return;
		}
		
		
		//Increase Player sus every X amount seconds
		if(susDelay && increraseSus)
		{
			//Increase Player sus
			increraseSus = false;
			susIncreaseOverTime();
		}
		
		
		//I'm sick of waiting
		if(Input.IsActionPressed("cheat"))
		{
			MeleeCooldown = true;
			RangedCooldown = true;
		}
		
		//If the player wishes to open/close their inventory
		if(Input.IsActionJustPressed("p_inv"))
		{
			OpenInv();
		}
		
		//If paused, do nothing
		if(Stop)
		{
			MySpriteAnimation.Pause();
			return;
		}
		
		//If the player wishes to interact with an interactable object and there is an object for them to interact with
		if(Input.IsActionJustPressed("p_interact"))
		{
			//Go through the objects in the interaction zone
			Godot.Collections.Array<Node2D> Interactables = InteractionZone.GetOverlappingBodies();
			foreach(Node2D Interaction in Interactables)
			{
				//Do not interact with hositle NPCs
				if(Interaction is NPC && ((NPC)Interaction).IsHostile)
				{
					continue;
				}
				//Begin dialogue with the first interactable object
				if(Interaction is Interactable)
				{
					InDialogue = true;
					Interactable body = (Interactable)Interaction;
					body.BeginDialogue();
					break;
				}
			}
		}
		
		//If the player wishes to create a projectile and their cooldown is over
		if(Input.IsActionJustPressed("p_ranged") && RangedCooldown)
		{
			CreateProjectile();
			// Increase local sus by 2%
			var roomSus = Game.Instance.LocalSuspicions[RoomId];
			float x = roomSus * 0.02f;
			Game.Instance.IncreaseLocalSuspicion(RoomId, x);
		}
		
		//If the player wishes to make a melee attack and their cooldown is over
		if(Input.IsActionJustPressed("p_melee") && MeleeCooldown)
		{
			DealDamage();
			// Increase local sus by 2%
			var roomSus = Game.Instance.LocalSuspicions[RoomId];
			float x = roomSus * 0.02f;
			Game.Instance.IncreaseLocalSuspicion(RoomId, x);
		}
		
		//If the animation is currently not walking
		if (!MySpriteAnimation.Animation.ToString().StartsWith("Walk"))
		{
			//If the animation is not playing, have the player face its current direction
			if (!MySpriteAnimation.IsPlaying())
			{
				MySpriteAnimation.Animation = "Walk_"+CurrentDir;
				MySpriteAnimation.Frame = 0;
			}
			else if(!MySpriteAnimation.Animation.ToString().StartsWith("Dash"))
			{
				//If it is playing, do nothing, as the animation must finish playing
				//(unless it's the dash)
				return;
			}
		}
		
		if(Input.IsActionJustPressed("p_dash") && DashCooldown)
		{
			Speed = 500;
			((AudioStreamPlayer2D)GetNode("Sounds/Dash")).Play();
			MySpriteAnimation.Animation = "Dash_"+CurrentDir;
			MySpriteAnimation.Play();
			// Increase local sus by 2%
			var roomSus = Math.Max(Game.Instance.LocalSuspicions[RoomId], 0.02f);
			float IncreaseAmount = roomSus * 0.02f;
			Game.Instance.IncreaseLocalSuspicion(RoomId, IncreaseAmount);
			IsDashing = true;
			DashDistanceCoolDown();
			DashCoolDown();
		} 
		
		//Get player input and set up animation and velocity
		float hInput = Input.GetAxis("p_a", "p_d");
		float vInput = Input.GetAxis("p_s", "p_w");
		Velocity = new Vector2(0,0);
		//Set the current animation to either walk or dash
		string AnimationStart = "Walk_";
		if(IsDashing)
		{
			AnimationStart = "Dash_";
		}
		//Determine Velocity
		if (Mathf.Abs(hInput) < 0.1f && Mathf.Abs(vInput) < 0.1f)
		{ //If the velocity is 0, just play the first frame of walking
			MySpriteAnimation.Frame = 0;
		}
		else if (Mathf.Abs(hInput) >= Mathf.Abs(vInput))
		{
			//If the player is moving left or right
			if (hInput > 0)
			{ //If the player is moving right
				Velocity += new Vector2(Speed, 0);
				MySpriteAnimation.Animation = AnimationStart+"R";
				CurrentDir = "R";
				MyPhysicsCollider.Rotation = ((float)Math.PI/180)*-90.0f;
			}
			else
			{ 
				//If the player is moving left
				Velocity += new Vector2(-Speed, 0);
				MySpriteAnimation.Animation = AnimationStart+"L";
				CurrentDir = "L";
				MyPhysicsCollider.Rotation = ((float)Math.PI/180)*90.0f;
			}
			//If the player is walking diagonally
			if(Mathf.Abs(hInput) == Mathf.Abs(vInput))
			{
				if(vInput > 0)
				{ 
					//Diagonally Up
					Velocity += new Vector2(0,-Speed);
				}
				else
				{
					//Diagonally Down
					Velocity += new Vector2(0,Speed);
				}
				//Adjust using the unit circle so that diagonal movement isn't faster
				float Adjustment = Mathf.Sqrt(2)/2;
				Velocity *= new Vector2(Adjustment,Adjustment);
			}
			//Play the walking sound
			PlayWalkingSound();
		}
		else
		{ //If the player is moving up or down
			if (vInput > 0)
			{
				//up
				Velocity += new Vector2(0, -Speed);
				MySpriteAnimation.Animation = AnimationStart+"U";
				CurrentDir = "U";
				MyPhysicsCollider.Rotation = ((float)Math.PI/180)*180;
			}
			else
			{
				//down
				Velocity += new Vector2(0, Speed);
				MySpriteAnimation.Animation = AnimationStart+"D";
				CurrentDir = "D";
				MyPhysicsCollider.Rotation = 0;
			}
			PlayWalkingSound();
		}
		
		MySpriteAnimation.Play();
		MoveAndSlide();
		
		//Move the player camera along with the player
		if(PlayerCamera != null)
		{
			PlayerCamera.ChangePosition(Position);
		}

	}
	
	//Open inventory
	public void OpenInv()
	{
		//If not in dialogue, pause the whole screen
		if(!InDialogue)
		{
			GetTree().CallGroup("Pausable","Pause");
		}
		//Make the inventory and Settings menu invisible.
		InvPanel.Visible = !InvPanel.Visible;
		((Control)GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).GetNode("MainUI/Settings")).Visible = false;
	}
	
	//alternate projectile test
	public void CreateProjectile()
	{
		//If the player is in the bar, do not attack
		if(RoomId == 5 || RoomId == 13)
		{
			return;
		}
		//If there is no gun equipped, do a bite attack
		if(Inv.EquipedWeapons[1].ID == 0)
		{
			DealDamage();
			return;
		}
		//If the ranged weapon is still cooling down, don't shoot the bullet
		if(!RangedCooldown)
		{
			return;
		}
		
		// Get mouse position and calculate direction
		Vector2 mousePos = GetGlobalMousePosition();
		Vector2 directionToMouse = (mousePos - GlobalPosition).Normalized();
		
		// Determine animation direction
		string animDir = "D";
		if(Mathf.Abs(directionToMouse.X) >= Mathf.Abs(directionToMouse.Y))
		{
			// Mouse is more horizontal
			animDir = directionToMouse.X > 0 ? "R" : "L";
		}
		else
		{
			// Mouse is more vertical
			animDir = directionToMouse.Y > 0 ? "D" : "U";
		}
		
		//Get which weapon is equiped
		int Weapon = Inv.EquipedWeapons[1].ID;
		//Create an array of bullets based on the weapon
		Projectile[] Bullets = new Projectile[Weapon];
		
		//Create the bullets
		PackedScene PlayerProjectileScene = GD.Load<PackedScene>("uid://2ajb1ycxcaik"); // bullet scene
		for(int i = 0; i < Weapon; i++)
		{
			Bullets[i] = (Projectile)PlayerProjectileScene.Instantiate();
			Bullets[i].Damage = Inv.EquipedWeapons[1].Damage;
		}
		
		//Set Position and Direction
		int BulletDistance = 65; //Distance bullet spawns from player
		Vector2 spawnOffset = directionToMouse * BulletDistance;
		
		// Main bullet - always shoots toward mouse
		Bullets[0].Direction = directionToMouse;
		Bullets[0].Position = GlobalPosition + spawnOffset;
		
		// For shotgun, create spread pattern
		if(Weapon == 3)
		{
			float spreadAngle = 0.4f; //radians not degrees. took me a sec to figure that out - DT
			
			// Left bullet
			Vector2 leftDirection = directionToMouse.Rotated(-spreadAngle);
			Bullets[1].Direction = leftDirection;
			Bullets[1].Position = GlobalPosition + leftDirection * BulletDistance;
			
			// Right bullet
			Vector2 rightDirection = directionToMouse.Rotated(spreadAngle);
			Bullets[2].Direction = rightDirection;
			Bullets[2].Position = GlobalPosition + rightDirection * BulletDistance;
		}
		
		//Set bullet speeds
		int BulletSpeed = 300;
		if(Weapon == 3)
		{
			BulletSpeed = 200;
		}
		
		//Create the bullets
		for(int i = 0; i < Weapon; i++)
		{
			Bullets[i].Speed = BulletSpeed;
			Bullets[i].SetCollisionMaskValue(2,true); //Bullet is looking for interactables
			Bullets[i].CurrentDir = animDir;
			GetTree().GetRoot().GetChild(Game.Instance.SceneIndex).AddChild(Bullets[i]);
		}
		
		//Play sound and animation
		if(Weapon == 1)
		{
			MySpriteAnimation.Animation = "Pistol_"+animDir;
			MySpriteAnimation.Play();
			((AudioStreamPlayer2D)GetNode("Sounds/PistolLoud")).Play();
		}
		else if(Weapon == 3)
		{
			MySpriteAnimation.Animation = "Shotgun_"+animDir;
			MySpriteAnimation.Play();
			((AudioStreamPlayer2D)GetNode("Sounds/ShotgunAndReload")).Play();
		}
		
		//Start the cooldown
		RangedCooldown = false;
		RangedCoolDown();
	}
	
	//Increase local suspicion because of an event
	public void IncreaseLocalSus(int amount)
	{
		Game.Instance.IncreaseLocalSuspicion(RoomId, amount);
	}
	
	//Tell the player that they lost or won the game
	public void InformOfDeath()
	{
		Panel WinLoseMenu = (Panel)PlayerUI.GetNode("WinLoseMenu");
		WinLoseMenu.Visible = true;
		if(Game.Instance.BossIsDead) //The boss died
		{ //The boss died
			((Label)WinLoseMenu.GetNode("Text")).Text = "You have slain of your father!";
		}else if(Dying || Health <= 0) //The player died
		{ //The player died
			((Label)WinLoseMenu.GetNode("Text")).Text = "You Died.";
		}
		else if(Game.Instance.GlobalSuspicion >= Game.Instance.MaxGlobalSuspicion)
		{ //Global suspicion is too high
			((Label)WinLoseMenu.GetNode("Text")).Text = "You triggered the alarm. The boss has fled.";
			((AudioStreamPlayer2D)GetNode("Sounds/GlobalSuspicionTooHigh")).Play();
		}
		else if(Game.Instance.CurrentDrinks >= Game.Instance.DrinkLimit)
		{ //The player drank to much champawn
			((Label)WinLoseMenu.GetNode("Text")).Text = "You drank too much. You pass and are captured.";
			((AudioStreamPlayer2D)GetNode("Sounds/PlayerGameOverSad")).Play();
		}
	}
	
	//Deal damage when attacking something
	public async void DealDamage()
	{
		//If the player is in the bar, do not attack
		if(RoomId == 5 || RoomId == 13)
		{
			return;
		}
		
		//If the melee attack is still on cool down, do not melee attack
		if(!MeleeCooldown)
		{
			return;
		}
		
		//Go through every single interactable in the InteractionZone
		NPC Attacked = null; //Assume there are none
		Godot.Collections.Array<Node2D> Interactables = MeleeAttackRadius.GetOverlappingBodies();
		foreach(Node2D body in Interactables)
		{
			//If the room is not yet hostile, ask the player if they want to start combat in this room
			if(!Game.Instance.RoomsHostile[RoomId])
			{
				//Pause the room
				GetTree().CallGroup("Pausable","Pause");
				//Ask the user what they want to do
				YesNoMenu ChoiceMenu = (YesNoMenu)PlayerUI.GetNode("YesNoMenu");
				ChoiceMenu.Visible = true;
				//Wait for their choice
				await ToSignal(ChoiceMenu, YesNoMenu.SignalName.Choice);
				//Unpause the room
				GetTree().CallGroup("Pausable","Pause");
				ChoiceMenu.Visible = false;
				//If the user said no, don't do anything;
				if(!Choice)
				{
					break;
				}
			}
			else
			{ //The choice is already made for them if the room is already hostile
				Choice = true;
			}
			//If it's an NPC, deal the damage of the equiped weapon
			if(body is NPC)
			{
				Attacked = (NPC)body;
				Attacked.TakeDamage(Inv.EquipedWeapons[0].Damage);
				Game.Instance.RoomsHostile[RoomId] = true; //Set room to hostile
			}
		}
		
		//If the player chose to attack or there was nothing to attack, play the attack animation and sound
		if(Choice || Attacked == null)
		{
			if(Inv.EquipedWeapons[0].ID == 0)
			{ //Bite
				MySpriteAnimation.Animation = "Bite_"+CurrentDir;
				((AudioStreamPlayer2D)GetNode("Sounds/BiteAttack")).Play();
			}
			else
			{ //Knife
				MySpriteAnimation.Animation = "Knife_"+CurrentDir;
				int Chosen = (int)(GD.Randi() % 3) + 1;
				((AudioStreamPlayer2D)GetNode("Sounds/Knife"+Chosen)).Play();
			}
			MySpriteAnimation.Play();
		}
		
		//Reset the choice if the player didn't choose to make the room hostile
		Choice = false;
		
		//Begin the melee attack cool down
		MeleeCooldown = false;
		MeleeCoolDown();
	}
	
	//Take damage when attacked or hit by a projectile
	public void TakeDamage(int damage)
	{
		//If the player is already dying, do nothing
		if(Dying)
		{
			return;
		}
		//Take the damage
		Health -= damage;
		Game.Instance.PlayerHealth -= damage;
		//HealthBar.Value = Health;
		//If the player's health is below 0, die
		if(Health <= 0)
		{
			Dying = true;
			//Play death animation and sound
			MySpriteAnimation.Animation = "Hurt_"+CurrentDir;
			AudioStreamPlayer2D DeathSound = (AudioStreamPlayer2D)GetNode("Sounds/PlayerGameOverSad");
			DeathSound.Play();
			//Wait for the animation and the sound to stop playing
			Remove();
			return;
		}
		MySpriteAnimation.Animation = "Hurt_"+CurrentDir;
		MySpriteAnimation.Play();
	}
	
	public async void DashDistanceCoolDown()
	{
		await ToSignal(GetTree().CreateTimer(0.4),"timeout");
		MySpriteAnimation.Animation = "Walk_"+CurrentDir;
		MySpriteAnimation.Play();
		IsDashing = false;
		Speed = 200;
	}
	
	public async void DashCoolDown()
	{
		DashCooldown = false;
		await ToSignal(GetTree().CreateTimer(1.4),"timeout");
		DashCooldown = true;
	}
	
	//Start the cool down for the melee attack
	public async void MeleeCoolDown()
	{
		MeleeCooldown = false;
		await ToSignal(GetTree().CreateTimer(Inv.EquipedWeapons[0].CoolDown),"timeout");
		MeleeCooldown = true;
	}
	
	//Start the cool down for the ranged attack
	public async void RangedCoolDown()
	{
		RangedCooldown = false;
		await ToSignal(GetTree().CreateTimer(Inv.EquipedWeapons[1].CoolDown),"timeout");
		RangedCooldown = true;
	}
	
	//Pick up an item
	public void Pickup(int ItemId)
	{
		string path = "";
		//Handle if the new item is a weapon, a key, or just an item
		if(ItemId < 4)
		{
			Weapon NewWeapon = new Weapon();
			NewWeapon.ID = ItemId;
			if(ItemId == 1)
			{ //Pistol
				path = "res://Art Assets/Items/gun_Pistol.png";
			}
			else if(ItemId == 2)
			{ //Knife
				path = "res://Art Assets/Items/knife.png";
			}
			else if(ItemId == 3)
			{ //Shotgun
				path = "res://Art Assets/Items/gun_Shotgun.png";
			}
			//Place weapon in the inventory
			NewWeapon.Portrait = (CompressedTexture2D)GD.Load(path);
			NewWeapon.InventorySlot = ItemCount;
			Inv.ItemsStored[ItemCount] = NewWeapon;
		}
		else if(ItemId == 4 || ItemId == 5 || ItemId == 7)
		{
			Key NewKey = new Key();
			NewKey.ID = ItemId;
			if(ItemId == 4)
			{ //Green key
				NewKey.KeyColor = true;
				path = "res://Art Assets/Items/keycard_green.png";
			}
			else if(ItemId == 5)
			{ //Red key
				NewKey.KeyColor = false;
				path = "res://Art Assets/Items/keycard_red.png";
			}
			else if(ItemId == 7)
			{ //Blue key
				path = "uid://b8rnjl0pao1hc";
			}
			//Place key in the inventory
			NewKey.Portrait = (CompressedTexture2D)GD.Load(path);
			NewKey.InventorySlot = ItemCount;
			Inv.ItemsStored[ItemCount] = NewKey;
		}
		else
		{
			//Place hamster in the inventory
			Item NewItem = new Item();
			path = "res://Art Assets/Items/hamster.png"; //Path to hamster
			NewItem.ID = ItemId;
			NewItem.InventorySlot = ItemCount;
			Inv.ItemsStored[ItemCount] = NewItem;
		}
		//Add item to the player inventory in the game master
		Game.Instance.PlayerInventory[ItemCount] = new Item();
		Game.Instance.PlayerInventory[ItemCount].ID = ItemId;
		//Remove the item from the room save and load data in the game master
		//Go through each item. Shouldn't be too bad since rooms have at most 2 items
		if(Game.Instance.RoomItems.ContainsKey(RoomId))
		{
			for(int i = 0; i < Game.Instance.RoomItems[RoomId].Count; i+=3)
			{
				//If the item wasn't in the room, but rather in the player's inventory, do noting
				if(!Game.Instance.RoomItems.ContainsKey(RoomId))
				{
					break;
				}
				//If this isn't the item, move on
				if(Game.Instance.RoomItems[RoomId][i] != ItemId)
				{
					continue;
				}
				//Remove the item
				(Game.Instance.RoomItems[RoomId]).RemoveRange(i,3);
				break;
			}
		}
		//Increase number of items in the inventory
		ItemCount += 1;
		//Add weapon to the inventory on the screen
		GridContainer Items = (GridContainer)Inv.GetNode("Items");
		PackedScene SlotScene = GD.Load<PackedScene>("res://Packed Scenes/User Interface/InventorySlot.tscn");
		InventorySlot NewSlot = (InventorySlot)SlotScene.Instantiate();
		NewSlot.ID = ItemId;
		((TextureRect)NewSlot.GetNode("PanelContainer/Portrait")).Texture = (CompressedTexture2D)GD.Load(path);
		//If it's not a weapon, get rid of the equip button. If it is, add functionality to the button
		if(ItemId >= 4)
		{
			Button EquipButton = ((Button)NewSlot.GetNode("EquipButton"));
			EquipButton.Visible = false;
		}
		else
		{
			NewSlot.Equip += Inv.EquipItem;
		}
		//Add the item to the screen
		Items.AddChild(NewSlot);
	}
	
	//Kill the player
	public async override void Remove()
	{
		//Play the death animation
		MySpriteAnimation.Animation = "Die_"+CurrentDir;
		MySpriteAnimation.Play();
		await ToSignal(MySpriteAnimation, AnimatedSprite2D.SignalName.AnimationFinished);
		//Pause the game
		GetTree().CallGroup("Pausable","Pause");
		//Tell the game the player died
		InformOfDeath();
	}
	
	//When the player has made a choice to make the room hostile
	public void ChoiceMade(bool PlayerChoice)
	{
		Choice = PlayerChoice;
	}
	
	//The initial delay before a room sus starts increasing
	public async void initialSusDelay()
	{
		susDelay = false;
		await ToSignal(GetTree().CreateTimer(5),"timeout");
		susDelay = true;
	}
	
	//Start the cool down for the ranged attack
	public async void susIncreaseOverTime()
	{
		increraseSus = false;
		//var pRoom = Game.Instance.PlayerRoom;
		//GD.Print("susIncreaseOverTime has been called.");
		//GD.Print("This is room id: " + RoomId);
		if(RoomId == 0 || RoomId == 5 || RoomId == 13 || RoomId == 20 )
		{
			//GD.Print("Safe room id");
		}
		else
		{
			var roomSus = Game.Instance.LocalSuspicions[RoomId];
			//GD.Print("This room's sus: " + roomSus);
			float x = roomSus * 0.05f;
			//GD.Print("Increasing this room's sus by: " + x);
			//Not using local function because I need a float
			Game.Instance.IncreaseLocalSuspicion(RoomId, x);
		}
		//await ToSignal(GetTree().CreateTimer(5),"timeout");
		await playerTimeWithPause(5f);
		increraseSus = true;
	}
	
	public async Task playerTimeWithPause(float duration)
	{
		float elapsed = 0f;
		
		while (elapsed < duration)
		{
			if (!(Game.Instance.isPaused))
			{
				elapsed += (float)GetProcessDeltaTime();
			}
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
	}
	
	//Mark an interatable object as interactable
	public void MarkInteractable(Node2D body)
	{
		//Make sure the body is an interactable object
		if(!(body is Interactable))
		{
			return;
		}
		Interactable CurrentInteraction = (Interactable)body;
		//Make sure the NPC isn't a hostile NPC
		if(CurrentInteraction is NPC)
		{
			if(((NPC)CurrentInteraction).IsHostile){
				return;
			}
		}
		//Make sure it has an interaction box
		if(CurrentInteraction.InteractionBox == null)
		{
			return;
		}
		//Mark it as interactable by showing its interaction box
		CurrentInteraction.InteractionBox.Visible = true;
	}
	
	public void MarkNoLongerInteractable(Node2D body)
	{
		//Make sure the body is an interactable object
		if(!(body is Interactable))
		{
			return;
		}
		Interactable CurrentInteraction = (Interactable)body;
		//Make sure it has an interaction box
		if(CurrentInteraction.InteractionBox == null)
		{
			return;
		}
		//Mark it as no longer interactable by hiding its interaction box
		CurrentInteraction.InteractionBox.Visible = false;
	}
}
