using Godot;
using System;

public partial class GlobalSusMeter : ProgressBar
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Set value and max value
		Value = Game.Instance.GlobalSuspicion;
		MaxValue = Game.Instance.MaxGlobalSuspicion;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Update global suspicion in real time
		Value = Game.Instance.GlobalSuspicion;
		//Update border width based on if the bar is filled or not
		//Ensures the fill right border color doesn't show up when it's not full
		if(Value >= MaxValue)
		{
			((StyleBoxFlat)GetThemeStylebox("fill")).BorderWidthRight = 2;
		}else{
			((StyleBoxFlat)GetThemeStylebox("fill")).BorderWidthRight = 0;
		}
	}
}
