using Godot;
using System;

public partial class Player : Entity
{
	public string CurrentDir = "D"; //Start looking down
	
	public int MaxHealth = 100; //Store maximum health for healing purposes
	
	public int Health; //Store player's health
	
	[Export]
	public FollowCamera PlayerCamera; //Camera that follows the player
	
	public bool RangedCooldown = true; //Whether the ranged attack cool down is finished
	
	public bool MeleeCooldown = true; //Whether the melee attack cool down is finished
	
	[Export]
	public Area2D InteractionZone; //Zone where interactables will register for the player
	
	public Interactable CurrentInteraction; //Current interaction with the player
	
	public Inventory Inv; //Player's inventory
	
	public int ItemCount = 0; //Player's item count
	
	public bool InDialogue = false; //Whether the player is in dialogue or not
	
	public override void _Ready()
	{
		base._Ready();
		
		Velocity = new Vector2(0, 0); //Don't move at the start
		
		Health = MaxHealth; //Start at maximum health
		
		CurrentInteraction = null; //The player is not interacting with anything
		
		MySpriteAnimation.Animation = "Walk_" + CurrentDir; //Set start animation
		
		Inv = (Inventory)GetTree().GetRoot().GetChild(1).GetNode("MainUI/Inventory"); //Get Player's inventory
		
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
					Inv.EquipedWeapons[0] = Game.Instance.Bite;
				}
				continue;
			}
			if(ItemCount > 0)
			{
				Inv.EquipItem(Game.Instance.PlayerWeapons[i].ID, ItemCount-1);
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
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
		if(Input.IsActionJustPressed("p_interact") && CurrentInteraction != null)
		{
			InDialogue = true;
			CurrentInteraction.BeginDialogue();
		}
		
		//If the player wishes to create a projectile and their cooldown is over
		if(Input.IsActionJustPressed("p_ranged") && RangedCooldown)
		{
			CreateProjectile();
		}
		
		//If the player wishes to make a melee attack and their cooldown is over
		if(Input.IsActionJustPressed("p_melee") && MeleeCooldown)
		{
			DealDamage();
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
			else
			{
				//If it is playing, do nothing, as the animation must finish playing
				return;
			}
		}

		//Get player input and set up velocity
		float hInput = Input.GetAxis("p_a", "p_d");
		float vInput = Input.GetAxis("p_s", "p_w");
		Velocity = new Vector2(0,0);
		if (Mathf.Abs(hInput) < 0.1f && Mathf.Abs(vInput) < 0.1f)
		{ //If the velocity is 0, just play the first frame of walking
			MySpriteAnimation.Frame = 0;
		}
		else if (Mathf.Abs(hInput) > Mathf.Abs(vInput))
		{
			//If the player is moving left or right
			MyPhysicsCollider.Rotation = ((float)Math.PI/180)*90.0f;
			if (hInput > 0)
			{ //If the player is moving right
				Velocity += new Vector2(100, 0);
				MySpriteAnimation.Animation = "Walk_R";
				CurrentDir = "R";
			}
			else
			{ 
				//If the player is moving left
				Velocity += new Vector2(-100, 0);
				MySpriteAnimation.Animation = "Walk_L";
				CurrentDir = "L";
			}
			PlayWalkingSound();
		}
		else
		{ //If the player is moving up or down
			MyPhysicsCollider.Rotation = 0;
			if (vInput > 0)
			{
				//up
				Velocity += new Vector2(0, -100);
				MySpriteAnimation.Animation = "Walk_U";
				CurrentDir = "U";
			}
			else
			{
				//down
				Velocity += new Vector2(0, 100);
				MySpriteAnimation.Animation = "Walk_D";
				CurrentDir = "D";
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
		Inv.Visible = !Inv.Visible;
	}
	
	//Shoot a bullet
	public void CreateProjectile()
	{
		//If the ranged weapon is still cooling down, don't shoot the bullet
		if(!RangedCooldown)
		{
			return;
		}
		//Get which weapon is equiped
		int Weapon = Inv.EquipedWeapons[1].ID;
		//Create an array of bullets based on the weapon
		Projectile[] Bullets = new Projectile[Weapon];
		//Since the pistol ID is 1 and the shotgun ID is 3, it works
		//I did not plan that
		//Create the bullets
		PackedScene PlayerProjectileScene = GD.Load<PackedScene>("res://Packed Scenes/Projectiles/Bullet.tscn");
		for(int i = 0; i < Weapon; i++)
		{
			Bullets[i] = (Projectile)PlayerProjectileScene.Instantiate();
			Bullets[i].Damage = Inv.EquipedWeapons[1].Damage;
		}
		//Set Position and Direction
		int BulletDistance = 65; //Distance bullet spawns from player
		float Angle = 1.0f; //Angle the bullet moves from the player
		if(CurrentDir == "D")
		{
			Bullets[0].Direction = new Vector2(0,1);
			Bullets[0].Position = new Vector2(GlobalPosition.X,GlobalPosition.Y+BulletDistance);
			if(Weapon == 3)
			{ //Other two bullets for the shotgun
				Bullets[1].Direction = new Vector2(-1*Angle,1);
				Bullets[1].Position = new Vector2(GlobalPosition.X-BulletDistance,GlobalPosition.Y+BulletDistance);
				Bullets[2].Direction = new Vector2(Angle,1);
				Bullets[2].Position = new Vector2(GlobalPosition.X+BulletDistance,GlobalPosition.Y+BulletDistance);
			}
		}
		else if(CurrentDir == "U")
		{
			Bullets[0].Direction = new Vector2(0,-1);
			Bullets[0].Position = new Vector2(GlobalPosition.X,GlobalPosition.Y-BulletDistance);
			if(Weapon == 3)
			{ //Other two bullets for the shotgun
				Bullets[1].Direction = new Vector2(-1*Angle,-1);
				Bullets[1].Position = new Vector2(GlobalPosition.X-BulletDistance,GlobalPosition.Y-BulletDistance);
				Bullets[2].Direction = new Vector2(Angle,-1);
				Bullets[2].Position = new Vector2(GlobalPosition.X+BulletDistance,GlobalPosition.Y-BulletDistance);
			}
		}
		else if(CurrentDir == "L")
		{
			Bullets[0].Direction = new Vector2(-1,0);
			Bullets[0].Position = new Vector2(GlobalPosition.X-65,GlobalPosition.Y);
			if(Weapon == 3)
			{ //Other two bullets for the shotgun
				Bullets[1].Direction = new Vector2(-1,Angle);
				Bullets[1].Position = new Vector2(GlobalPosition.X-BulletDistance,GlobalPosition.Y+BulletDistance);
				Bullets[2].Direction = new Vector2(-1,-1*Angle);
				Bullets[2].Position = new Vector2(GlobalPosition.X-BulletDistance,GlobalPosition.Y-BulletDistance);
			}
		}
		else if(CurrentDir == "R")
		{
			Bullets[0].Direction = new Vector2(1,0);
			Bullets[0].Position = new Vector2(GlobalPosition.X+65,GlobalPosition.Y);
			if(Weapon == 3)
			{ //Other two bullets for the shotgun
				Bullets[1].Direction = new Vector2(1,Angle);
				Bullets[1].Position = new Vector2(GlobalPosition.X+BulletDistance,GlobalPosition.Y+BulletDistance);
				Bullets[2].Direction = new Vector2(1,-1*Angle);
				Bullets[2].Position = new Vector2(GlobalPosition.X+BulletDistance,GlobalPosition.Y-BulletDistance);
			}
		}
		//Rotate the bullets for the shot gun and set bullet speeds
		int BulletSpeed = 300;
		if(Weapon == 3)
		{
			BulletSpeed = 150;
			if(CurrentDir == "L" || CurrentDir == "D")
			{
				Bullets[1].Rotation = ((float)Math.PI/180)*45;
				Bullets[2].Rotation = ((float)Math.PI/180)*315;
				if(CurrentDir == "L")
				{
					Bullets[2].Rotation = ((float)Math.PI/180)*135;
				}
			}
			else if(CurrentDir == "R" || CurrentDir == "U")
			{
				Bullets[1].Rotation = ((float)Math.PI/180)*135;
				Bullets[2].Rotation = ((float)Math.PI/180)*-135;
				if(CurrentDir == "R")
				{
					Bullets[1].Rotation = ((float)Math.PI/180)*-45;
				}
			}
		}
		//Create the bullets
		for(int i = 0; i < Weapon; i++)
		{
			Bullets[i].Speed = BulletSpeed;
			Bullets[i].SetCollisionLayerValue(3,true); //Bullet is player projectile
			Bullets[i].SetCollisionMaskValue(2,true); //Bullet is looking for interactables
			GetTree().GetRoot().AddChild(Bullets[i]);
			if(i > 0)
			{ //If these are the other two bullets spawned by the shotgun
				Bullets[i].IsDiagonal = true;
			}
		}
		//Play sound and animation
		if(Weapon == 1)
		{
			MySpriteAnimation.Animation = "Pistol_"+CurrentDir;
			MySpriteAnimation.Play();
			((AudioStreamPlayer2D)GetNode("Sounds/PistolLoud")).Play();
		}
		else if(Weapon == 3)
		{
			MySpriteAnimation.Animation = "Shotgun_"+CurrentDir;
			MySpriteAnimation.Play();
			((AudioStreamPlayer2D)GetNode("Sounds/ShotgunAndReload")).Play();
		}
		//Start the cooldown
		RangedCooldown = false;
		RangedCoolDown();
	}
	
	//Remove a door when opening it from the unlocked side
	public void PlayerRemovesDoor()
	{
		
	}
	
	//Use a secret passage
	public void UseSecretPass()
	{
		
	}
	
	//Increase local suspicion because of an event
	public void IncreaseLocalSus()
	{
		
	}
	
	//Tell the game that the player died
	public void InformOfDeath()
	{
		
	}
	
	//Deal damage when attacking something
	public void DealDamage()
	{
		//If the melee attack is still on cool down, do not melee attack
		if(!MeleeCooldown)
		{
			return;
		}
		
		//Play animation and sound
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
		
		//Go through every single interactable in the InteractionZone
		Godot.Collections.Array<Node2D> Interactables = InteractionZone.GetOverlappingBodies();
		foreach(Node2D body in Interactables)
		{
			//If it's an NPC, deal the damage of the equiped weapon
			if(body is NPC)
			{
				NPC Attacked = (NPC)body;
				Attacked.TakeDamage(Inv.EquipedWeapons[0].Damage);
			}
		}
		
		//Begin the melee attack cool down
		MeleeCooldown = false;
		MeleeCoolDown();
	}
	
	//Take damage when attacked or hit by a projectile
	public void TakeDamage(int damage)
	{
		Health -= damage;
		//If the player's health is below 0, die
		if(Health <= 0)
		{
			//Death logic goes here
			InformOfDeath();
		}
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
	
	//Mark the object as something that the player will interact with
	public void MarkInteractable(Node2D Body)
	{
		if(Body is Interactable)
		{
			CurrentInteraction = (Interactable)Body;
		}
	}
	
	//Mark the object that the player is interacting with as no longer interactable
	public void MarkNoLongerInteractable(Node2D Body)
	{
		CurrentInteraction = null;
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
			Inv.ItemsStored[ItemCount] = NewWeapon;
		}
		else if(ItemId == 4 || ItemId == 5)
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
			//Place key in the inventory
			NewKey.Portrait = (CompressedTexture2D)GD.Load(path);
			Inv.ItemsStored[ItemCount] = NewKey;
		}
		else
		{
			//Place key in the inventory
			Item NewItem = new Item();
			path = ""; //Path to hamster
			NewItem.ID = ItemId;
			Inv.ItemsStored[ItemCount] = NewItem;
		}
		//Increase number of items in the inventory
		ItemCount += 1;
		//Add weapon to the inventory on the screen
		GridContainer Items = (GridContainer)Inv.GetNode("Items");
		PackedScene SlotScene = GD.Load<PackedScene>("res://Packed Scenes/User Interface/InventorySlot.tscn");
		InventorySlot NewSlot = (InventorySlot)SlotScene.Instantiate();
		NewSlot.ID = ItemId;
		((TextureRect)NewSlot.GetNode("Portrait")).Texture = (CompressedTexture2D)GD.Load(path);
		//If it's not a weapon, get rid of the equip button. If it is, add functionality to the button
		if(ItemId > 4)
		{
			((Button)NewSlot.GetNode("EquipButton")).Visible = false;
		}
		else
		{
			NewSlot.Equip += Inv.EquipItem;
		}
		//Add the weapon to the screen
		Items.AddChild(NewSlot);
	}
}
