using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;

	public override void _Process(double delta){
		//Get Inputs
		Vector2 translate = Input.GetVector("p_left","p_right","p_up","p_down");
		//Set up velocity
		Vector2 DesiredVelocity = translate*Speed;
		Velocity = DesiredVelocity;
		//FREWAKING MOVEAS!!! SERIOUSLY, STOP FOGETTING THIS STESP!@
		MoveAndSlide();
	}
	
	public void ShowInteractable(Node2D Body){
		if(Body is Interactable){
			Interactable InteractionObject = (Interactable)Body;
			InteractionObject.E.Visible = true;
		}
	}
	
	public void HideInteractable(Node2D Body){
		if(Body is Interactable){
			Interactable InteractionObject = (Interactable)Body;
			InteractionObject.E.Visible = false;
		}
	}
}
