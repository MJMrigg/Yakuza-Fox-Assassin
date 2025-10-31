using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public float Speed = 300.0f; //Movement speed
	public Interactable interacted = null; //Object to be interacted with
	public bool IsInteracting = false; //Whether the player is interacting

	public override void _Process(double delta){
		//Get Inputs
		float translate_x = Input.GetAxis("p_left","p_right");
		float translate_y = Input.GetAxis("p_up","p_down");
		//Set up velocity
		Vector2 DesiredVelocity = Vector2.Zero;
		//Get direction of velocity
		if(Math.Abs(translate_x) >= Math.Abs(translate_y)){
			DesiredVelocity = new Vector2(translate_x,0);
		}else{
			DesiredVelocity = new Vector2(0,translate_y);
		}
		//Add speed
		Velocity = DesiredVelocity*Speed;
		//FREWAKING MOVEAS!!! SERIOUSLY, STOP FOGETTING THIS STESP!
		if(!IsInteracting){ //Unless the player is interacting with something
			MoveAndSlide();
		}
		
		//If the player presses E, interact
		if(Input.IsActionJustPressed("p_interact")){
			Interact();
		}
	}
	
	//Show an object is interactable
	public void ShowInteractable(Node2D Body){
		if(Body is Interactable){
			Interactable InteractionObject = (Interactable)Body;
			InteractionObject.E.Visible = true;
			interacted = (Interactable)Body;
		}
	}
	
	//Stop showing an object is interactable
	public void HideInteractable(Node2D Body){
		if(Body is Interactable){
			Interactable InteractionObject = (Interactable)Body;
			InteractionObject.E.Visible = false;
			interacted = null;
		}
	}
	
	//When the player interacts with an object
	public void Interact(){
		//If there is no object to interact with, return
		if(interacted == null){
			return;
		}
		//Start the interaction
		IsInteracting = true;
		interacted.Interact();
	}
}
