using Godot;
using System;

public partial class DialogueOption : Button
{
	public string Dialogue; //Text that the dialogue is
	
	[Signal]
	public delegate void ChooseOptionEventHandler(int Chosen);
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Text = Dialogue; //Set the text on the button to the text in the dialogue
		SetTheme(null);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	//Tell the interactable what option was chosen
	public void ReturnOption()
	{
		GD.Print(((StyleBoxFlat)GetThemeStylebox("Normal")).GetBgColor());
		//Mark the option as clicked
		StyleBoxFlat Normal = ((StyleBoxFlat)GetThemeStylebox("Normal"));
		StyleBoxFlat Pressed = ((StyleBoxFlat)GetThemeStylebox("Pressed"));
		Normal.SetBgColor(Pressed.GetBgColor());
		AddThemeStyleboxOverride("Normal",Normal);
		GD.Print(Normal.GetBgColor());
		GD.Print(((StyleBoxFlat)GetThemeStylebox("Normal")).GetBgColor());
		//Emit the index of the dialogue option in the dialog tree
		((AudioStreamPlayer)GetNode("ButtonClicked")).Play();
		EmitSignal(SignalName.ChooseOption, GetIndex());
		//Remove the dialogue option from the dialogue options
		//QueueFree();
	}
}
