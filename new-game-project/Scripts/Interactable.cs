using Godot;
using System;

public partial class Interactable : CharacterBody2D{
	[Export] public Control E;
	[Export] public Control DialogScreen;
	[Export] public String[] DialogOptions;
	[Export] public String[] DialogAnswers;
	
	public void Interact(){
		//If there is no dialog screen, return
		if(DialogScreen == null){
			GD.Print("Error: No Dialog Screen");
			return;
		}
		//Show the dialog screen
		((ColorRect)DialogScreen.GetNode("Screen")).Show();
		for(int i = 0; i < DialogOptions.Length; i++){
			GD.Print(DialogOptions[i]);
		}
	}
}
