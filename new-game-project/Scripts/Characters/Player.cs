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
	
	[Export]
	public Area2D InteractionZone; //Zone where interactables will register for the player
	
	public Interactable CurrentInteraction; //Current interaction with the player
	
	public Inventory Inv; //Player's inventory
	
	public int ItemCount = 0; //Player's item count
	
	public override void _Ready()
	{
		base._Ready();
		
		Velocity = new Vector2(0, 0); //Don't move at the start
		
		Health = MaxHealth; //Start at maximum health
		
		CurrentInteraction = null; //The player is not interacting with anything
		
		MySpriteAnimation.Animation = "Walk_" + CurrentDir; //Set start animation
		
		Inv = (Inventory)GetTree().GetRoot().GetChild(1).GetNode("MainUI/Inventory"); //Get Player's inventory
		
		//Equip the player with their starting weapons
		for(int i = 0; i < 2; i++)
		{
			int WeaponId = Game.Instance.PlayerWeapons[i].ID;
			//If the player didn't have a weapon equipped in the slot, move on
			if(WeaponId == 0 || WeaponId == null || WeaponId == -1)
			{
				continue;
			}
			this.Pickup(Game.Instance.PlayerWeapons[i].ID);
			GD.Print(Game.Instance.PlayerWeapons[i].ID);
			ItemCount = Inv.GetNode("Items").GetChildCount();
			if(ItemCount > 0)
			{
				Inv.EquipItem(Game.Instance.PlayerWeapons[i].ID, ItemCount-1);
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		//If the player wishes to open/close their inventory
		if(Input.IsActionJustPressed("p_inv"))
		{
			Inv.Visible = !Inv.Visible;
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
			CurrentInteraction.BeginDialogue();
		}
		
		//If the player wishes to create a projectile and their cooldown is over
		if(Input.IsActionJustPressed("p_ranged") && RangedCooldown)
		{
			CreateProjectile();
		}
		
		//If the animation is currently not walking
		if (!MySpriteAnimation.Animation.ToString().StartsWith("Walk"))
		{
			//If the animation is not playing, have the player face its current direction
			if (!MySpriteAnimation.IsPlaying())
			{
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
		
	}
	
	//Shoot a bullet
	public void CreateProjectile()
	{
		//If the ranged weapon is still cooling down, don't shoot the bullet
		if(!RangedCooldown)
		{
			return;
		}
		//Unpack the scene
		PackedScene PlayerProjectileScene = GD.Load<PackedScene>("res://Packed Scenes/Projectiles/Player Projectile.tscn");
		Projectile NewBullet = (Projectile)PlayerProjectileScene.Instantiate();
		//Set Position and Direction
		if(CurrentDir == "D")
		{
			NewBullet.Direction = new Vector2(0,1);
			NewBullet.Position = new Vector2(GlobalPosition.X,GlobalPosition.Y+65);
		}
		else if(CurrentDir == "U")
		{
			NewBullet.Direction = new Vector2(0,-1);
			NewBullet.Position = new Vector2(GlobalPosition.X,GlobalPosition.Y-65);
		}
		else if(CurrentDir == "L")
		{
			NewBullet.Direction = new Vector2(-1,0);
			NewBullet.Position = new Vector2(GlobalPosition.X-65,GlobalPosition.Y);
		}
		else if(CurrentDir == "R")
		{
			NewBullet.Direction = new Vector2(1,0);
			NewBullet.Position = new Vector2(GlobalPosition.X+65,GlobalPosition.Y);
		}
		//Create the bullet
		GetTree().GetRoot().AddChild(NewBullet);
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
	
	//Start the cool down for the ranged attack
	public async void RangedCoolDown()
	{
		RangedCooldown = false;
		await ToSignal(GetTree().CreateTimer(2),"timeout");
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
				NewWeapon.Damage = 10;
				NewWeapon.CoolDown = 5;
				path = "res://Art Assets/Items/gun_Pistol.png";
			}
			else if(ItemId == 2)
			{ //Knife
				NewWeapon.Damage = 10;
				NewWeapon.CoolDown = 5;
				path = "res://Art Assets/Items/knife.png";
			}
			else if(ItemId == 3)
			{ //Shotgun
				NewWeapon.Damage = 20;
				NewWeapon.CoolDown = 10;
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
